import { Component, Input } from '@angular/core';
import { BadgeModule } from 'primeng/badge';
import { CartItem } from '../../../shared/models/cart';
import { DiscountPipe } from '../../../shared/pipes/discount.pipe';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-checkout-item',
  standalone: true,
  imports: [CommonModule, BadgeModule, DiscountPipe],
  templateUrl: './checkout-item.component.html',
  styleUrl: './checkout-item.component.css'
})
export class CheckoutItemComponent {
  @Input() item: CartItem;
}
