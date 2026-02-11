import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router'

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button'
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { BookService } from '../../services/book';
import { Book, PagedResult } from '../../models/book';
import { AuthService } from '../../services/auth.service';
import { ConfirmDialogComponent } from '../../confimation/confirm-dialog';
import { BookBorrowedDetailsService } from '../../services/book-borrowed-details';
import { BorrowDialogComponent } from '../../borrow-book/borrow-dialog';
import { BorrowBookRequestDto } from '../../models/borrow-book';

@Component({
  selector: 'app-books-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatProgressBarModule,
    MatTooltipModule
  ],
  templateUrl: './books-list.html'
})
export class BooksListComponent implements OnInit {
  paged?: PagedResult<Book>;
  loading = false;
  error: string | null = null;

  pageNumber = 1;
  pageSize = 5;

  search = '';
  genre = '';
  yearFrom: number | null = null;
  yearTo: number | null = null;
  minPrice: number | null = null;
  maxPrice: number | null = null;
  availability: boolean | null = null;
  isDeleted: boolean | null = null;

  isAdmin = false;
  displayedColumns: string[] = [];

  constructor(
    private bookService: BookService,
    public auth: AuthService,
    private router: Router,
    private dialog: MatDialog,
    private borrowService: BookBorrowedDetailsService
  ) { }

  ngOnInit(): void {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    this.isAdmin = this.auth.isAdmin();
    this.setDisplayedColumns();

    this.load();
  }

  private setDisplayedColumns(): void {
    this.displayedColumns = ['title', 'author', 'genre', 'year', 'isbn', 'price', 'availability'];

    if (this.isAdmin) {
      this.displayedColumns.push('deleteStatus');
    }

    this.displayedColumns.push('actions');
  }

  load(): void {
    this.loading = true;
    this.error = null;

    this.bookService.getBooks({
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      search: this.search || undefined,
      genre: this.genre || undefined,
      yearFrom: this.yearFrom,
      yearTo: this.yearTo,
      minPrice: this.minPrice,
      maxPrice: this.maxPrice,
      isAvailable: this.availability,
      isDeleted: this.isDeleted
    }).subscribe({
      next: (res: PagedResult<Book>) => {

        this.paged = res;

        if (this.isAdmin) {
          this.loadBorrowStatuses();
        }

        this.loading = false;
      },
      error: (err: any) => {
        this.error = err?.error?.message || 'Failed to load books.';
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.load();
  }

  clearFilters(): void {
    this.search = '';
    this.genre = '';
    this.yearFrom = null;
    this.yearTo = null;
    this.minPrice = null;
    this.maxPrice = null;
    this.availability = null;
    this.isDeleted = null;
    this.pageNumber = 1;
    this.load();
  }

  prev(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.load();
    }
  }

  next(): void {
    if (this.paged && this.pageNumber < this.paged.totalPages) {
      this.pageNumber++;
      this.load();
    }
  }

  private loadBorrowStatuses(): void {
    if (!this.paged?.items?.length) return;

    this.borrowService.getAllActive().subscribe({
      next: (activeList) => {
        const activeMap = new Map(activeList.map(a => [a.bookId, a]));

        this.paged!.items = this.paged!.items.map(b => {
          const active = activeMap.get(b.id);

          const availableByBorrow = !active;
          const availableFinal = !b.isDeleted && availableByBorrow;

          return {
            ...b,
            isAvailable: availableFinal,
            borrowedByUsername: active?.borrowedByUsername ?? null,
            dueAt: active?.dueAt ?? null
          };
        });
      },
      error: () => {
        this.paged!.items = this.paged!.items.map(b => ({
          ...b,
          isAvailable: true,
          borrowedByUsername: null,
          dueAt: null
        }));
      }
    });
  }

  borrow(book: Book): void {
    if (!this.auth.isAdmin()) return;

    const ref = this.dialog.open(BorrowDialogComponent, {
      width: '560px',
      maxWidth: '95vw',
      disableClose: true,
      data: {
        bookId: book.id,
        title: book.title
      }
    });

    ref.afterClosed().subscribe(result => {
      if (!result) return;

      this.borrowBook(
        book,
        result.borrowedByUserId,
        result.dueAt ? new Date(result.dueAt) : null
      );
    });
  }

  borrowBook(book: Book, borrowedByUserId: number, dueDate: Date | null): void {
    const dto: BorrowBookRequestDto = {
      bookId: book.id,
      borrowedByUserId,
      dueAt: dueDate ? dueDate.toISOString() : null
    };

    this.borrowService.borrow(dto).subscribe({
      next: () => {
        book.isAvailable = false;
        this.load();
      },
      error: (err) => {
        const msg = err?.error?.message || 'Failed to borrow book';

        if (msg.toLowerCase().includes('overdue')) {
          // navigate to overdue page filtered by the user
          this.router.navigate(['/overdue'], { queryParams: { userId: dto.borrowedByUserId } });
          return;
        }

        alert(msg);
      }
    });
  }


  returnBook(book: Book): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '420px',
      data: {
        title: 'Return Book',
        message: `Are you sure you want to return "${book.title}"?`,
        confirmText: 'Return',
        confirmColor: 'primary'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;

      this.borrowService.returnBook({ bookId: book.id }).subscribe({
        next: () => {
          book.isAvailable = true;
          this.load();
        },
        error: (err) =>
          alert(err?.error?.message || 'Failed to return book')
      });
    });
  }

  goToDetails(book: Book) {
    this.router.navigate(['/books', book.id]);
  }

  goToEdit(book: Book) {
    if (book.isDeleted) return;
    this.router.navigate(['/books', book.id, 'edit']);
  }


  delete(id: number): void {
    if (!id) return;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '420px',
      data: {
        title: 'Confirm Delete',
        message: 'Are you sure you want to delete this book?',
        warning: 'This action cannot be undone.',
        confirmText: 'Delete',
        confirmColor: 'warn'
      }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) return;

      this.bookService.deleteBook(id).subscribe({
        next: () => { this.load(); },
        error: () => alert('Failed to delete book')
      });
    });
  }

  restore(book: Book): void {
    if (!this.auth.isAdmin()) return;

    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '420px',
      data: {
        title: 'Restore Book',
        message: `Do you want to restore "${book.title}"?`,
        confirmText: 'Restore',
        confirmColor: 'primary'
      }
    });

    ref.afterClosed().subscribe(result => {
      if (result !== true) return;

      this.bookService.restore(book.id).subscribe({
        next: () => {
          book.isDeleted = false;
          book.deletedAt = null;
          book.deletedByUserId = null;

          this.load();
        },
        error: err =>
          alert(err?.error?.message || 'Failed to restore book')
      });
    });
  }



}
