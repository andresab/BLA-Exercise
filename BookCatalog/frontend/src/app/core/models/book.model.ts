export interface Book {
  id: string;
  title: string;
  author: string;
  isbn: string;
  publishedYear: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBook {
  title: string;
  author: string;
  isbn: string;
  publishedYear: number;
}

export type UpdateBook = Partial<CreateBook>;
