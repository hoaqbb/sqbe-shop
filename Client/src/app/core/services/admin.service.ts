import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import {
  CreateProduct,
  ProductVariant,
  UpdateProduct,
} from '../../shared/models/product';
import { AdminProductFilterParams } from '../../shared/models/adminParams';
import { OrderFilterParams } from '../../shared/models/orderParams';
import {
  CreatePromotion,
  PromotionDetail,
  UpdatePromotion,
} from '../../shared/models/promotion';
import { CategoryDetail, CreateCategory } from '../../shared/models/category';
import { tap } from 'rxjs';
import { ColorDetail } from '../../shared/models/color';
import { SizeDetail } from '../../shared/models/size';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;
  adminProductFilterParams = signal<AdminProductFilterParams>(
    new AdminProductFilterParams()
  );
  adminOrderFilterParams = signal<OrderFilterParams>(new OrderFilterParams());
  categories: CategoryDetail[] = [];
  colors: ColorDetail[] = [];
  sizes: SizeDetail[] = [];
  promotions: PromotionDetail[] = [];

  constructor(private http: HttpClient) {}

  //#region product

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

  createNewProduct(newProduct: CreateProduct) {
    const formData = new FormData();

    formData.append('name', newProduct.name);
    formData.append('price', newProduct.price.toString());
    formData.append('description', newProduct.description);
    formData.append('discount', newProduct.discount.toString());
    formData.append('categoryId', newProduct.categoryId.toString());

    newProduct.productColors.forEach((colorId) => {
      formData.append(`productColors`, colorId.toString());
    });

    newProduct.productSizes.forEach((sizeId) => {
      formData.append(`productSizes`, sizeId.toString());
    });

    formData.append('mainImage', newProduct.mainImage);
    formData.append('subImage', newProduct.subImage);
    newProduct.productImages.forEach((file) => {
      formData.append(`productImages`, file);
    });

    return this.http.post(this.baseUrl + '/api/Products', formData);
  }

  deleteProductById(id: string) {
    return this.http.delete(this.baseUrl + '/api/Products/' + id);
  }

  updateProductStatusById(id: string) {
    return this.http.put(this.baseUrl + '/api/Products/' + id + '/status', {});
  }

  updateProduct(id: string, updateProduct: UpdateProduct) {
    return this.http.put(this.baseUrl + '/api/Products/' + id, updateProduct);
  }

  setMainImage(id: string, imageId: number) {
    return this.http.put(
      this.baseUrl +
        '/api/Products/' +
        id +
        '/set-main-image' +
        '?imageId=' +
        imageId,
      {}
    );
  }

  setSubImage(id: string, imageId: number) {
    return this.http.put(
      this.baseUrl +
        '/api/Products/' +
        id +
        '/set-sub-image' +
        '?imageId=' +
        imageId,
      {}
    );
  }

  deleteProductImage(id: string, imageId: number) {
    return this.http.delete(
      this.baseUrl + '/api/Products/' + id + '/delete-image/' + imageId
    );
  }

  addProductImages(id: string, imageFiles) {
    let formData: FormData = new FormData();
    imageFiles.forEach((file) => {
      formData.append('imageFiles', file);
    });

    return this.http.post(
      this.baseUrl + '/api/Products/' + id + '/add-product-images',
      formData
    );
  }

  resetAdminProductFilterParams() {
    const params = new AdminProductFilterParams();
    this.adminProductFilterParams.set(params);
  }

  setAdminProductFilterParams(params: AdminProductFilterParams) {
    this.adminProductFilterParams.set(params);
  }
  //#endregion product

  //#region order
  getOrders(orderParams: OrderFilterParams) {
    let params = new HttpParams();

    if (orderParams.sort) {
      params = params.append('sort', orderParams.sort);
    }

    if (orderParams.status != null) {
      params = params.append('status', orderParams.status);
    }

    if (orderParams.amountFrom) {
      params = params.append('amountFrom', orderParams.amountFrom);
    }

    if (orderParams.amountTo) {
      params = params.append('amountTo', orderParams.amountTo);
    }

    if (orderParams.isDiscounted != null) {
      params = params.append('isDiscounted', orderParams.isDiscounted);
    }

    if (orderParams.paymentMethod) {
      params = params.append('paymentMethod', orderParams.paymentMethod);
    }

    params = params.append('pageSize', orderParams.pageSize);
    params = params.append('pageIndex', orderParams.pageNumber);

    return this.http.get(this.baseUrl + '/api/Admins/orders', { params });
  }

  resetAdminOrderFilterParams() {
    const params = new OrderFilterParams();
    this.adminOrderFilterParams.set(params);
  }

  setAdminOrderFilterParams(params: OrderFilterParams) {
    this.adminOrderFilterParams.set(params);
  }

  //#endregion order

  //#region promotion
  getPromotions() {
    if(this.promotions.length > 0) return null;
    return this.http.get(this.baseUrl + '/api/Admins/promotions');
  }

  createPromotion(newPromotion: CreatePromotion) {
    return this.http.post(this.baseUrl + '/api/Promotions', newPromotion);
  }

  deletePromotion(id) {
    return this.http.delete(this.baseUrl + '/api/Promotions/' + id);
  }

  updatePromotion(id, updatedPromotion: UpdatePromotion) {
    return this.http.put(
      this.baseUrl + '/api/Promotions/' + id,
      updatedPromotion
    );
  }

  //#endregion promotion

  //#region category

  getCategories() {
    if (this.categories.length > 0) return null;

    return this.http.get(this.baseUrl + '/api/Admins/categories').pipe(
      tap((res: CategoryDetail[]) => {
        this.categories = res;
      })
    );
  }

  createCategory(newCategory: CreateCategory) {
    return this.http.post(this.baseUrl + '/api/Categories', newCategory);
  }

  updateCategory(updatedCategory: any) {
    return this.http.put(
      this.baseUrl + '/api/Categories/' + updatedCategory.id,
      { name: updatedCategory.name, slug: updatedCategory.slug }
    );
  }

  deleteCategory(id) {
    return this.http.delete(this.baseUrl + '/api/Categories/' + id);
  }

  //#endregion category

  //#region size

  getSizes() {
    return this.http.get(this.baseUrl + '/api/Admins/sizes').pipe(
      tap((res: SizeDetail[]) => {
        // this.sizes.set(res);
        this.sizes = res;
      })
    );
  }

  createSize(newSize: any) {
    return this.http.post(this.baseUrl + '/api/Sizes', { name: newSize.name });
  }

  updateSize(updatedSize: any) {
    return this.http.put(this.baseUrl + '/api/Sizes/' + updatedSize.id, { name: updatedSize.name });
  }

  deleteSize(id) {
    return this.http.delete(this.baseUrl + '/api/Sizes/' + id);
  }

  //#endregion size
}
