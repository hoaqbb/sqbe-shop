import { Component, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { ProductItemComponent } from '../products/product-item/product-item.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ProductItemComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{
  products: Product[] = [];

  constructor(private shopService: ShopService) { }
  
  ngOnInit(): void {
    this.getProducts();
  }

  getProducts() {
    this.shopService.getProducts().subscribe({
      next: (res) => this.products = res,
      error: (err) => console.log(err)
    });
  }
}
