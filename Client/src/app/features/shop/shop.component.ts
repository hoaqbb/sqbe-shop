import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../layout/header/header.component';
import { FooterComponent } from '../layout/footer/footer.component';
import { CartSidebarComponent } from '../cart/cart-sidebar/cart-sidebar.component';
import { SearchSidebarComponent } from '../search/search-sidebar/search-sidebar.component';
import { CartService } from '../../core/services/cart.service';
import { ShopService } from '../../core/services/shop.service';

@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, CartSidebarComponent, SearchSidebarComponent],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.css'
})
export class ShopComponent implements OnInit{

  constructor(private cartService: CartService, private shopService: ShopService) { }

  ngOnInit(): void {
    this.initShop();
  }

  initShop() {
    this.cartService.getCart().subscribe();
    this.shopService.getItemsLastSee();
  }
}
