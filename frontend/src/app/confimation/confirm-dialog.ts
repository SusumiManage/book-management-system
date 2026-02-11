import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

@Component({
  standalone: true,
  selector: 'app-confirm-delete-dialog',
  imports: [MatDialogModule, MatButtonModule, CommonModule,],
  template: `
    <h2 mat-dialog-title>{{ data.title ?? 'Confirm Action' }}</h2>
      <mat-dialog-content>
        {{ data.message ?? 'Are you sure?' }}
        <br />
        <strong *ngIf="data.warning">{{ data.warning }}</strong>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button [mat-dialog-close]="false">
          {{ data.cancelText ?? 'Cancel' }}
        </button>

        <button
          mat-button
          [color]="data.confirmColor ?? 'warn'"
          [mat-dialog-close]="true"
        >
          {{ data.confirmText ?? 'Confirm' }}
        </button>
      </mat-dialog-actions>
  `
})
export class ConfirmDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      title?: string;
      message?: string;
      warning?: string;
      confirmText?: string;
      cancelText?: string;
      confirmColor?: 'primary' | 'accent' | 'warn';
    }
  ) { }
}

