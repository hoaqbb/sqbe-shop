import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { AdminService } from '../../../../core/services/admin.service';
import { ButtonModule } from 'primeng/button';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TextInputComponent } from '../../../../shared/components/text-input/text-input.component';
import { ColorPickerModule } from 'primeng/colorpicker';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { ColorDetail } from '../../../../shared/models/color';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-manage-color',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    ReactiveFormsModule,
    TextInputComponent,
    ColorPickerModule,
    FormsModule,
    ConfirmDialogModule,
  ],
  templateUrl: './manage-color.component.html',
  styleUrl: './manage-color.component.css',
  providers: [ConfirmationService],
})
export class ManageColorComponent implements OnInit {
  isCreateFormVisible: boolean = false;
  isUpdateFormVisible: boolean = false;
  defaultColorCode: string = '#ff0000';
  createColorForm: FormGroup;
  updateColorForm: FormGroup;

  constructor(
    public adminService: AdminService,
    private formBuilder: FormBuilder,
    private confirmationService: ConfirmationService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeCreateForm();
  }

  initializeCreateForm() {
    this.createColorForm = this.formBuilder.group({
      name: ['', Validators.required],
      colorCode: [this.defaultColorCode, Validators.required],
    });
  }

  initializeUpdateForm(color: ColorDetail) {
    this.updateColorForm = this.formBuilder.group({
      id: [color.id, Validators.required],
      name: [color.name, Validators.required],
      colorCode: [color.colorCode, Validators.required],
    });
  }

  createColor() {
    if (this.createColorForm.invalid) {
      this.toastr.error('Vui lòng điền đầy đủ thông tin.');
      return;
    }

    const newColor = this.createColorForm.value;

    this.adminService.createColor(newColor).subscribe({
      next: (color: ColorDetail) => {
        this.adminService.colors.push(color);
        this.toastr.success('Tạo màu sắc thành công');
        this.isCreateFormVisible = false;
        this.createColorForm.reset();
      },
      error: (error) => {
        this.toastr.error('Tạo màu sắc thất bại!');
      },
    });
  }

  updateColor() {
    if (this.updateColorForm.invalid) {
      this.toastr.error('Vui lòng điền đầy đủ thông tin.');
      return;
    }

    const updatedColor = this.updateColorForm.value;
    const selectedColor = this.adminService.colors.find(
      (c) => c.id == updatedColor.id
    );
    if (selectedColor.name === updatedColor.name && selectedColor.colorCode == updatedColor.colorCode) return null;

    this.adminService.updateColor(updatedColor).subscribe({
      next: (color: ColorDetail) => {
        const index = this.adminService.colors.findIndex(
          (c) => c.id === color.id
        );
        if (index !== -1) {
          this.adminService.colors[index].name = color.name;
          this.adminService.colors[index].colorCode = color.colorCode;
          this.adminService.colors[index].updateAt = color.updateAt;
        }
        this.toastr.success('Cập nhật kích thước thành công');
        this.isUpdateFormVisible = false;
        this.updateColorForm.reset();
      },
      error: (error) => {
        this.toastr.error('Cập nhật kích thước thất bại!');
      },
    });
  }

  deleteColor(colorId) {
    this.adminService.deleteColor(colorId).subscribe({
      next: () => {
        this.adminService.colors = this.adminService.colors.filter(
          (c) => c.id !== colorId
        );
        this.toastr.success('Xóa màu sắc thành công');
      },
      error: (error) => {
        this.toastr.error('Xóa màu sắc thất bại!');
      },
    });
  }

  deleteConfirmationDialog(colorId: number) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa màu sắc này không?',
      header: 'Xác nhận xóa',
      icon: 'pi pi-info-circle',
      acceptButtonStyleClass: 'p-button-danger p-button-text',
      rejectButtonStyleClass: 'p-button-text p-button-text',
      acceptIcon: 'none',
      rejectIcon: 'none',
      accept: () => {
        this.deleteColor(colorId);
      },
    });
  }

  manageCreateForm() {
    this.isCreateFormVisible = !this.isCreateFormVisible;
  }

  manageUpdateForm(color?) {
    if (this.isUpdateFormVisible == false) {
      this.initializeUpdateForm(color);
      this.isUpdateFormVisible = !this.isUpdateFormVisible;
    } else {
      this.updateColorForm.disable();
      this.isUpdateFormVisible = !this.isUpdateFormVisible;
    }
  }
}
