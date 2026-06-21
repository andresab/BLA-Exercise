import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Book, CreateBook, UpdateBook } from '../models/book.model';

@Injectable({ providedIn: 'root' })
export class BookService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/books`;

  getAll() {
    return this.http.get<Book[]>(this.baseUrl);
  }

  getById(id: string) {
    return this.http.get<Book>(`${this.baseUrl}/${id}`);
  }

  create(book: CreateBook) {
    return this.http.post<Book>(this.baseUrl, book);
  }

  update(id: string, book: UpdateBook) {
    return this.http.put<Book>(`${this.baseUrl}/${id}`, book);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
