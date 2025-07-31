import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { FileUploadModule } from 'primeng/fileupload';
import { AdminService } from '../../../../core/services/admin.service';
import { Banner } from '../../../../shared/models/banner';

@Component({
  selector: 'app-update-banner',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CalendarModule,
    FileUploadModule,
  ],
  templateUrl: './update-banner.component.html',
  styleUrl: './update-banner.component.css',
})
export class UpdateBannerComponent implements OnInit {
  updateBannerForm: FormGroup;
  banner: Banner;

  constructor(
      private adminService: AdminService,
      private formBuilder: FormBuilder
    ) {}

  ngOnInit(): void {
    this.initializeUpdateForm();
  }

  initializeUpdateForm() {
    this.updateBannerForm = this.formBuilder.group({
      linkUrl: [''],
      isActive: [this.banner.isActive],
      startDate: [new Date(this.banner.startDate), Validators.required],
      endDate: [new Date(this.banner.endDate), Validators.required],
      displayType: [this.banner.displayType, Validators.required],
      // file: [null, Validators.required],
    });
  }

  updateBanner() {

  }

  onSelectedFile(e) {

  }
}
