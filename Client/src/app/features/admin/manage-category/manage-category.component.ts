import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TableModule } from 'primeng/table';
import { AdminService } from '../../../core/services/admin.service';
import { ToastrService } from 'ngx-toastr';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';

@Component({
  selector: 'app-manage-category',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    ConfirmDialogModule,
    TextInputComponent
  ],
  templateUrl: './manage-category.component.html',
  styleUrl: './manage-category.component.css',
  providers: [ConfirmationService, DialogService],
})
export class ManageCategoryComponent implements OnInit {
  dialogRef: DynamicDialogRef;
  isCreate: boolean = false;
  createForm: FormGroup;

  constructor(
    public adminService: AdminService,
    private formBuilder: FormBuilder,
    private confirmationService: ConfirmationService,
    private toastr: ToastrService,
    public dialogService: DialogService
  ) {}

  ngOnInit(): void {
  }
}
