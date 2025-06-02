import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { AdminService } from '../../../../core/services/admin.service';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ConfirmationService } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { TextInputComponent } from '../../../../shared/components/text-input/text-input.component';
import { SizeDetail } from '../../../../shared/models/size';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-manage-size',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    TextInputComponent,
    ReactiveFormsModule,
    ConfirmDialogModule,
  ],
  templateUrl: './manage-size.component.html',
  styleUrl: './manage-size.component.css',
  providers: [ConfirmationService],
})
export class ManageSizeComponent implements OnInit {
  isCreateFormVisible: boolean = false;
  isUpdateFormVisible: boolean = false;
  createSizeForm: FormGroup;
  updateSizeForm: FormGroup;

  constructor(
    public adminService: AdminService,
    private confirmationService: ConfirmationService,
    private formBuilder: FormBuilder,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeCreateForm();
  }

  initializeCreateForm() {
    this.createSizeForm = this.formBuilder.group({
      name: ['', Validators.required],
    });
  }

  initializeUpdateForm(size: SizeDetail) {
    this.updateSizeForm = this.formBuilder.group({
      id: [size.id, Validators.required],
      name: [size.name, Validators.required],
    });
  }

  createSize() {
    if (this.createSizeForm.invalid) {
      this.toastr.error('Vui lòng điền đầy đủ thông tin.');
      return;
    }

    const newSize = this.createSizeForm.value;

    this.adminService.createSize(newSize).subscribe({
      next: (size: SizeDetail) => {
        this.adminService.sizes.push(size);
        this.toastr.success('Tạo kích thước thành công');
        this.isCreateFormVisible = false;
        this.createSizeForm.reset();
      },
      error: (error) => {
        this.toastr.error('Tạo kích thước thất bại!');
      },
    });
  }

  updateSize() {
    if (this.updateSizeForm.invalid) {
      this.toastr.error('Vui lòng điền đầy đủ thông tin.');
      return;
    }

    const updatedSize = this.updateSizeForm.value;
    const selectedSize = this.adminService.sizes.find(
      (c) => c.id == updatedSize.id
    );
    if (selectedSize.name === updatedSize.name) return null;

    this.adminService.updateSize(updatedSize).subscribe({
      next: (size: SizeDetail) => {
        const index = this.adminService.sizes.findIndex(
          (c) => c.id === size.id
        );
        if (index !== -1) {
          this.adminService.sizes[index].name = size.name;
          this.adminService.sizes[index].updateAt = size.updateAt;
        }
        this.toastr.success('Cập nhật kích thước thành công');
        this.isUpdateFormVisible = false;
        this.updateSizeForm.reset();
      },
      error: (error) => {
        this.toastr.error('Cập nhật kích thước thất bại!');
      },
    });
  }

  deleteSize(sizeId) {
    this.adminService.deleteSize(sizeId).subscribe({
      next: () => {
        this.adminService.sizes = this.adminService.sizes.filter(
          (c) => c.id !== sizeId
        );
        this.toastr.success('Xóa kích thước thành công');
      },
      error: (error) => {
        this.toastr.error('Xóa kích thước thất bại!');
      },
    });
  }

  deleteConfirmationDialog(sizeId: number) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa kích thước này không?',
      header: 'Xác nhận xóa',
      icon: 'pi pi-info-circle',
      acceptButtonStyleClass: 'p-button-danger p-button-text',
      rejectButtonStyleClass: 'p-button-text p-button-text',
      acceptIcon: 'none',
      rejectIcon: 'none',
      accept: () => {
        this.deleteSize(sizeId);
      },
    });
  }

  manageCreateForm() {
    this.isCreateFormVisible = !this.isCreateFormVisible;
  }

  manageUpdateForm(size) {
    if (this.isUpdateFormVisible == false) {
      this.initializeUpdateForm(size);
      this.isUpdateFormVisible = !this.isUpdateFormVisible;
    } else {
      this.updateSizeForm.disable();
      this.isUpdateFormVisible = !this.isUpdateFormVisible;
    }
  }
}
