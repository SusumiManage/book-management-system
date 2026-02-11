export interface Book {
    id: number;
    title: string;
    author: string;
    genre: string;
    publicationYear: number;
    isbn: string;
    price: number;
    isDeleted: boolean;
    deletedAt?: string | null;
    deletedByUserId: number | null;
    isAvailable: boolean;
    borrowedByUsername?: string | null;
    dueAt?: string | null;
}

export interface BookCreate {
    title: string;
    author: string;
    genre: string;
    publicationYear: number;
    isbn: string;
    price: number;
}

export interface PagedResult<T> {
    items: T[];
    pageNumber: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}