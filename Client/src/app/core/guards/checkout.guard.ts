import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../services/cart.service';
import { ToastrService } from 'ngx-toastr';
import { firstValueFrom } from 'rxjs';

export const checkoutGuard: CanActivateFn = async (route, state) => {
  const cartService = inject(CartService);
  const router = inject(Router);
  const toastr = inject(ToastrService);

  // Nếu chưa có cart hoặc cart rỗng => thử load giỏ hàng
  if (
    cartService.cart().id === '' ||
    cartService.cart().cartItems.length === 0
  ) {
    try {
      await firstValueFrom(cartService.getCart());
    } catch (err) {
      console.error('Failed to load cart:', err);
      toastr.error('Không thể tải giỏ hàng. Vui lòng thử lại sau.');
      router.navigateByUrl('/cart');
      return false;
    }
  }
  const cart = cartService.cart();
  if (!cart || cart.cartItems.length === 0) {
    toastr.error(
      'Giỏ hàng bạn đang trống, vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán!'
    );
    router.navigateByUrl('/cart');
    return false;
  }
  return true;
};
