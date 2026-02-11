export interface BorrowBookRequestDto {
    bookId: number;
    borrowedByUserId: number;
    dueAt?: string | null;
}

export interface ReturnBookRequestDto {
    bookId: number;
}

export interface BookBorrowedDetailResponseDto {
    id: number;
    bookId: number;
    borrowedByUserId: number;
    borrowedByUsername?: string | null;
    issuedByUserId: number;
    issuedByUsername?: string | null;
    borrowedAt: string;
    issuedAt: string;
    dueAt?: string | null;
    returnedAt?: string | null;
}
