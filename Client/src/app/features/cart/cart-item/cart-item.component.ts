import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { InputNumberModule } from 'primeng/inputnumber';
import { CartItem } from '../../../shared/models/cart';
import { CartService } from '../../../core/services/cart.service';
import { FormsModule } from '@angular/forms';
import { DiscountPipe } from '../../../shared/pipes/discount.pipe';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cart-item',
  standalone: true,
  imports: [CommonModule, InputNumberModule, RouterLink, FormsModule, DiscountPipe],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.css'
})
export class CartItemComponent {
  @Input() cartItem: CartItem;

  constructor(public cartService: CartService) { }

  updateCartItem(event, cartItemId) {
    const newQuantity = event.target.ariaValueNow;

    return this.cartService.updateCartItem(cartItemId, newQuantity).subscribe()
  }

  removeCartItem(cartItemId: number) {
    this.cartService.removeCartItem(cartItemId).subscribe();
  }
}
