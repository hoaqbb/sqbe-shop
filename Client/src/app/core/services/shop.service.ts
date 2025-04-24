import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Category } from '../../shared/models/category';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = environment.apiUrl;
  categories = signal<Category[] | null>(null);

  constructor(private http: HttpClient) { }

  getCategories() {
    return this.http.get<Category[]>(this.baseUrl + '/api/Categories').pipe(
      tap((res: Category[]) => this.categories.set(res))
    );
  }
}
