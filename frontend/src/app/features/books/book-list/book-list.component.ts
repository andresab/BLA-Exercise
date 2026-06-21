import { httpResource } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Book } from '../../../core/models/book.model';
import { BookCardComponent } from '../../../shared/components/book-card.component';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [MatFormFieldModule, MatIconModule, MatInputModule, BookCardComponent],
  template: `
    <main class="page">
      <section class="header">
        <div>
          <h1>Books</h1>
          <p>{{ filteredBooks().length }} titles</p>
        </div>
        <mat-form-field appearance="outline">
          <mat-label>Search</mat-label>
          <mat-icon matPrefix>search</mat-icon>
          <input matInput [value]="query()" (input)="query.set($any($event.target).value)" />
        </mat-form-field>
      </section>

      <section class="grid" [class.compact]="isHandset()">
        @for (book of filteredBooks(); track book.id) {
          <app-book-card [book]="book" />
        }
      </section>
    </main>
  `,
  styles: [`
    .header {
      display: flex;
      gap: 24px;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 24px;
    }
    h1 {
      margin: 0;
      font-size: 34px;
    }
    p {
      margin: 6px 0 0;
      color: #657287;
    }
    mat-form-field {
      width: min(360px, 100%);
    }
    .grid {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: 16px;
    }
    .grid.compact {
      grid-template-columns: 1fr;
    }
    @media (max-width: 820px) {
      .header {
        align-items: stretch;
        flex-direction: column;
      }
      .grid {
        grid-template-columns: 1fr;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BookListComponent {
  private readonly breakpointObserver = inject(BreakpointObserver);
  readonly booksResource = httpResource<Book[]>(() => `${environment.apiUrl}/books`, { defaultValue: [] });
  readonly query = signal('');
  readonly isHandset = toSignal(
    this.breakpointObserver.observe([Breakpoints.Handset]).pipe(map((state) => state.matches)),
    { initialValue: false }
  );
  readonly filteredBooks = computed(() => {
    const q = this.query().trim().toLowerCase();
    return this.booksResource.value().filter((book) =>
      !q || book.title.toLowerCase().includes(q) || book.author.toLowerCase().includes(q));
  });
}
