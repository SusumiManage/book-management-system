import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Book, BookCreate, PagedResult } from '../models/book';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private baseUrl = `${environment.apiBaseUrl}/api/books`;

  constructor(private http: HttpClient) { }

  getBooks(options: {
    pageNumber?: number;
    pageSize?: number;
    search?: string;
    genre?: string;
    yearFrom?: number | null;
    yearTo?: number | null;
    minPrice?: number | null;
    maxPrice?: number | null;
    isAvailable?: boolean | null;
    isDeleted?: boolean | null;
  }): Observable<PagedResult<Book>> {
    let params = new HttpParams();

    if (options.pageNumber) params = params.set('pageNumber', options.pageNumber);
    if (options.pageSize) params = params.set('pageSize', options.pageSize);
    if (options.search) params = params.set('search', options.search);
    if (options.genre) params = params.set('genre', options.genre);

    if (options.yearFrom != null) params = params.set('yearFrom', options.yearFrom);
    if (options.yearTo != null) params = params.set('yearTo', options.yearTo);

    if (options.minPrice != null) params = params.set('minPrice', options.minPrice);
    if (options.maxPrice != null) params = params.set('maxPrice', options.maxPrice);

    if (options.isAvailable != null) params = params.set('isAvailable', String(options.isAvailable));
    if (options.isDeleted != null) params = params.set('isDeleted', String(options.isDeleted));

    return this.http.get<PagedResult<Book>>(this.baseUrl, { params });
  }

  getBookById(id: number): Observable<Book> {
    return this.http.get<Book>(`${this.baseUrl}/${id}`);
  }

  createBook(payload: BookCreate): Observable<Book> {
    return this.http.post<Book>(this.baseUrl, payload);
  }

  updateBook(id: number, payload: BookCreate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  deleteBook(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  restore(bookId: number) {
    return this.http.post<void>(`${this.baseUrl}/${bookId}/restore`, {});
  }
}
