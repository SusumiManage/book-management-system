import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { BookBorrowedDetailsService } from '../../services/book-borrowed-details';
import { PagedResult } from '../../models/book';
import { OverdueBorrowBooksDto } from '../../models/OverdueBorrowBooksDto';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
    selector: 'app-overdue-books',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatTableModule,
        MatIconModule,
        MatProgressBarModule,
        MatTooltipModule
    ],
    templateUrl: './overdue-books-list.html'
})
export class OverdueBooksComponent implements OnInit {
    paged?: PagedResult<OverdueBorrowBooksDto>;
    loading = false;
    error: string | null = null;

    pageNumber = 1;
    pageSize = 10;

    search = '';
    userId: number | null = null;

    displayedColumns = ['bookTitle', 'borrowedByUsername', 'dueAt', 'daysOverdue', 'actions'];

    constructor(
        private svc: BookBorrowedDetailsService,
        private route: ActivatedRoute,
        private router: Router
    ) { }

    ngOnInit(): void {
        const uid = this.route.snapshot.queryParamMap.get('userId');
        this.userId = uid ? Number(uid) : null;
        this.load();
    }

    load(): void {
        this.loading = true;
        this.error = null;

        this.svc.getOverdue({
            pageNumber: this.pageNumber,
            pageSize: this.pageSize,
            search: this.search || undefined,
            userId: this.userId ?? undefined
        }).subscribe({
            next: (res) => { this.paged = res; this.loading = false; },
            error: (err) => { this.error = err?.error?.message || 'Failed to load overdue books.'; this.loading = false; }
        });
    }

    apply(): void { this.pageNumber = 1; this.load(); }
    clear(): void { this.search = ''; this.pageNumber = 1; this.load(); }

    prev(): void {
        if (this.pageNumber > 1) { this.pageNumber--; this.load(); }
    }

    next(): void {
        if (this.paged && this.pageNumber < this.paged.totalPages) { this.pageNumber++; this.load(); }
    }

    // optional: return directly from overdue list
    returnBook(row: OverdueBorrowBooksDto): void {
        this.svc.returnBook({ bookId: row.bookId }).subscribe({
            next: () => this.load(),
            error: (err) => alert(err?.error?.message || 'Failed to return book')
        });
    }
}
