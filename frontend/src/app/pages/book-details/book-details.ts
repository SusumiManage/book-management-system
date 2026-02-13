import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { timeout } from 'rxjs';

import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { ConfirmDialogComponent } from '../../confimation/confirm-dialog';
import { Book } from '../../models/book';
import { BookService } from '../../services/book';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-book-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatDialogModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatProgressBarModule,
  ],
  templateUrl: './book-details.html'
})
export class BookDetailsComponent implements OnInit {
  book?: Book;
  loading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService,
    public auth: AuthService,
    private dialog: MatDialog
    //private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadBookDetails();
  }

  loadBookDetails() {
    debugger
    this.route.paramMap.subscribe(params => {
      const idStr = params.get('id');
      const id = Number(idStr);

      this.book = undefined;
      this.error = null;
      this.loading = true;

      if (!idStr || Number.isNaN(id)) {
        this.error = 'Invalid book id in URL.';
        this.loading = false;
        return;
      }

      this.bookService.getBookById(id).pipe(
        timeout(5000)
      ).subscribe({
        next: (b) => {
          this.book = b;
          this.loading = false;
        },
        error: () => {
          this.error = 'Book not found.';
          this.loading = false;
        }
      });
    });
  }

  delete(bookId: number): void {
    if (!bookId) return;
    
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

      this.bookService.deleteBook(bookId).subscribe({
        next: () => {
          this.router.navigate(['/books']);
        },
        error: () => alert('Failed to delete book')
      });
    });
  }


}





