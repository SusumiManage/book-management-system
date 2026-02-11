import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { BooksListComponent } from './books-list';

import { BookService } from '../../services/book';
import { AuthService } from '../../services/auth.service';
import { BookBorrowedDetailsService } from '../../services/book-borrowed-details';
import { provideRouter, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

describe('BooksListComponent', () => {
  let component: BooksListComponent;
  let fixture: ComponentFixture<BooksListComponent>;

  let bookService: jasmine.SpyObj<BookService>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;
  let dialog: jasmine.SpyObj<MatDialog>;
  let borrowService: jasmine.SpyObj<BookBorrowedDetailsService>;

  beforeEach(async () => {
    bookService = jasmine.createSpyObj('BookService', ['getBooks', 'deleteBook', 'restore']);
    authService = jasmine.createSpyObj('AuthService', ['isLoggedIn', 'isAdmin']);
    router = jasmine.createSpyObj('Router', ['navigate']);
    dialog = jasmine.createSpyObj('MatDialog', ['open']);
    borrowService = jasmine.createSpyObj('BookBorrowedDetailsService', [
      'getAllActive',
      'borrow',
      'returnBook'
    ]);

    // defaults
    authService.isLoggedIn.and.returnValue(true);
    authService.isAdmin.and.returnValue(false);

    bookService.getBooks.and.returnValue(of({
      items: [],
      pageNumber: 1,
      pageSize: 5,
      totalItems: 0,
      totalPages: 1
    } as any));

    await TestBed.configureTestingModule({
      imports: [BooksListComponent],
      providers: [
        provideRouter([]),

        { provide: BookService, useValue: bookService },
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router },
        { provide: MatDialog, useValue: dialog },
        { provide: BookBorrowedDetailsService, useValue: borrowService }
      ],
      schemas: [NO_ERRORS_SCHEMA] // ignore material + dialog templates
    }).compileComponents();

    fixture = TestBed.createComponent(BooksListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // triggers ngOnInit
  });

  // TEST 1: component creation
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // TEST 2: prev() should decrement page and reload
  it('prev should decrement pageNumber and call load', () => {
    component.pageNumber = 3;
    spyOn(component, 'load');

    component.prev();

    expect(component.pageNumber).toBe(2);
    expect(component.load).toHaveBeenCalled();
  });

  // TEST 3: admin loadBorrowStatuses maps borrowed books correctly
  it('loadBorrowStatuses should mark borrowed books as unavailable', () => {
    component.paged = {
      items: [
        { id: 1, title: 'Book A', isDeleted: false } as any,
        { id: 2, title: 'Book B', isDeleted: false } as any
      ]
    } as any;

    borrowService.getAllActive.and.returnValue(of([
      {
        bookId: 1,
        borrowedByUsername: 'john',
        dueAt: '2030-01-01T00:00:00.000Z'
      } as any
    ]));

    // call private method safely
    (component as any).loadBorrowStatuses();

    const b1 = component.paged!.items[0] as any;
    const b2 = component.paged!.items[1] as any;

    expect(b1.isAvailable).toBeFalse();
    expect(b1.borrowedByUsername).toBe('john');

    expect(b2.isAvailable).toBeTrue();
    expect(b2.borrowedByUsername).toBeNull();
  });
});
