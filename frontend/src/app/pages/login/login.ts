import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';

import { AuthApiService } from '../../services/auth-api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule
  ],
  templateUrl: './login.html'
})
export class LoginComponent {
  private fb = inject(FormBuilder);

  error: string | null = null;
  loading = false;

  hidePassword = true;

  form = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });
  constructor(
    private api: AuthApiService,
    private auth: AuthService,
    private router: Router
  ) { }

  get username() { return this.form.get('username'); }
  get password() { return this.form.get('password'); }

  login() {
    this.error = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { username, password } = this.form.value;

    this.loading = true;

    this.api.login(username!, password!).subscribe({
      next: (res) => {
        this.auth.setToken(res.token);
        this.router.navigate(['/books']);
      },
      error: () => {
        this.error = 'Invalid login.';
        this.loading = false;
      }
    });

    setTimeout(() => {
      this.loading = false;

      console.log('Submitting:', { username, password });
    }, 700);
  }

  togglePassword() {
    this.hidePassword = !this.hidePassword;
  }
}
