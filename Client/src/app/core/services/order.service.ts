import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { OrderRequest } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createOrder(orderReqest: OrderRequest) {
    return this.http.post(this.baseUrl + '/api/Orders', orderReqest)
  }

  vnpayCallback(queryParams: HttpParams) {
    return this.http.get(this.baseUrl + '/api/Payments/vnpay-callback?', { params: queryParams });
  }

  paypalCallback(queryParams: HttpParams) {
      return this.http.get(this.baseUrl + '/api/Payments/paypal-callback?', { params: queryParams });
  }

  applyDiscount(code: string, subtotal: number) {
    return this.http.get(this.baseUrl + '/api/Promotions/'+ code + '/apply?subtotal=' + subtotal);
  }
}
