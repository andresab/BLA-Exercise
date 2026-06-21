import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';
import { BookService } from '../../../core/services/book.service';

@Component({
  selector: 'app-book-detail',
  standalone: true,
  imports: [DatePipe, MatButtonModule, MatCardModule, MatIconModule, RouterLink],
  template: `
    <main class="page">
      <a mat-button routerLink="/books"><mat-icon>arrow_back</mat-icon>Back</a>
      @if (book(); as item) {
        <mat-card appearance="outlined">
          <mat-card-header>
            <mat-card-title>{{ item.title }}</mat-card-title>
            <mat-card-subtitle>{{ item.author }}</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <dl>
              <dt>ISBN</dt><dd>{{ item.isbn }}</dd>
              <dt>Published</dt><dd>{{ item.publishedYear }}</dd>
              <dt>Created</dt><dd>{{ item.createdAt | date:'medium' }}</dd>
              <dt>Updated</dt><dd>{{ item.updatedAt | date:'medium' }}</dd>
            </dl>
          </mat-card-content>
        </mat-card>
      }
    </main>
  `,
  styles: [`
    mat-card {
      margin-top: 18px;
      border-radius: 8px;
    }
    mat-card-title {
      font-size: 28px;
    }
    dl {
      display: grid;
      grid-template-columns: 120px 1fr;
      gap: 12px 20px;
    }
    dt {
      color: #657287;
      font-weight: 600;
    }
    dd {
      margin: 0;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BookDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly books = inject(BookService);
  readonly book = toSignal(this.route.paramMap.pipe(switchMap((params) => this.books.getById(params.get('id') ?? ''))));
}
