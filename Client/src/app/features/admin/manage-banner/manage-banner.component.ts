import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TableModule } from 'primeng/table';
import { AdminService } from '../../../core/services/admin.service';
import { ConfirmationService } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { FileUploadModule } from 'primeng/fileupload';
import { CreateBannerComponent } from "./create-banner/create-banner.component";

@Component({
  selector: 'app-manage-banner',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, ConfirmDialogModule, CalendarModule, FileUploadModule, CreateBannerComponent],
  templateUrl: './manage-banner.component.html',
  styleUrl: './manage-banner.component.css',
  providers: [ConfirmationService, DialogService],
})
export class ManageBannerComponent implements OnInit {
  isCreateFormVisible: boolean = false;
  isUpdateFormVisible: boolean = false;
  dialogRef: DynamicDialogRef;
  banners: any[] = [];

  updateBannerForm: FormGroup;

  constructor(
    private adminService: AdminService,
    private formBuilder: FormBuilder,
    private confirmationService: ConfirmationService,
    private toastr: ToastrService,
    public dialogService: DialogService
  ) {}

  ngOnInit(): void {
    this.getBanners();
  }

  getBanners() {
    this.adminService.getBanners().subscribe({
      next: (response: any[]) => (this.banners = response),
      error: (error: any) => console.error(error),
    });
  }

  

  manageCreateForm() {
    this.isCreateFormVisible = !this.isCreateFormVisible;
  }

  manageUpdateForm(category) {
    // if (this.isUpdateFormVisible == false) {
    //   this.initializeUpdateForm(category);
    //   this.isUpdateFormVisible = !this.isUpdateFormVisible;
    // } else {
    //   this.updateBannerForm.disable();
    //   this.isUpdateFormVisible = !this.isUpdateFormVisible;
    // }
  }
}
