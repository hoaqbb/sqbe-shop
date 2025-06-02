import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TableModule } from 'primeng/table';
import { AdminService } from '../../../core/services/admin.service';
import { ToastrService } from 'ngx-toastr';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { CategoryDetail } from '../../../shared/models/category';

@Component({
  selector: 'app-manage-category',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    ConfirmDialogModule,
    TextInputComponent,
  ],
  templateUrl: './manage-category.component.html',
  styleUrl: './manage-category.component.css',
  providers: [ConfirmationService],
})
export class ManageCategoryComponent implements OnInit {
  isCreateFormVisible: boolean = false;
  isUpdateFormVisible: boolean = false;
  createCategoryForm: FormGroup;
  updateCategoryForm: FormGroup;

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
    this.createCategoryForm = this.formBuilder.group({
      name: ['', Validators.required],
      slug: ['', Validators.required],
    });
  }

  initializeUpdateForm(cat: CategoryDetail) {
    this.updateCategoryForm = this.formBuilder.group({
      id: [cat.id, Validators.required],
      name: [cat.name, Validators.required],
      slug: [cat.slug, Validators.required],
    });
  }

  createCategory() {
    if (this.createCategoryForm.invalid) {
      this.toastr.error('Vui lòng điền đầy đủ thông tin.');
      return;
    }

    const newCategory = this.createCategoryForm.value;

    this.adminService.createCategory(newCategory).subscribe({
      next: (category: CategoryDetail) => {
        this.adminService.categories.push(category);
        this.toastr.success('Tạo thể loại thành công');
        this.isCreateFormVisible = false;
        this.createCategoryForm.reset();
      },
      error: (error) => {
        this.toastr.error('Tạo thể loại thất bại!');
      },
    });
  }

  updateCategory() {
    if (this.updateCategoryForm.invalid) {
      this.toastr.error('Vui lòng điền đầy đủ thông tin.');
      return;
    }

    const updatedCategory = this.updateCategoryForm.value;
    const selectedCategory = this.adminService.categories.find(
      (c) => c.id == updatedCategory.id
    );
    if (
      selectedCategory.name === updatedCategory.name &&
      selectedCategory.slug === updatedCategory.slug
    )
      return null;

    this.adminService.updateCategory(updatedCategory).subscribe({
      next: (category: CategoryDetail) => {
        const index = this.adminService.categories.findIndex(
          (c) => c.id === category.id
        );
        if (index !== -1) {
          this.adminService.categories[index].name = category.name;
          this.adminService.categories[index].slug = category.slug;
          this.adminService.categories[index].updateAt = category.updateAt;
        }
        this.toastr.success('Cập nhật thể loại thành công');
        this.isUpdateFormVisible = false;
        this.updateCategoryForm.reset();
      },
      error: (error) => {
        this.toastr.error('Cập nhật thể loại thất bại!');
      },
    });
  }

  deleteCategory(categoryId: number) {
    this.adminService.deleteCategory(categoryId).subscribe({
      next: () => {
        this.adminService.categories = this.adminService.categories.filter(
          (c) => c.id !== categoryId
        );
        this.toastr.success('Xóa thể loại thành công');
      },
      error: (error) => {
        this.toastr.error('Xóa thể loại thất bại!');
      },
    });
  }

  deleteConfirmationDialog(categoryId: number) {
    this.confirmationService.confirm({
      message: 'Bạn có chắc muốn xóa thể loại này không?',
      header: 'Xác nhận xóa',
      icon: 'pi pi-info-circle',
      acceptButtonStyleClass: 'p-button-danger p-button-text',
      rejectButtonStyleClass: 'p-button-text p-button-text',
      acceptIcon: 'none',
      rejectIcon: 'none',
      accept: () => {
        this.deleteCategory(categoryId);
      },
    });
  }

  manageCreateForm() {
    this.isCreateFormVisible = !this.isCreateFormVisible;
  }

  manageUpdateForm(id) {
    if (this.isUpdateFormVisible == false) {
      const category = this.adminService.categories.find((c) => c.id === id);
      this.initializeUpdateForm(category);
      this.isUpdateFormVisible = !this.isUpdateFormVisible;
    } else {
      this.updateCategoryForm.disable();
      this.isUpdateFormVisible = !this.isUpdateFormVisible;
    }
  }
}
