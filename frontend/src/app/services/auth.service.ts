import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private tokenKey = 'token';

    setToken(token: string) {
        sessionStorage.setItem(this.tokenKey, token);
    }

    getToken(): string | null {
        return sessionStorage.getItem(this.tokenKey);
    }

    logout() {
        sessionStorage.removeItem(this.tokenKey);
    }

    // Decode JWT payload to read role
    getUserRole(): string | null {
        const token = this.getToken();
        if (!token) return null;

        const payload = token.split('.')[1];
        if (!payload) return null;

        const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
        const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=');

        try {
            const decoded = JSON.parse(atob(padded));
            const roleKey =
                'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

            return decoded?.[roleKey] ?? decoded?.role ?? null;
        } catch {
            return null;
        }
    }

    isAdmin(): boolean {
        return this.getUserRole() === 'Admin';
    }

    isLoggedIn(): boolean {
        return !!this.getToken();
    }
}
