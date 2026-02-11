import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { BookCreate } from '../../models/book';
import { BookService } from '../../services/book';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule
  ],
  templateUrl: './book-form.html'
})
export class BookFormComponent implements OnInit {
  id: number | null = null;
  isEdit = false;
  loading = false;
  error: string | null = null;

  model: BookCreate = {
    title: '',
    author: '',
    genre: '',
    publicationYear: new Date().getFullYear(),
    isbn: '',
    price: 0
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService
  ) { }

  ngOnInit(): void {
    debugger
    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');

      if (idParam) {
        this.id = Number(idParam);
        this.isEdit = true;
        this.loadForEdit(this.id);
      } else {
        this.id = null;
        this.isEdit = false;
        this.resetForNew();
      }

    });
  }

  loadForEdit(id: number): void {
    this.loading = true;
    this.bookService.getBookById(id).subscribe({
      next: (b) => {
        this.model = {
          title: b.title,
          author: b.author,
          genre: b.genre,
          publicationYear: b.publicationYear,
          isbn: b.isbn,
          price: b.price
        };
        this.loading = false;
      },
      error: () => {
        this.error = 'Book not found.';
        this.loading = false;
      }
    });
  }

  save(): void {
    this.error = null;

    // Validations
    if (!this.model.title || !this.model.author || !this.model.genre || !this.model.isbn) {
      this.error = 'Please fill Title, Author, Genre, and ISBN.';
      return;
    }

    this.loading = true;

    if (this.isEdit && this.id != null) {
      this.bookService.updateBook(this.id, this.model).subscribe({
        next: () => this.router.navigate(['/books', this.id]),
        error: (err) => {
          this.error = err?.error?.message || 'Update failed.';
          this.loading = false;
        }
      });
    } else {
      this.bookService.createBook(this.model).subscribe({
        next: (created) => this.router.navigate(['/books', created.id]),
        error: (err) => {
          this.error = err?.error?.message || 'Create failed.';
          this.loading = false;
        }
      });
    }
  }

  private resetForNew(): void {
    this.error = null;
    this.loading = false;

    this.model = {
      title: '',
      author: '',
      genre: '',
      publicationYear: new Date().getFullYear(),
      isbn: '',
      price: 0
    };
  }
}
