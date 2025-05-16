import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../services/cart.service';
import { ToastrService } from 'ngx-toastr';

export const checkoutGuard: CanActivateFn = (route, state) => {
  const cartService = inject(CartService);
  const router = inject(Router);
  const toastr = inject(ToastrService);
  
  if (!cartService.cart() || cartService.cart()?.cartItems.length === 0) {
    toastr.error('Giỏ hàng bạn đang trống, vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.');
    router.navigateByUrl('/cart');
    return false;
  } 
  return true;
};
