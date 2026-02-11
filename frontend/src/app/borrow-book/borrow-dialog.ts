import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';

import { AuthApiService } from '../services/auth-api.service';
import { UserListDto } from '../models/user-list';

export interface UserLookupDto {
  id: number;
  username: string;
  role: string;
}

@Component({
  selector: 'app-borrow-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatOptionModule,
    MatInputModule,
    MatButtonModule,
    MatProgressBarModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule
  ],
  template: `
  <h2 mat-dialog-title>Borrow Book</h2>

  <mat-dialog-content class="dialog-content">
    <div class="book-row">
      <span class="label">Book:</span>
      <span class="value">{{ data.title }}</span>
    </div>

    <mat-progress-bar *ngIf="loading" mode="indeterminate"></mat-progress-bar>

    <mat-form-field appearance="outline" class="full-width">
      <mat-label>Borrowed By User</mat-label>
      <mat-select [(ngModel)]="borrowedByUserId" [disabled]="loading">
        <mat-option *ngFor="let u of users" [value]="u.id">
          {{ u.username }} ({{ u.role }})
        </mat-option>
      </mat-select>
    </mat-form-field>

    <mat-form-field appearance="outline" class="full-width">
      <mat-label>Due Date</mat-label>
      <input
        matInput
        [matDatepicker]="picker"
        [(ngModel)]="dueDate"
        [min]="minDate" 
        [disabled]="loading"
        (click)="picker.open()"
      />
      <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
      <mat-datepicker #picker></mat-datepicker>
    </mat-form-field>

    <div *ngIf="error" class="error-text">{{ error }}</div>
  </mat-dialog-content>

  <mat-dialog-actions align="end">
    <button mat-stroked-button (click)="close()" [disabled]="loading">
      Cancel
    </button>

    <button
      mat-flat-button
      color="primary"
      (click)="save()"
      [disabled]="loading || !borrowedByUserId || !dueDate"
    >
      Borrow
    </button>
  </mat-dialog-actions>
`,
  styles: [`
  .dialog-content {
    width: 520px;
    max-width: calc(100vw - 48px);
    padding-top: 8px;
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  .book-row {
    display: flex;
    gap: 6px;
    font-size: 14px;
    opacity: 0.85;
  }

  .book-row .label {
    font-weight: 500;
  }

  .full-width {
    width: 100%;
  }

  .error-text {
    color: #b00020;
    font-size: 13px;
    margin-top: -4px;
  }
`]


})
export class BorrowDialogComponent implements OnInit {
  users: UserListDto[] = [];
  borrowedByUserId: number | null = null;
  dueDate: Date | null = null;

  minDate = new Date();

  loading = false;
  error: string | null = null;

  constructor(
    private ref: MatDialogRef<BorrowDialogComponent>,

    @Inject(MAT_DIALOG_DATA) public data: { bookId: number; title: string },
    private authApiService: AuthApiService
  ) { }

  ngOnInit(): void {

    this.minDate.setHours(0, 0, 0, 0);

    this.loading = true;

    this.authApiService.getRegisteredUsers().subscribe({
      next: (res) => {
        this.users = res ?? [];
        this.loading = false;

        if (this.users.length === 0) {
          this.error = 'No registered users found. Please register users first.';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.message || 'Failed to load users (check API + token).';
      }
    });
  }

  close() {
    this.ref.close(null);
  }

  save() {
    if (!this.borrowedByUserId) {
      this.error = 'Please select a registered user.';
      return;
    }

    const dueAt = this.dueDate ? this.dueDate.toISOString() : null;

    this.ref.close({
      borrowedByUserId: this.borrowedByUserId,
      dueAt
    });
  }

}





