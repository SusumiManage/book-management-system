export interface ActiveBorrowBooksDto {
    bookId: number;
    borrowedByUsername: string | null;
    dueAt: string | null;
}