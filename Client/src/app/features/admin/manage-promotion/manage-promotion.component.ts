import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../core/services/admin.service';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { PromotionDetail } from '../../../shared/models/promotion';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastrService } from 'ngx-toastr';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CreatePromotionComponent } from './create-promotion/create-promotion.component';
import { UpdatePromotionComponent } from './update-promotion/update-promotion.component';

@Component({
  selector: 'app-manage-promotion',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, ConfirmDialogModule],
  templateUrl: './manage-promotion.component.html',
  styleUrl: './manage-promotion.component.css',
  providers: [DialogService, ConfirmationService],
})
export class ManagePromotionComponent implements OnInit {
  dialogRef: DynamicDialogRef;
  // promotions: PromotionDetail[] = [];

  constructor(
    public adminService: AdminService,
    private confirmationService: ConfirmationService,
    private toastr: ToastrService,
    public dialogService: DialogService
  ) {}

  ngOnInit(): void {
    this.getPromotions();
  }

  getPromotions() {
    if(this.adminService.promotions.length > 0) return;
    this.adminService.getPromotions().subscribe({
      next: (response: PromotionDetail[]) =>
        (this.adminService.promotions = response),
      error: (error: any) => console.error(error),
    });
  }

  showCreatePromotionDialog() {
    this.dialogRef = this.dialogService.open(CreatePromotionComponent, {
      width: '60vw',
      modal: true,
      // breakpoints: {
      //   '960px': '75vw',
      //   '640px': '90vw',
      // },
    });
  }

  showUpdatePromotionDialog(promotion) {
    this.dialogRef = this.dialogService.open(UpdatePromotionComponent, {
      data: promotion,
      width: '60vw',
      modal: true,
      breakpoints: {
        '960px': '75vw',
        '640px': '90vw',
      },
    });
  }

  deletePromotion(promotionId) {
    this.adminService.deletePromotion(promotionId).subscribe({
      next: () => {
        this.adminService.promotions = this.adminService.promotions.filter(
          (c) => c.id !== promotionId
        );
        this.toastr.success('Xóa mã khuyến mãi thành công');
      },
      error: (error) => {
        this.toastr.error('Xóa mã khuyến mãi thất bại!');
      },
    })
  }

  deleteConfirmationDialog(promotionId: number) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa mã khuyến mãi này không?',
      header: 'Xác nhận xóa',
      icon: 'pi pi-info-circle',
      acceptButtonStyleClass: 'p-button-danger p-button-text',
      rejectButtonStyleClass: 'p-button-text p-button-text',
      acceptIcon: 'none',
      rejectIcon: 'none',
      accept: () => {
        this.deletePromotion(promotionId);
      },
    });
  }
}
