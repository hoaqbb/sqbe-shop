import { Component, Input } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { CommonModule } from '@angular/common';
import { DiscountPipe } from '../../../shared/pipes/discount.pipe';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-product-item',
  standalone: true,
  imports: [CommonModule, DiscountPipe, RouterLink],
  templateUrl: './product-item.component.html',
  styleUrl: './product-item.component.css'
})
export class ProductItemComponent {
  @Input() product!: Product;
  

  constructor() { }
}
