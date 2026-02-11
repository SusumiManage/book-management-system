export interface OverdueBorrowBooksDto {
    borrowId: number;
    bookId: number;
    bookTitle: string;
    isbn?: string | null;
    borrowedByUserId: number;
    borrowedByUsername: string;
    borrowedAt: string;
    dueAt?: string | null;
    daysOverdue: number;
}