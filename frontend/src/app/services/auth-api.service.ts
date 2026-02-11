import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { UserListDto } from '../models/user-list';

export interface RegisterRequest {
    username: string;
    password: string;
    role: 'Admin' | 'User';
}

@Injectable({ providedIn: 'root' })
export class AuthApiService {
    constructor(private http: HttpClient) { }

    login(username: string, password: string): Observable<{ token: string }> {
        return this.http.post<{ token: string }>(`${environment.apiBaseUrl}/api/auth/login`, {
            username,
            password
        });
    }

    register(payload: RegisterRequest): Observable<any> {
        return this.http.post(
            `${environment.apiBaseUrl}/api/auth/register`,
            payload
        );
    }

    getRegisteredUsers(): Observable<UserListDto[]> {
        return this.http.get<UserListDto[]>(`${environment.apiBaseUrl}/api/auth/users`);
    }
}
