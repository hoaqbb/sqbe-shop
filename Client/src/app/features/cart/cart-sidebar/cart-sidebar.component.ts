import { Component } from '@angular/core';
import { SidebarModule } from 'primeng/sidebar';
import { SidebarService } from '../../../core/services/sidebar.service';
import { RouterLink } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';
import { CommonModule } from '@angular/common';
import { DiscountPipe } from '../../../shared/pipes/discount.pipe';

@Component({
  selector: 'app-cart-sidebar',
  standalone: true,
  imports: [SidebarModule, RouterLink, CommonModule, DiscountPipe],
  templateUrl: './cart-sidebar.component.html',
  styleUrl: './cart-sidebar.component.css',
})
export class CartSidebarComponent {

  constructor(public sidebarService: SidebarService, public cartService: CartService) {}
  
  get show() {
    return this.sidebarService.isOpen('cart');
  }

  set show(value: boolean) {
    if (value) {
      this.sidebarService.open('cart');
    } else {
      this.sidebarService.close('cart');
    }
  }

  removeCartItem(cartItemId: number) {
    this.cartService.removeCartItem(cartItemId).subscribe();
  }

  checkout() {
    this.sidebarService.closeAll();
  }
}
