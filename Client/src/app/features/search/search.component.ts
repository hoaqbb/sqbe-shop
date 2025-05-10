import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { PaginatorModule } from 'primeng/paginator';
import { ProductItemComponent } from '../products/product-item/product-item.component';
import { Product } from '../../shared/models/product';
import { ShopService } from '../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, debounceTime, distinctUntilChanged, of, startWith, switchMap } from 'rxjs';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ProductSearchParams } from '../../shared/models/productParams';
import { Pagination } from '../../shared/models/pagination';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ProductItemComponent, PaginatorModule],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css'
})
export class SearchComponent implements OnInit{
  keyword: string;
  params: ProductSearchParams;
  pagination: Pagination<Product>;
  searchForm = new FormGroup ({
    keyword: new FormControl('')
  })

  constructor(private shopService: ShopService, private route: ActivatedRoute,   private toastr: ToastrService) {
    this.params = new ProductSearchParams();
  }
  
  ngOnInit(): void {
    this.trackChangesInput();
    this.getKeywordFromParam();
  }

  getKeywordFromParam() {
    this.route.queryParams.subscribe(param => {
      this.keyword = param['keyword']
      if(this.keyword) {
        this.searchForm.controls['keyword'].setValue(this.keyword);
      }
      //remove query param
      window.history.replaceState({}, document.title, window.location.pathname);
    })
  }

  trackChangesInput() {
    this.searchForm.controls['keyword'].valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((term) => {
        this.resetPagination();
        return this.searchProductByKeyword();
      }),
      catchError(err => {    
        this.toastr.show(err);  
        return of({ result: [], pagination: null });  
      }))
      .subscribe((result: any) => {
        this.pagination = result
     });
  }

  searchProductByKeyword() {
    this.keyword = this.searchForm.controls['keyword'].value;
    return this.shopService.searchProduct(this.keyword, this.params);
  }

  resetPagination() {
    this.params = new ProductSearchParams();
    this.pagination = null;
  }

  pageChanged(event: any) {
    this.params.pageNumber = event.page + 1;
    this.searchProductByKeyword().subscribe({
      next: (result: any) => {
        this.pagination = result;
      },
      error: (err) => {
        console.error('Error fetching products:', err);
      }
    });
  }
}
