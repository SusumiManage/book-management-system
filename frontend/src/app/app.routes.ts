import { Routes } from '@angular/router';

import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },

    {
        path: 'login',
        loadComponent: () =>
            import('./pages/login/login').then(m => m.LoginComponent),
    },
    {
        path: 'register',
        loadComponent: () =>
            import('./pages/register/register').then(m => m.RegisterComponent),
    },
    {
        path: 'books/new',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./pages/book-form/book-form').then(m => m.BookFormComponent),
    },
    {
        path: 'books/:id/edit',
        canActivate: [adminGuard],
        loadComponent: () =>
            import('./pages/book-form/book-form').then(m => m.BookFormComponent),
    },
    {
        path: 'books/:id',
        loadComponent: () =>
            import('./pages/book-details/book-details').then(m => m.BookDetailsComponent),
    },

    {
        path: 'books',
        loadComponent: () =>
            import('./pages/books-list/books-list').then(m => m.BooksListComponent),
    },
    {
        path: 'overdue',
        loadComponent: () =>
            import('./pages/overdue-books-list/overdue-books-list').then(m => m.OverdueBooksComponent),
    },
    {
        path: '**',
        loadComponent: () =>
            import('./pages/not-found/not-found').then(m => m.NotFoundComponent),
    },
];
