import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { BookService } from './book';

describe('BookService', () => {
  let service: BookService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    service = TestBed.inject(BookService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  // TEST 1: service created
  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  // TEST 2: getBooks sends query params correctly
  it('getBooks should call GET with correct query params', () => {
    service.getBooks({
      pageNumber: 2,
      pageSize: 5,
      search: 'clean',
      genre: 'Programming',
      yearFrom: 2000,
      yearTo: 2020,
      minPrice: 10,
      maxPrice: 50,
      isAvailable: true,
      isDeleted: false
    }).subscribe();

    const req = httpMock.expectOne(r => r.method === 'GET' && r.url.includes('/api/books'));

    expect(req.request.params.get('pageNumber')).toBe('2');
    expect(req.request.params.get('pageSize')).toBe('5');
    expect(req.request.params.get('search')).toBe('clean');
    expect(req.request.params.get('genre')).toBe('Programming');

    expect(req.request.params.get('yearFrom')).toBe('2000');
    expect(req.request.params.get('yearTo')).toBe('2020');
    expect(req.request.params.get('minPrice')).toBe('10');
    expect(req.request.params.get('maxPrice')).toBe('50');

    expect(req.request.params.get('isAvailable')).toBe('true');
    expect(req.request.params.get('isDeleted')).toBe('false');

    req.flush({ items: [], pageNumber: 2, pageSize: 5, totalItems: 0, totalPages: 0 });
  });

  // TEST 3: getBookById calls correct URL
  it('getBookById should call GET /api/books/{id}', () => {
    service.getBookById(10).subscribe(res => {
      expect(res.id).toBe(10);
    });

    const req = httpMock.expectOne(r => r.method === 'GET' && r.url.endsWith('/api/books/10'));
    req.flush({ id: 10, title: 'Clean Code' });
  });

  // TEST 4: createBook sends POST with payload
  it('createBook should POST to /api/books with payload', () => {
    const payload: any = {
      title: 'New Book',
      author: 'Someone',
      genre: 'Programming',
      publicationYear: 2024,
      isbn: '123',
      price: 10
    };

    service.createBook(payload).subscribe(res => {
      expect(res.title).toBe('New Book');
    });

    const req = httpMock.expectOne(r => r.method === 'POST' && r.url.includes('/api/books'));
    expect(req.request.body).toEqual(payload);

    req.flush({ id: 99, ...payload });
  });

  // TEST 5: restore calls POST /api/books/{id}/restore with {}
  it('restore should POST /api/books/{id}/restore with empty body', () => {
    service.restore(7).subscribe();

    const req = httpMock.expectOne(r => r.method === 'POST' && r.url.endsWith('/api/books/7/restore'));
    expect(req.request.body).toEqual({});

    req.flush(null);
  });

  // TEST 6: updateBook calls PUT /api/books/{id} with payload
  it('updateBook should PUT /api/books/{id} with payload', () => {
    const payload: any = {
      title: 'Updated Book',
      author: 'Author',
      genre: 'Programming',
      publicationYear: 2023,
      isbn: '999',
      price: 20
    };

    service.updateBook(5, payload).subscribe();

    const req = httpMock.expectOne(r =>
      r.method === 'PUT' && r.url.endsWith('/api/books/5')
    );

    expect(req.request.body).toEqual(payload);

    req.flush(null);
  });

  // TEST 7: deleteBook calls DELETE /api/books/{id} with payload
  it('deleteBook should DELETE /api/books/{id}', () => {
    service.deleteBook(8).subscribe();

    const req = httpMock.expectOne(r =>
      r.method === 'DELETE' && r.url.endsWith('/api/books/8')
    );

    req.flush(null);
  });


});
