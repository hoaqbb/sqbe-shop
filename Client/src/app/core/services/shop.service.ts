import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Category } from '../../shared/models/category';
import { HttpClient, HttpParams } from '@angular/common/http';
import { of, tap } from 'rxjs';
import { Product, ProductDetail } from '../../shared/models/product';
import { ProductSearchParams } from '../../shared/models/productParams';

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

  getProducts() {
    return this.http.get<Product[]>(this.baseUrl + '/api/Products');
  }

  getProductBySlug(slug: string) {
    return this.http.get<ProductDetail>(this.baseUrl + '/api/Products/'+ slug);
  }

  searchProduct(keyword: string, productParams: ProductSearchParams) {
    let params = new HttpParams();

    productParams.keyword = keyword.trim();
    if (productParams.keyword === '') {
      return of();
    }

    params = params.append('keyword', productParams.keyword);
    params = params.append('pageSize', productParams.pageSize);
    params = params.append('pageIndex', productParams.pageNumber);
    return this.http.get(this.baseUrl + '/api/Products/search', {params});
  }
}
