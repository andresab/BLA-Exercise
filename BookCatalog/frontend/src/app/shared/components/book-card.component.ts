import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';
import { Book } from '../../core/models/book.model';

@Component({
  selector: 'app-book-card',
  standalone: true,
  imports: [MatCardModule, MatButtonModule, RouterLink],
  template: `
    <mat-card appearance="outlined">
      <mat-card-header>
        <mat-card-title>{{ book().title }}</mat-card-title>
        <mat-card-subtitle>{{ book().author }}</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <p>{{ book().publishedYear }} · {{ book().isbn }}</p>
      </mat-card-content>
      <mat-card-actions>
        <a mat-button [routerLink]="['/books', book().id]">Details</a>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [`
    mat-card {
      height: 100%;
      border-radius: 8px;
    }
    mat-card-title {
      font-size: 18px;
      line-height: 1.3;
    }
    p {
      margin: 16px 0 0;
      color: #546172;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BookCardComponent {
  readonly book = input.required<Book>();
}
