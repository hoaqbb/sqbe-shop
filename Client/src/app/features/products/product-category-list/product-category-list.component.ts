import { Component, effect, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ShopService } from '../../../core/services/shop.service';
import { ProductFilterParams } from '../../../shared/models/productParams';
import { Pagination } from '../../../shared/models/pagination';
import { Product } from '../../../shared/models/product';
import { PaginatorModule } from 'primeng/paginator';
import { ProductItemComponent } from '../product-item/product-item.component';
import { FilterSidebarComponent } from '../filter-sidebar/filter-sidebar.component';
import { SidebarService } from '../../../core/services/sidebar.service';

@Component({
  selector: 'app-product-category-list',
  standalone: true,
  imports: [PaginatorModule, ProductItemComponent, FilterSidebarComponent],
  templateUrl: './product-category-list.component.html',
  styleUrl: './product-category-list.component.css',
})
export class ProductCategoryListComponent implements OnInit {
  param = new ProductFilterParams();
  paginatedProducts: Pagination<Product>;

  constructor(
    private route: ActivatedRoute,
    private shopService: ShopService,
    public sidebarService: SidebarService
  ) {
    this.fetchProductsOnFilterChange();
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const categorySlug = params.get('categorySlug');

      if(categorySlug !== this.shopService.currentCategory()) {
        this.shopService.currentCategory.set(categorySlug);
        this.shopService.resetProductFilterParams();
      }
    });
  }

  fetchProductsOnFilterChange() {
    effect(() => {
      const filters = this.shopService.productFilterParams();
      this.getProductsByFilter(filters);
    });
  }

  getProductsByFilter(filter: ProductFilterParams) {
    this.shopService.getProducts(filter).subscribe({
      next: (res: Pagination<Product>) => {
        this.paginatedProducts = res;
      }
    });
  }

  pageChanged(event: any) {
    const currentParams = this.shopService.productFilterParams();
    currentParams.pageNumber = event.page + 1;
    this.shopService.setProductFilterParams(currentParams);
    this.getProductsByFilter(currentParams);
  }
}
