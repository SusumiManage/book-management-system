import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';

import { BorrowBookRequestDto, ReturnBookRequestDto, BookBorrowedDetailResponseDto } from '../models/borrow-book';
import { environment } from '../../environments/environment';
import { PagedResult } from '../models/book';
import { OverdueBorrowBooksDto } from '../models/OverdueBorrowBooksDto';

@Injectable({ providedIn: 'root' })
export class BookBorrowedDetailsService {
    private baseUrl = `${environment.apiBaseUrl}/api/BookBorrowedDetails`;

    constructor(private http: HttpClient) { }

    borrow(dto: BorrowBookRequestDto): Observable<BookBorrowedDetailResponseDto> {
        return this.http.post<BookBorrowedDetailResponseDto>(`${this.baseUrl}/borrow`, dto);
    }

    returnBook(dto: ReturnBookRequestDto): Observable<void> {
        return this.http.post<void>(`${this.baseUrl}/return`, dto);
    }


    getAllActive(): Observable<BookBorrowedDetailResponseDto[]> {
        return this.http.get<BookBorrowedDetailResponseDto[]>(`${this.baseUrl}/active`);
    }

    getOverdue(options: {
        pageNumber?: number;
        pageSize?: number;
        search?: string;
        userId?: number;
    }): Observable<PagedResult<OverdueBorrowBooksDto>> {
        let params = new HttpParams();

        if (options.pageNumber != null) params = params.set('pageNumber', options.pageNumber.toString());
        if (options.pageSize != null) params = params.set('pageSize', options.pageSize.toString());
        if (options.search) params = params.set('search', options.search);
        if (options.userId != null) params = params.set('userId', options.userId.toString());

        return this.http.get<PagedResult<OverdueBorrowBooksDto>>(`${this.baseUrl}/overdue`, { params });
    }

    getOverdueCount(userId?: number): Observable<{ totalOverdue: number }> {
        let params = new HttpParams();
        if (userId != null) params = params.set('userId', userId.toString());

        return this.http.get<{ totalOverdue: number }>(`${this.baseUrl}/overdue/count`, { params });
    }


}
