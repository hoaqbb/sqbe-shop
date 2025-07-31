import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { FileUploadModule } from 'primeng/fileupload';
import { AdminService } from '../../../../core/services/admin.service';

@Component({
  selector: 'app-create-banner',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CalendarModule,
    FileUploadModule,
  ],
  templateUrl: './create-banner.component.html',
  styleUrl: './create-banner.component.css',
})
export class CreateBannerComponent implements OnInit {
  createBannerForm: FormGroup;

  constructor(
    private adminService: AdminService,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initializeCreateForm();
  }

  initializeCreateForm() {
    this.createBannerForm = this.formBuilder.group({
      linkUrl: [''],
      isActive: [true],
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      displayType: [0, Validators.required],
      file: [null, Validators.required],
    });
  }
  onSelectedFile(e) {

  }

  createBanner() {
    
  }
}
