import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { AdminService } from '../../../../core/services/admin.service';
import { ToastrService } from 'ngx-toastr';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { CreatePromotion, PromotionDetail } from '../../../../shared/models/promotion';

@Component({
  selector: 'app-create-promotion',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CalendarModule],
  templateUrl: './create-promotion.component.html',
  styleUrl: './create-promotion.component.css',
})
export class CreatePromotionComponent implements OnInit {
  createPromotionForm: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    public adminService: AdminService,
    public toastr: ToastrService,
    private ref: DynamicDialogRef
  ) {}

  ngOnInit(): void {
    this.initializeCreatePromotionForm();
  }

  initializeCreatePromotionForm() {
    this.createPromotionForm = this.formBuilder.group({
      code: ['', Validators.required],
      description: [''],
      discountType: [0, Validators.required], // Mặc định là %
      discountValue: [0, [Validators.required, Validators.min(0)]],
      minOrderAmount: [null, Validators.min(0)],
      usageLimit: [0, Validators.min(0)],
      isActive: [false],
      isUserRestricted: [false],
      maxDiscountValue: [null],
      rangeDates: [null],
    });

    this.createPromotionForm
      .get('discountType')
      .valueChanges.subscribe((value) => {
        this.onDiscountTypeChange(value);
      });
  }

  createPromotion() {
    if (this.createPromotionForm.invalid) return null;
    const rangeDates = this.createPromotionForm.get('rangeDates').value
    const validateFromDate = rangeDates[0];
    const validateToDate = rangeDates[1];

    let newPromotion: CreatePromotion = {
      code: this.createPromotionForm.get('code').value,
      description: this.createPromotionForm.get('description').value,
      discountType: this.createPromotionForm.get('discountType').value,
      discountValue: this.createPromotionForm.get('discountValue').value,
      minOrderAmount: this.createPromotionForm.get('minOrderAmount').value,
      usageLimit: this.createPromotionForm.get('usageLimit').value,
      isActive: this.createPromotionForm.get('isActive').value,
      isUserRestricted: this.createPromotionForm.get('isUserRestricted').value,
      validateFrom: validateFromDate,
      validateTo: validateToDate,
      maxDiscountValue: this.createPromotionForm.get('maxDiscountValue').value,
    };
    
    return this.adminService.createPromotion(newPromotion).subscribe({
      next: (response: PromotionDetail) => {
        this.adminService.promotions.push(response);
        this.ref.destroy();
        this.toastr.success('Thêm mã thành công!');
      },
      error: (error) => {
        console.log(error);
        this.toastr.error('Lỗi khi thêm mã!');
        this.toastr.error(error.error);
      }
    });
  }

  maxDiscountValueField: boolean = true;
  onDiscountTypeChange(selectedValue: any): void {
    this.maxDiscountValueField = selectedValue == 0;
    if (!this.maxDiscountValueField) {
      this.createPromotionForm.get('maxDiscountValue').setValue(null);
      this.createPromotionForm.get('maxDiscountValue').clearValidators();
    } else {
      this.createPromotionForm
        .get('maxDiscountValue')
        .setValidators([Validators.min(0)]);
    }
    this.createPromotionForm.get('maxDiscountValue').updateValueAndValidity();
  }
}
