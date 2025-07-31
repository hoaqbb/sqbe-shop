import { Component, effect } from '@angular/core';
import { AdminService } from '../../../core/services/admin.service';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { PaginatorModule } from 'primeng/paginator';
import { Order, OrderDetail } from '../../../shared/models/order';
import { OrderFilterSidebarComponent } from './order-filter-sidebar/order-filter-sidebar.component';
import { SidebarService } from '../../../core/services/sidebar.service';
import { Pagination } from '../../../shared/models/pagination';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { UpdateOrderStatusComponent } from './update-order-status/update-order-status.component';

@Component({
  selector: 'app-manage-order',
  standalone: true,
  imports: [
    TableModule,
    TagModule,
    CommonModule,
    PaginatorModule,
    OrderFilterSidebarComponent,
  ],
  templateUrl: './manage-order.component.html',
  styleUrl: './manage-order.component.css',
  providers: [DialogService],
})
export class ManageOrderComponent {
  paginatedOrders: Pagination<Order>;
  ref: DynamicDialogRef | undefined;
  constructor(
    private adminService: AdminService,
    public sidebarService: SidebarService,
    // private ref: DynamicDialogRef,
    public dialogService: DialogService,
  ) {
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
      },
    });
  }

  showUpdateOrderStatusDialog(orderId: string) {
    this.adminService.getOrderById(orderId).subscribe((response: OrderDetail) => {
        this.ref = this.dialogService.open(UpdateOrderStatusComponent, {
          data: response,
          modal: true,
        });
      });

  }

  pageChanged(event: any) {
    const currentParams = this.adminService.adminOrderFilterParams();
    currentParams.pageNumber = event.page + 1;
    this.getOrders(currentParams);
  }
}