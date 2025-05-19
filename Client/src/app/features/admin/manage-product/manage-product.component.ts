import { Component, effect } from '@angular/core';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CreateProductComponent } from './create-product/create-product.component';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule } from 'primeng/paginator';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../core/services/admin.service';
import { AdminProductFilterParams } from '../../../shared/models/adminParams';
import { Pagination } from '../../../shared/models/pagination';
import { Product, ProductDetail } from '../../../shared/models/product';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { FilterSidebarComponent } from '../../products/filter-sidebar/filter-sidebar.component';
import { SidebarService } from '../../../core/services/sidebar.service';

@Component({
  selector: 'app-manage-product',
  standalone: true,
  imports: [
    TableModule,
    ButtonModule,
    CommonModule,
    PaginatorModule,
    ConfirmDialogModule,
    FilterSidebarComponent,
  ],
  templateUrl: './manage-product.component.html',
  styleUrl: './manage-product.component.css',
  providers: [DialogService, ConfirmationService],
})
export class ManageProductComponent {
  ref: DynamicDialogRef | undefined;

  product: ProductDetail;
  adminParams = new AdminProductFilterParams();
  pagination: Pagination<Product>;

  constructor(
    public dialogService: DialogService,
    private adminService: AdminService,
    public sidebarService: SidebarService
  ) {
    this.fetchProductsOnFilterChange();
  }

  fetchProductsOnFilterChange() {
    effect(() => {
      const filters = this.adminService.adminProductFilterParams();
      this.loadProducts(filters);
    });
  }

  loadProducts(filter) {
    this.adminService
      .getProducts(filter)
      .subscribe((response: any) => (this.pagination = response));
  }

  pageChanged(event: any) {
    this.adminParams.pageNumber = event.page + 1;
    const currentParams = this.adminService.adminProductFilterParams();
    currentParams.pageNumber = event.page + 1;
    this.loadProducts(currentParams);
  }
}
