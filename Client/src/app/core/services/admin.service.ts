import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { AdminProductFilterParams } from '../../shared/models/adminParams';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;
  adminProductFilterParams = signal<AdminProductFilterParams>(
    new AdminProductFilterParams()
  );

  constructor(private http: HttpClient) {}

  getProducts(productParams: AdminProductFilterParams) {
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

    if (productParams.visible != null) {
      params = params.append('visible', productParams.visible);
    }

    params = params.append('pageSize', productParams.pageSize);
    params = params.append('pageIndex', productParams.pageNumber);
    return this.http.get(this.baseUrl + '/api/Admins/products', { params });
  }

  resetAdminProductFilterParams() {
    const params = new AdminProductFilterParams();
    this.adminProductFilterParams.set(params);
  }

  setAdminProductFilterParams(params: AdminProductFilterParams) {
    this.adminProductFilterParams.set(params);
  }
}
