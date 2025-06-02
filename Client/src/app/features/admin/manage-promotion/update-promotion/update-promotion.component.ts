import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  PromotionDetail,
  UpdatePromotion,
} from '../../../../shared/models/promotion';
import { AdminService } from '../../../../core/services/admin.service';
import { ToastrService } from 'ngx-toastr';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CommonModule } from '@angular/common';
import { CalendarModule } from 'primeng/calendar';

@Component({
  selector: 'app-update-promotion',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CalendarModule, FormsModule],
  templateUrl: './update-promotion.component.html',
  styleUrl: './update-promotion.component.css',
})
export class UpdatePromotionComponent implements OnInit {
  updatePromotionForm: FormGroup;
  promotion: PromotionDetail;
  isPercentageDiscount: boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    public adminService: AdminService,
    public toastr: ToastrService,
    private ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {}

  ngOnInit(): void {
    this.promotion = this.config.data;
    this.initializeUpdatePromotionForm();
  }

  initializeUpdatePromotionForm() {
    let a = (this.updatePromotionForm = this.formBuilder.group({
      code: [this.promotion.code, Validators.required],
      description: [this.promotion.description, Validators.required],
      discountType: [this.promotion.discountType, Validators.required],
      discountValue: [
        this.promotion.discountValue,
        [Validators.required, Validators.min(0)],
      ],
      minOrderAmount: [this.promotion.minOrderAmount, Validators.min(0)],
      usageLimit: [this.promotion.usageLimit, Validators.min(0)],
      usageCount: [{ value: this.promotion.usageCount, disabled: true }],
      isActive: [this.promotion.isActive, Validators.required],
      isUserRestricted: [this.promotion.isUserRestricted, Validators.required],
      validateFrom: [new Date(this.promotion.validateFrom)],
      validateTo: [new Date(this.promotion.validateTo)],
      maxDiscountValue: [this.promotion.maxDiscountValue],
    }));
  }

  updatePromotion() {
    const updatePromotion: UpdatePromotion = {
      code: this.updatePromotionForm.get('code').value,
      description: this.updatePromotionForm.get('description').value,
      discountType: this.updatePromotionForm.get('discountType').value,
      discountValue: this.updatePromotionForm.get('discountValue').value,
      minOrderAmount: this.updatePromotionForm.get('minOrderAmount').value,
      usageLimit: this.updatePromotionForm.get('usageLimit').value,
      isActive: this.updatePromotionForm.get('isActive').value,
      isUserRestricted: this.updatePromotionForm.get('isUserRestricted').value,
      validateFrom: this.updatePromotionForm.get('validateFrom').value,
      validateTo: this.updatePromotionForm.get('validateTo').value,
      maxDiscountValue: this.updatePromotionForm.get('maxDiscountValue').value,
    };
    
    this.adminService
      .updatePromotion(this.promotion.id, updatePromotion)
      .subscribe({
        next: (res: any) => {
          let promotionIndex = this.adminService.promotions.findIndex(p => p.id === this.promotion.id);
          this.adminService.promotions[promotionIndex] = res;
          this.ref.destroy();
          this.toastr.success('Cập nhật mã khuyến mãi thành công!');
        },
        error: (err: any) => {
          console.log(err);
          this.toastr.error('Cập nhật mã khuyến mãi thất bại!');
          this.toastr.error(err.error);
        },
      });
  }

  onDiscountTypeChange(selectedValue: any): void {
    this.isPercentageDiscount = selectedValue === 0;
    const maxDiscountValueControl =
      this.updatePromotionForm.get('maxDiscountValue');

    if (!this.isPercentageDiscount) {
      maxDiscountValueControl.setValue(null);
      maxDiscountValueControl.clearValidators();
    } else {
      maxDiscountValueControl.setValidators([Validators.min(0)]);
    }
    maxDiscountValueControl.updateValueAndValidity();
  }
}
