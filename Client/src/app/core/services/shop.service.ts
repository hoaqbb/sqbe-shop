import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Category } from '../../shared/models/category';
import { HttpClient, HttpParams } from '@angular/common/http';
import { of, tap } from 'rxjs';
import { Product, ProductDetail } from '../../shared/models/product';
import {
  ProductFilterParams,
  ProductSearchParams,
} from '../../shared/models/productParams';
import { Color } from '../../shared/models/color';
import { Size } from '../../shared/models/size';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = environment.apiUrl;
  categories: Category[] = [];
  colors: Color[] = [];
  sizes: Size[] = [];
  currentCategory = signal<string | null>(null);
  productFilterParams = signal<ProductFilterParams>(new ProductFilterParams());

  itemsLastSee: Product[] = [];

  products: Product[] = [];

  getRelatedProducts(currentProductSLug: string) {
    // Lọc bỏ sản phẩm hiện tại
    const filteredProducts = this.products.filter(p => p.slug !== currentProductSLug);

    const relatedProducts: Product[] = [];
    const availableProducts = [...filteredProducts];

    const numToSelect = Math.min(this.products.length - 1, 4);

    if (availableProducts.length <= 4) {
      return availableProducts;
    }

    // Lấy ngẫu nhiên 4 sản phẩm
    for (let i = 0; i < numToSelect; i++) {
      const randomIndex = Math.floor(Math.random() * availableProducts.length);
      relatedProducts.push(availableProducts[randomIndex]);
      availableProducts.splice(randomIndex, 1);
    }

    return relatedProducts;
  }

  getItemsLastSee() {
    const items = JSON.parse(localStorage.getItem('itemsLastSee'));
    if(items) {
      this.itemsLastSee = items;
    }
  }

  addItemLastSee(item: Product) {
    this.itemsLastSee = this.itemsLastSee.filter((p) => p.id !== item.id);
    this.itemsLastSee.unshift(item);
    if (this.itemsLastSee.length > 4) {
      this.itemsLastSee = this.itemsLastSee.slice(0, 4);
    }
    localStorage.setItem('itemsLastSee', JSON.stringify(this.itemsLastSee));
  }

  resetProductFilterParams() {
    const params = new ProductFilterParams();
    params.category = this.currentCategory();
    this.productFilterParams.set(params);
  }

  setProductFilterParams(params: ProductFilterParams) {
    params.category = this.currentCategory();
    this.productFilterParams.set(params);
  }

  constructor(private http: HttpClient) {}

  getCategories() {
    return this.http
      .get<Category[]>(this.baseUrl + '/api/Categories')
      .pipe(tap((res: Category[]) => (this.categories = res)));
  }

  getColors() {
    return this.http
      .get<Color[]>(this.baseUrl + '/api/Colors')
      .pipe(tap((res) => (this.colors = res)));
  }

  getSizes() {
    return this.http
      .get<Size[]>(this.baseUrl + '/api/Sizes')
      .pipe(tap((res) => (this.sizes = res)));
  }

  getProducts(productParams: ProductFilterParams) {
    let params = new HttpParams();

    if (productParams.category && productParams.category !== 'all') {
      params = params.append('category', productParams.category);
    }

    if (productParams.colors?.length > 0) {
      productParams.colors.forEach((element) => {
        params = params.append('colors', element);
      });
    }

    if (productParams.sizes?.length > 0) {
      productParams.sizes.forEach((element) => {
        params = params.append('sizes', element);
      });
    }

    if (productParams.priceFrom) {
      params = params.append('priceFrom', productParams.priceFrom);
    }

    if (productParams.priceTo) {
      params = params.append('priceTo', productParams.priceTo);
    }

    if (productParams.promotion) {
      params = params.append('promotion', productParams.promotion);
    }

    if (productParams.sort) {
      params = params.append('sort', productParams.sort);
    }

    params = params.append('pageSize', productParams.pageSize);
    params = params.append('pageIndex', productParams.pageNumber);

    return this.http.get(this.baseUrl + '/api/Products', {params}).pipe(
      tap((res: any) => {
        this.products = res.data;
      })
    );
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
    return this.http.get(this.baseUrl + '/api/Products/search', { params });
  }

  getBanners() {
    return this.http.get(this.baseUrl + '/api/Banners');
  }

  getBlogs() {
    return this.http.get(this.baseUrl + '/api/Blogs');
  }

  getBlogBySlug(slug) {
    return this.http.get(this.baseUrl + '/api/Blogs/' + slug);
  }
}
