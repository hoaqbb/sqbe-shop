import { Component, Input } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { Color } from '../../../shared/models/color';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-item.component.html',
  styleUrl: './product-item.component.css'
})
export class ProductItemComponent {
  @Input() product!: Product;
  

  constructor() { }
}
