import { Component } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CartItemComponent } from './cart-item/cart-item.component';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, CartItemComponent, RouterLink],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent {
  constructor(
    public cartService: CartService,
    private router: Router
  ) { }

  checkout() {
    this.router.navigateByUrl('/checkout');
  }
}
