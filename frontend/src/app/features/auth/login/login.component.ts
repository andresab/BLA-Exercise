import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSnackBarModule],
  template: `
    <main class="page auth-page">
      <mat-card appearance="outlined">
        <mat-card-header>
          <mat-card-title>Login</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form class="form-grid" [formGroup]="form" (ngSubmit)="submit()">
            <mat-form-field appearance="outline">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Password</mat-label>
              <input matInput type="password" formControlName="password" />
            </mat-form-field>
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || saving()">Login</button>
            <a mat-button routerLink="/register">Create account</a>
          </form>
        </mat-card-content>
      </mat-card>
    </main>
  `,
  styles: [`
    .auth-page {
      display: grid;
      place-items: start center;
    }
    mat-card {
      width: min(440px, 100%);
      border-radius: 8px;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  readonly saving = signal(false);
  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  submit(): void {
    if (this.form.invalid) {
      return;
    }

    this.saving.set(true);
    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => void this.router.navigateByUrl('/admin/books'),
      error: () => {
        this.saving.set(false);
        this.snackBar.open('Invalid email or password', 'Close', { duration: 3500 });
      }
    });
  }
}
