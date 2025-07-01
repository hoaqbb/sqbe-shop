import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CartService } from '../../core/services/cart.service';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TextInputComponent } from '../../shared/components/text-input/text-input.component';
import { CommonModule } from '@angular/common';
import { BadgeModule } from 'primeng/badge';
import { AddressService } from '../../core/services/address.service';
import { CheckoutItemComponent } from './checkout-item/checkout-item.component';
import { ShopService } from '../../core/services/shop.service';
import { OrderRequest } from '../../shared/models/order';
import { OrderService } from '../../core/services/order.service';
import { AccountService } from '../../core/services/account.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [
    TextInputComponent,
    ReactiveFormsModule,
    CommonModule,
    BadgeModule,
    RouterLink,
    CheckoutItemComponent,
  ],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css',
})
export class CheckoutComponent implements OnInit {
  checkoutForm: FormGroup;
  orderRequest: OrderRequest;
  provinces: any;
  districts: any;
  wards: any;
  shippingFee = 0;
  amount = 0;
  discountAmount = 0;

  constructor(
    private formBuilder: FormBuilder,
    public cartService: CartService,
    private addressService: AddressService,
    private shopService: ShopService,
    private accountService: AccountService,
    private orderService: OrderService,
    private route: Router,
    private toarstr: ToastrService
  ) {}

  ngOnInit(): void {
    if (
      this.cartService.cart() &&
      this.cartService.cart().cartItems.length === 0
    ) {
      this.cartService.getCart().subscribe();
    }
    this.initializeCheckoutForm();
    this.getProvinces();
    this.amount = this.cartService.subtotal();
  }

  initializeCheckoutForm() {
    if(this.accountService.currentUser()) {
      let userInfo = this.accountService.currentUser();
      this.checkoutForm = this.formBuilder.group({
      fullname: [userInfo.lastname + ' ' + userInfo.firstname, Validators.required],
      email: [userInfo.email, [Validators.required, Validators.email]],
      phoneNumber: [
        '',
        [
          Validators.required,
          Validators.minLength(10),
          Validators.maxLength(10),
        ],
      ],
      street: ['', Validators.required],
      province: ['', Validators.required],
      district: ['', Validators.required],
      ward: ['', Validators.required],
      deliveryMethod: ['1', Validators.required],
      paymentMethod: ['cod', Validators.required],
      note: [''],
      promotionCode: [''],
    });
    } else {
      this.checkoutForm = this.formBuilder.group({
      fullname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [
        '',
        [
          Validators.required,
          Validators.minLength(10),
          Validators.maxLength(10),
        ],
      ],
      street: ['', Validators.required],
      province: ['', Validators.required],
      district: ['', Validators.required],
      ward: ['', Validators.required],
      deliveryMethod: ['1', Validators.required],
      paymentMethod: ['cod', Validators.required],
      note: [''],
      promotionCode: [''],
    });
    }
    
  }

  getProvinces() {
    return this.addressService.getProvinces().then((res: any) => {
      this.provinces = res.data;
    });
  }

  getDistricts(provinceId) {
    return this.addressService.getDistricts(provinceId).then((res: any) => {
      this.districts = res.data;
      this.wards = [];
    });
  }

  getWards(districtId) {
    return this.addressService.getWards(districtId).then((res: any) => {
      this.wards = res.data;
    });
  }

  calculateAmount() {
    if (this.cartService.subtotal() > 1500000) this.shippingFee = 0;
    this.amount = this.cartService.subtotal() + this.shippingFee;
    if (this.discountAmount > 0)
      this.amount = this.amount + this.discountAmount;

    return this.amount;
  }

  calculateShippingFee(provinceId: string) {
    if (this.cartService.subtotal() < 1500000) {
      //mien phi van chuyen cho don > 1tr5
      const specifiedProvince = [
        '79', //tp HCM
        '01', //Ha Noi
      ];

      if (specifiedProvince.includes(provinceId)) {
        this.shippingFee = 20000;
      } else {
        this.shippingFee = 30000;
      }
      for (let i = 0; i < this.cartService.cart().cartItems.length; i++) {
        if (this.cartService.cart().cartItems[i].category == 'bag') {
          this.shippingFee += 10000;
          break;
        }
      }
    }
    this.calculateAmount();
  }

  createOrder() {
    const wardId = this.checkoutForm.get('ward').value;
    return this.addressService.getFullAddress(wardId).then((res: any) => {
      this.orderRequest = {
        fullname: this.checkoutForm.get('fullname').value,
        email: this.checkoutForm.get('email').value,
        phoneNumber: this.checkoutForm.get('phoneNumber').value,
        address:
          this.checkoutForm.get('street').value + ', ' + res.data.full_name,
        paymentMethod: this.checkoutForm.get('paymentMethod').value,
        deliveryMethod: this.checkoutForm.get('deliveryMethod').value,
        shippingFee: this.shippingFee,
        amount: this.amount,
        subtotal: this.cartService.subtotal(),
        discountAmount: this.discountAmount,
        promotionCode: this.checkoutForm.get('promotionCode').value,
        note: this.checkoutForm.get('note').value,
      };

      this.orderService.createOrder(this.orderRequest).subscribe({
        next: (res: any) => {
          if (res && res.success) {
            if (res.redirectUrl) window.location.href = `${res.redirectUrl}`;
            else {
              this.route.navigateByUrl('/checkout/result?gateway=cod');
            }
          }
        },
        error: (err) => console.log(err),
      });
    });
  }

  discountCode: string;
  applyDiscount(code) {
    if (this.discountCode === code) return null;
    return this.orderService
      .applyDiscount(code, this.cartService.subtotal())
      .subscribe({
        next: (res: any) => {
          this.discountCode = code;
          this.discountAmount = res.discountAmount;
          this.amount = this.amount - this.discountAmount;
        },
        error: (err) => {
          this.discountCode = code;
          this.checkoutForm.get('promotionCode').setErrors({ incorrect: true });
          this.amount = this.amount + this.discountAmount;
          this.discountAmount = 0;
        },
      });
  }
}
