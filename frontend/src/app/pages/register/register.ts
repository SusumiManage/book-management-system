import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthApiService, RegisterRequest } from '../../services/auth-api.service';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './register.html',
})
export class RegisterComponent {
  roles: Array<RegisterRequest['role']> = ['Admin', 'User'];

  isSubmitting = false;
  message = '';
  error = '';

  form;

  constructor(private fb: FormBuilder, private auth: AuthApiService) {
    this.form = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      role: ['User' as RegisterRequest['role'], [Validators.required]],
    });
  }

  submit() {
    this.message = '';
    this.error = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const payload = this.form.getRawValue() as RegisterRequest;

    this.auth.register(payload).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.message = 'User registered successfully!';
        this.form.reset({ username: '', password: '', role: 'User' });
      },
      error: (err) => {
        this.isSubmitting = false;
        this.error =
          err?.error?.message ??
          err?.error ??
          'Registration failed. Check API endpoint and validation.';
      },
    });
  }

  get f() {
    return this.form.controls;
  }
}

