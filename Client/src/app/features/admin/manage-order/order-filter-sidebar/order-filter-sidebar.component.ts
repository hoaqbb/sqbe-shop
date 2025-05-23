import { Component, OnInit } from '@angular/core';
import { SidebarService } from '../../../../core/services/sidebar.service';
import { SidebarModule } from 'primeng/sidebar';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { AdminService } from '../../../../core/services/admin.service';
import { SliderModule } from 'primeng/slider';
import { OrderFilterParams } from '../../../../shared/models/orderParams';

@Component({
  selector: 'app-order-filter-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    SidebarModule,
    ReactiveFormsModule,
    FormsModule,
    SliderModule,
  ],
  templateUrl: './order-filter-sidebar.component.html',
  styleUrl: './order-filter-sidebar.component.css',
})
export class OrderFilterSidebarComponent implements OnInit {
  orderFilterForm: FormGroup;
  amountRangeValues: number[] = [0, 100];

  constructor(
    public sidebarService: SidebarService,
    private fb: FormBuilder,
    private adminService: AdminService
  ) { }

  ngOnInit(): void {
    this.initFilterForm();
  }

  initFilterForm() {
    this.orderFilterForm = this.fb.group({
      sort: ['dateDesc'],
      status: [],
      paymentMethod: [],
      isDiscounted: [],
    });
  }

  applyFilter() {
    let filterParams = this.orderFilterForm.value;

    if (this.amountRangeValues[0] != 0) {
      filterParams = {
        ...filterParams,
        priceFrom: this.amountRangeValues[0] * 100000,
      };
    }
    if (this.amountRangeValues[1] != 100) {
      filterParams = {
        ...filterParams,
        priceTo: this.amountRangeValues[1] * 100000,
      };
    }

    this.getOrderFilter(filterParams);

    this.sidebarService.closeAll();
  }

  resetFilter() {
    this.adminService.resetAdminOrderFilterParams();
    this.resetFilterForm();
  }

  private resetFilterForm() {
    this.orderFilterForm?.reset({
      sort: ['dateDesc'],
      status: null,
      paymentMethod: null,
      isDiscounted: null,
    });

    this.amountRangeValues = [0, 100];
  }

  getOrderFilter(formValues: any) {
    const orderParams: OrderFilterParams = {
      ...new OrderFilterParams(),
      sort: formValues.sort,
      status: formValues.status,
      paymentMethod: formValues.paymentMethod,
      amountFrom: formValues.amountFrom,
      amountTo: formValues.amountTo,
      isDiscounted: formValues.isDiscounted,
    };

    this.adminService.setAdminOrderFilterParams(orderParams);
  }

  get show() {
    return this.sidebarService.isOpen('filter-order');
  }

  set show(value: boolean) {
    if (value) {
      this.sidebarService.open('filter-order');
    } else {
      this.sidebarService.close('filter-order');
    }
  }
}
