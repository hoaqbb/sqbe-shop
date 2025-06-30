import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { TagModule } from 'primeng/tag';
import { AccountService } from '../../core/services/account.service';
import { OrderService } from '../../core/services/order.service';
import { MenuItem } from 'primeng/api';
import { OrderParams } from '../../shared/models/orderParams';
import { Pagination } from '../../shared/models/pagination';
import { Order } from '../../shared/models/order';
import { TabViewModule } from 'primeng/tabview';
import { Product } from '../../shared/models/product';
import { ProductItemComponent } from '../products/product-item/product-item.component';
import { User } from '../../shared/models/user';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [
    RouterLink,
    TableModule,
    CommonModule,
    PaginatorModule,
    TagModule,
    TabViewModule,
    ProductItemComponent,
  ],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css',
})
export class AccountComponent implements OnInit {
  orderParams: OrderParams = new OrderParams();
  paginatedResult: Pagination<Order>;

  items: MenuItem[] | undefined;
  activeItem: MenuItem | undefined;

  likedProduct: Product[] = [];
  userProfile: User;

  constructor(
    public accountService: AccountService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.onTabChange(0);
    this.userProfile = this.accountService.currentUser();
  }

  onTabChange(value) {
    switch (value) {
      case 0:
        this.loadUserOrders();
        break;
      case 1:
        this.loadFavoriteProducts();
        break;
    }
  }

  loadUserOrders() {
    this.orderService.getUserOrders(this.orderParams).subscribe((res: any) => {
      this.paginatedResult = res;
    });
  }

  loadFavoriteProducts() {
    this.accountService.getFavoriteProducts().subscribe({
      next: (res: any) => (this.likedProduct = res),
      error: (err) => console.log(err),
    });
  }

  logout() {
    this.accountService.logout().subscribe();
    this.router.navigateByUrl('/');
  }

  pageChanged(event: any) {
    this.orderParams.pageNumber = event.page + 1;
    this.loadUserOrders();
  }
}
