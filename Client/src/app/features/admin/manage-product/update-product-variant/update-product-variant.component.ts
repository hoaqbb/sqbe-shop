import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../../core/services/admin.service';
import { ToastrService } from 'ngx-toastr';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Product, ProductDetail, ProductVariant } from '../../../../shared/models/product';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-update-product-variant',
  standalone: true,
  imports: [CommonModule, TableModule, InputTextModule, InputNumberModule, FormsModule],
  templateUrl: './update-product-variant.component.html',
  styleUrl: './update-product-variant.component.css'
})
export class UpdateProductVariantComponent implements OnInit {
  product: Product;
  productVariants: ProductVariant[] = []

  constructor(
      public adminService: AdminService,
      public toastr: ToastrService,
      private ref: DynamicDialogRef,
      public config: DynamicDialogConfig,
      ) { }

  ngOnInit(): void {
    this.product = this.config.data;
    this.getProductVariants(this.product.id);
  }

  getProductVariants(id) {
    this.adminService.getProductVariants(id).subscribe({
      next: (res: ProductVariant[]) => {
        this.productVariants = res;
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  onEdit(event, quantityId) {
    this.productVariants.find(x => x.id == quantityId).quantity = event.target.value;
  }

  updateProductVariantQuantity() {
    this.adminService.updateProductVariantQuantity(this.product.id, this.productVariants).subscribe({
      next: () => {
          this.ref.destroy();
          this.toastr.success('Cập nhật sản phẩm trong kho thành công!')
        },
        error: (error) => {
          this.ref.destroy();
          this.toastr.warning(error)
        }
    });
  }
}
