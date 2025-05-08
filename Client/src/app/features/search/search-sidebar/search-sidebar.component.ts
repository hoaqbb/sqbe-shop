import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { SidebarModule } from 'primeng/sidebar';
import { Product } from '../../../shared/models/product';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ShopService } from '../../../core/services/shop.service';
import { ToastrService } from 'ngx-toastr';
import { Router, RouterLink } from '@angular/router';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  of,
  startWith,
  switchMap,
} from 'rxjs';
import { SidebarService } from '../../../core/services/sidebar.service';
import { ProductSearchParams } from '../../../shared/models/productParams';
import { Pagination } from '../../../shared/models/pagination';
import { DiscountPipe } from '../../../shared/pipes/discount.pipe';

@Component({
  selector: 'app-search-sidebar',
  standalone: true,
  imports: [CommonModule, SidebarModule, ReactiveFormsModule, DiscountPipe, RouterLink],
  templateUrl: './search-sidebar.component.html',
  styleUrl: './search-sidebar.component.css',
})
export class SearchSidebarComponent {
  products: Product[];
  params: ProductSearchParams;
  pagination: Pagination<Product>;
  searchForm = new FormGroup({
    search: new FormControl(''),
  });

  constructor(
    private shopService: ShopService,
    public sidebarService: SidebarService,
    private toastr: ToastrService,
    private router: Router
  ) {
    this.params = new ProductSearchParams();
    this.searchForm.controls['search'].valueChanges
      .pipe(
        startWith(''),
        debounceTime(300),
        distinctUntilChanged(),
        switchMap(term => this.shopService.searchProduct(term, this.params)),
        catchError((err) => {
          this.toastr.show(err);
          return of([]);
        })
      )
      .subscribe((data: any) => {
        this.pagination = data
        this.products = this.pagination.data;
      });
  }

  get show() {
    return this.sidebarService.isOpen('search');
  }

  set show(value: boolean) {
    if (value) {
      this.sidebarService.open('search');
    } else {
      this.sidebarService.close('search');
    }
  }

  navigateToSearch(key: string) {
    this.router.navigate(['/search'], { queryParams: { keyword: key } });
  }
}
