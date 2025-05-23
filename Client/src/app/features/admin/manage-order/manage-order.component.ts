import { Component, effect } from '@angular/core';
import { AdminService } from '../../../core/services/admin.service';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { PaginatorModule } from 'primeng/paginator';
import { Order } from '../../../shared/models/order';
import { OrderFilterSidebarComponent } from "./order-filter-sidebar/order-filter-sidebar.component";
import { SidebarService } from '../../../core/services/sidebar.service';
import { Pagination } from '../../../shared/models/pagination';

@Component({
  selector: 'app-manage-order',
  standalone: true,
  imports: [TableModule, TagModule, CommonModule, PaginatorModule, OrderFilterSidebarComponent],
  templateUrl: './manage-order.component.html',
  styleUrl: './manage-order.component.css'
})
export class ManageOrderComponent  {
  paginatedOrders: Pagination<Order>;

  constructor(private adminService: AdminService, public sidebarService: SidebarService) {
    this.fetchOrdersOnFilterChange();
  }

  fetchOrdersOnFilterChange() {
      effect(() => {
        const filters = this.adminService.adminOrderFilterParams();
        this.getOrders(filters);
      });
    }

  getOrders(filter) {
    this.adminService.getOrders(filter).subscribe({
      next: (res: any) => {
        this.paginatedOrders = res;
      }
    })
  }

  pageChanged(event: any) {
    const currentParams = this.adminService.adminOrderFilterParams();
    currentParams.pageNumber = event.page + 1;
    this.getOrders(currentParams);
  }
}