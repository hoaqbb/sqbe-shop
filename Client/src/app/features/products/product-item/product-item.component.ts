import { Component, Input } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { CommonModule } from '@angular/common';
import { DiscountPipe } from '../../../shared/pipes/discount.pipe';
import { RouterLink } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { ToastrService } from 'ngx-toastr';
import { ShopService } from '../../../core/services/shop.service';

@Component({
  selector: 'app-product-item',
  standalone: true,
  imports: [CommonModule, DiscountPipe, RouterLink],
  templateUrl: './product-item.component.html',
  styleUrl: './product-item.component.css',
})
export class ProductItemComponent {
  @Input() product!: Product;
  @Input() actionsOnHover: boolean = true;

  constructor(
    private accountService: AccountService,
    public shopService: ShopService,
    private toastr: ToastrService
  ) {}

  likeProduct(productId: string) {
    if (!this.accountService.currentUser()) {
      return this.toastr.show('Vui lòng đăng nhập để tiếp tục.');
    } else {
      return this.accountService.likeProduct(productId).subscribe({
        next: () => {
          this.product.isLikedByCurrentUser = true;
        },
        error: (error) => {
          this.toastr.error(error.error);
        },
      });
    }
  }

  unlikeProduct(productId: string) {
    this.accountService.unlikeProduct(productId).subscribe({
      next: () => {
        this.product.isLikedByCurrentUser = false;
      },
      error: (error) => {
        this.toastr.error(error.error);
      },
    });
  }
}
