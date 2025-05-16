import { Component, effect, OnInit, signal } from '@angular/core';
import { SidebarService } from '../../../core/services/sidebar.service';
import { SidebarModule } from 'primeng/sidebar';
import { CommonModule } from '@angular/common';
import { ShopService } from '../../../core/services/shop.service';
import { InputSwitchModule } from 'primeng/inputswitch';
import { SliderModule } from 'primeng/slider';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { ProductFilterParams } from '../../../shared/models/productParams';
import { ColorCheckboxGroupComponent } from '../../../shared/components/color-checkbox-group/color-checkbox-group.component';
import { SizeCheckboxGroupComponent } from '../../../shared/components/size-checkbox-group/size-checkbox-group.component';

@Component({
  selector: 'app-filter-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    SidebarModule,
    InputSwitchModule,
    SliderModule,
    FormsModule,
    ReactiveFormsModule,
    ColorCheckboxGroupComponent,
    SizeCheckboxGroupComponent,
  ],
  templateUrl: './filter-sidebar.component.html',
  styleUrl: './filter-sidebar.component.css',
})
export class FilterSidebarComponent implements OnInit {
  filterForm: FormGroup;
  priceRangeValues: number[] = [0, 100];
  filterParams = new ProductFilterParams();

  constructor(
    public sidebarService: SidebarService,
    public shopService: ShopService,
    private fb: FormBuilder
  ) {
    this.resetFilterOnCategoryChange();
  }

  ngOnInit(): void {
    this.initFilterForm();
  }

  resetFilterOnCategoryChange() {
    effect(
      () => {
        const category = this.shopService.currentCategory();
        if (category) {
          this.resetFilter();
        }
      },
      {
        allowSignalWrites: true,
      }
    );
  }

  get show() {
    return this.sidebarService.isOpen('filter');
  }

  set show(value: boolean) {
    if (value) {
      this.sidebarService.open('filter');
    } else {
      this.sidebarService.close('filter');
    }
  }

  initFilterForm() {
    this.filterForm = this.fb.group({
      sort: ['dateDesc'],
      colors: this.fb.control([]),
      sizes: this.fb.control([]),
      promotion: [false],
    });
  }

  applyFilter() {
    let filterParams = this.filterForm.value;

    // Update price values from slider before sending
    if (this.priceRangeValues[0] != 0) {
      filterParams = {
        ...filterParams,
        priceFrom: this.priceRangeValues[0] * 10000,
      };
    }
    if (this.priceRangeValues[1] != 100) {
      filterParams = {
        ...filterParams,
        priceTo: this.priceRangeValues[1] * 10000,
      };
    }

    this.getFilteredProducts(filterParams);
    this.sidebarService.closeAll();
  }

  resetFilter() {
    this.shopService.resetProductFilterParams();
    this.resetFilterForm();
  }

  private resetFilterForm() {
    this.filterForm?.reset({
      sort: 'dateDesc',
      colors: [],
      sizes: [],
      promotion: false,
    });
    this.priceRangeValues = [0, 100];
  }

  getFilteredProducts(formValues: any) {
    const currentCategory = this.shopService.currentCategory();

    this.filterParams = {
      ...new ProductFilterParams(),
      category: currentCategory,
      sort: formValues.sort,
      colors: formValues.colors,
      sizes: formValues.sizes,
      priceFrom: formValues.priceFrom,
      priceTo: formValues.priceTo,
      promotion: formValues.promotion,
    };

    this.shopService.setProductFilterParams(this.filterParams);
  }
}
