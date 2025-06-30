import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import { OrderDetail } from '../../../shared/models/order';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { StepsModule } from 'primeng/steps';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, TableModule, RouterLink, StepsModule],
  templateUrl: './order-detail.component.html',
  styleUrl: './order-detail.component.css',
})
export class OrderDetailComponent implements OnInit {
  orderDetail: OrderDetail;
  items: MenuItem[] | undefined;

  constructor(
    private route: ActivatedRoute,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    this.loadOrderDetail();
    this.items = [
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
  }


  loadOrderDetail() {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      this.orderService.getOrderById(id).subscribe({
        next: (res: any) => {
          this.orderDetail = res;
        },
        error: (err: any) => console.log(err),
      });
    }
  }
}
