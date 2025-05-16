import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-checkout-result',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './checkout-result.component.html',
  styleUrl: './checkout-result.component.css'
})
export class CheckoutResultComponent implements OnInit{

  queryParams = new HttpParams();

  constructor(
    private route: ActivatedRoute, 
    private orderService: OrderService, 
    private cartService: CartService
  ) { }

  isCheckoutSuccess: boolean;

  ngOnInit(): void {
    this.route.queryParams.subscribe((params: HttpParams) => {
      if (params['gateway'] === 'vnpay') {
        this.handleVnPayReturn(params);
      } else if (params['gateway'] === 'paypal') {
        this.handlePayPalReturn(params);
      } else if (params['gateway'] === 'cod') {
        this.isCheckoutSuccess = true;
      } else {
        this.handleUnknownPayment();
      }
      this.clearQueryParams()
    });
  }
  
  handleVnPayReturn(params: any) {
    let httpParams = this.getHttpParams(params);
    return this.orderService.vnpayCallback(httpParams).subscribe({
      next: (response: any) => {
        this.isCheckoutSuccess = true;
      },
      error: (error: any) => {
        console.log(error);
        this.isCheckoutSuccess = false;
      }
    });
  }

  handlePayPalReturn(params: any) {
    let httpParams = this.getHttpParams(params);
    return this.orderService.paypalCallback(httpParams).subscribe({
      next: (response: any) => {
        this.isCheckoutSuccess = true;
      },
      error: (error: any) => {
        console.log(error);
        this.isCheckoutSuccess = false;
      }
    });
  }
  
  getHttpParams(params: any) {
    let httpParams = new HttpParams();
    Object.keys(params).forEach(key => {
      httpParams = httpParams.append(key, params[key]);
    });
    return httpParams;
  }

  handleUnknownPayment() {
    this.isCheckoutSuccess = false;
    console.warn('Không xác định được cổng thanh toán!');
  }

  clearQueryParams() {
    window.history.replaceState({}, document.title, window.location.pathname);
  }
}

