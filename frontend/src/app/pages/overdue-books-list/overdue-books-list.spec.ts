import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OverdueBooksComponent } from './overdue-books-list';
import { provideRouter } from '@angular/router';

describe('OverdueBooks', () => {
    let component: OverdueBooksComponent;
    let fixture: ComponentFixture<OverdueBooksComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [OverdueBooksComponent],
            providers: [provideRouter([])]
        })
            .compileComponents();

        fixture = TestBed.createComponent(OverdueBooksComponent);
        component = fixture.componentInstance;
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
