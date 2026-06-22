import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, MatToolbarModule, MatButtonModule, MatIconModule],
  template: `
    <mat-toolbar>
      <a routerLink="/books" class="brand">BookCatalog</a>
      <span class="toolbar-spacer"></span>
      <a mat-button routerLink="/books">Books</a>
      @if (auth.isLoggedIn()) {
        <a mat-button routerLink="/admin/books">Admin</a>
        <button mat-icon-button type="button" aria-label="Sign out" (click)="auth.logout()">
          <mat-icon>logout</mat-icon>
        </button>
      } @else {
        <a mat-button routerLink="/login">Login</a>
        <a mat-raised-button routerLink="/register">Register</a>
      }
    </mat-toolbar>
    <router-outlet />
  `,
  styles: [`
    mat-toolbar {
      position: sticky;
      top: 0;
      z-index: 10;
      border-bottom: 1px solid #dfe4ee;
      background: #ffffff;
    }
    .brand {
      font-weight: 700;
      font-size: 18px;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {
  readonly auth = inject(AuthService);
}
