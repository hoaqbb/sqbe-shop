import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../../core/services/admin.service';
import { ToastrService } from 'ngx-toastr';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { OrderDetail } from '../../../../shared/models/order';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { StepsModule } from 'primeng/steps';
import { MenuItem } from 'primeng/api';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-order-status',
  standalone: true,
  imports: [CommonModule, TableModule, StepsModule, FormsModule],
  templateUrl: './update-order-status.component.html',
  styleUrl: './update-order-status.component.css'
})
export class UpdateOrderStatusComponent implements OnInit{
  orderDetail: OrderDetail;
  items: MenuItem[] = [
      {
        label: 'Chờ xác nhận',
      },
      {
        label: 'Đã xác nhận',
      },
      {
        label: 'Đang giao hàng',
      },
      {
        label: 'Thành công',
      },
    ];
  orderStatuses = [
    { value: 1, label: "Đã xác nhận"},
    { value: 2, label: "Đang giao hàng"},
    { value: 3, label: "Giao hàng thành công"}
  ]

  status: number;

  constructor(
      public adminService: AdminService,
      public toastr: ToastrService,
      private ref: DynamicDialogRef,
      public config: DynamicDialogConfig
    ) {}

  ngOnInit(): void {
    this.orderDetail = this.config.data;
    this.status = this.orderDetail.status;
    console.log(this.status);
  }

  updateOrderStatus(orderId) {
    console.log(this.status);
    console.log(this.adminService.ordersWithFilter);
    
    this.adminService.updateOrderStatus(orderId,this.status).subscribe({
      next: (res) => {
        this.updateOrderLocally(orderId, this.status);
        this.toastr.success("Cập nhật trạng thái đơn hàng thành công.");
        this.ref.destroy();
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  cancleOrderStatus(orderId) {
    this.adminService.updateOrderStatus(orderId, 4).subscribe({
      next: (res) => {
        this.updateOrderLocally(orderId, 4);
        this.toastr.success("Hủy đơn hàng thành công.");
        this.ref.destroy();
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  updateOrderLocally(orderId, status) {
    let orderModifiedIndex = this.adminService.ordersWithFilter.data.findIndex(o => o.id == orderId);
    this.adminService.ordersWithFilter.data[orderModifiedIndex].status = status;
    this.adminService.ordersWithFilter.data[orderModifiedIndex].updateAt = new Date().toISOString();
  }
  
  getDeliveryMethodText(methodNumber: number): string {
    switch (methodNumber) {
      case 0:
        return 'Giao hàng tiêu chuẩn';
      case 1:
        return 'Giao hàng nhanh';
      default:
        return 'Không xác định';
    }
  }
}
