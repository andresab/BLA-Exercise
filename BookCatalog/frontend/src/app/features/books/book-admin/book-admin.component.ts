import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { filter, switchMap } from 'rxjs';
import { Book } from '../../../core/models/book.model';
import { BookService } from '../../../core/services/book.service';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog.component';

@Component({
  selector: 'app-book-admin',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSnackBarModule,
    MatTableModule
  ],
  template: `
    <main class="page">
      <section class="admin-layout">
        <form class="editor form-grid" [formGroup]="form" (ngSubmit)="save()">
          <h1>{{ editingId() ? 'Edit book' : 'Create book' }}</h1>
          <mat-form-field appearance="outline">
            <mat-label>Title</mat-label>
            <input matInput formControlName="title" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Author</mat-label>
            <input matInput formControlName="author" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>ISBN</mat-label>
            <input matInput formControlName="isbn" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Published year</mat-label>
            <input matInput type="number" formControlName="publishedYear" />
          </mat-form-field>
          <div class="actions">
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">Save</button>
            <button mat-button type="button" (click)="reset()">Clear</button>
          </div>
        </form>

        <div class="table-wrap">
          <table mat-table [dataSource]="books()">
            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>Title</th>
              <td mat-cell *matCellDef="let book">{{ book.title }}</td>
            </ng-container>
            <ng-container matColumnDef="author">
              <th mat-header-cell *matHeaderCellDef>Author</th>
              <td mat-cell *matCellDef="let book">{{ book.author }}</td>
            </ng-container>
            <ng-container matColumnDef="year">
              <th mat-header-cell *matHeaderCellDef>Year</th>
              <td mat-cell *matCellDef="let book">{{ book.publishedYear }}</td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let book">
                <button mat-icon-button type="button" aria-label="Edit" (click)="edit(book)">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button type="button" aria-label="Delete" (click)="delete(book)">
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="columns"></tr>
            <tr mat-row *matRowDef="let row; columns: columns;"></tr>
          </table>
        </div>
      </section>
    </main>
  `,
  styles: [`
    .admin-layout {
      display: grid;
      grid-template-columns: 360px minmax(0, 1fr);
      gap: 24px;
      align-items: start;
    }
    .editor {
      padding: 18px;
      border: 1px solid #dfe4ee;
      border-radius: 8px;
      background: #ffffff;
    }
    h1 {
      margin: 0;
      font-size: 24px;
    }
    .actions {
      display: flex;
      gap: 8px;
    }
    .table-wrap {
      overflow: auto;
      border: 1px solid #dfe4ee;
      border-radius: 8px;
      background: #ffffff;
    }
    table {
      width: 100%;
      min-width: 680px;
    }
    @media (max-width: 920px) {
      .admin-layout {
        grid-template-columns: 1fr;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BookAdminComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly booksService = inject(BookService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  readonly books = signal<Book[]>([]);
  readonly editingId = signal<string | null>(null);
  readonly columns = ['title', 'author', 'year', 'actions'];
  readonly currentYear = computed(() => new Date().getUTCFullYear());
  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    author: ['', [Validators.required, Validators.maxLength(150)]],
    isbn: ['', [Validators.required, Validators.pattern(/^\d{9}[\dX]$|^\d{13}$/)]],
    publishedYear: [2000, [Validators.required, Validators.min(1450)]]
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.booksService.getAll().subscribe((books) => this.books.set(books));
  }

  edit(book: Book): void {
    this.editingId.set(book.id);
    this.form.setValue({
      title: book.title,
      author: book.author,
      isbn: book.isbn,
      publishedYear: book.publishedYear
    });
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }

    const value = this.form.getRawValue();
    const id = this.editingId();
    const request = id ? this.booksService.update(id, value) : this.booksService.create(value);
    request.subscribe({
      next: () => {
        this.reset();
        this.load();
        this.snackBar.open('Book saved', 'Close', { duration: 2500 });
      },
      error: () => this.snackBar.open('Save failed', 'Close', { duration: 3500 })
    });
  }

  delete(book: Book): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Delete book', message: `Delete ${book.title}?` }
    }).afterClosed().pipe(
      filter(Boolean),
      switchMap(() => this.booksService.delete(book.id))
    ).subscribe({
      next: () => {
        this.load();
        this.snackBar.open('Book deleted', 'Close', { duration: 2500 });
      },
      error: () => this.snackBar.open('Delete failed', 'Close', { duration: 3500 })
    });
  }

  reset(): void {
    this.editingId.set(null);
    this.form.reset({ title: '', author: '', isbn: '', publishedYear: 2000 });
  }
}
