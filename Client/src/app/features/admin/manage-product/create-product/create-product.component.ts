import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { FileUploadModule } from 'primeng/fileupload';
import { EditorModule } from 'primeng/editor';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { ColorCheckboxGroupComponent } from '../../../../shared/components/color-checkbox-group/color-checkbox-group.component';
import { SizeCheckboxGroupComponent } from '../../../../shared/components/size-checkbox-group/size-checkbox-group.component';
import { CreateProduct } from '../../../../shared/models/product';
import { AdminService } from '../../../../core/services/admin.service';
import { ToastrService } from 'ngx-toastr';
import { InputNumberModule } from 'primeng/inputnumber';
import { AdminProductFilterParams } from '../../../../shared/models/adminParams';

@Component({
  selector: 'app-create-product',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    FileUploadModule,
    EditorModule,
    ColorCheckboxGroupComponent,
    SizeCheckboxGroupComponent,
    InputNumberModule,
  ],
  templateUrl: './create-product.component.html',
  styleUrl: './create-product.component.css',
})
export class CreateProductComponent implements OnInit {
  createProductForm: FormGroup;
  mainImage!: File;
  subImage!: File;
  productImages: File[] = [];
  maxPrice = Number.MAX_SAFE_INTEGER;

  constructor(
    private ref: DynamicDialogRef,
    private formBuilder: FormBuilder,
    public adminService: AdminService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.createProductForm = this.formBuilder.group({
      name: ['', Validators.required],
      price: ['', Validators.required],
      discount: [0, Validators.required],
      description: ['', Validators.required],
      category: ['', Validators.required],
      productColors: [[], Validators.required],
      productSizes: [[], Validators.required],
    });
  }

  createProduct() {
    let product: CreateProduct = {
      name: this.createProductForm.get('name').value,
      price: this.createProductForm.get('price').value,
      description: this.createProductForm.get('description').value,
      discount: this.createProductForm.get('discount').value,
      categoryId: this.createProductForm.get('category').value,
      productColors: this.createProductForm.get('productColors').value,
      productSizes: this.createProductForm.get('productSizes').value,
      mainImage: this.mainImage,
      subImage: this.subImage,
      productImages: this.productImages,
    };

    this.adminService.createNewProduct(product).subscribe({
      next: (response) => {
        this.adminService.adminProductFilterParams.set(new AdminProductFilterParams());
        this.ref.destroy()
        this.toastr.success('Thêm sản phẩm thành công!');
      },
      error: (error) => {
        console.log(error);
        this.toastr.success('Lỗi khi thêm sản phẩm!');
      },
    });
  }

  onSelectedFile(event, varName) {
    switch (varName) {
      case 'mainImage': {
        this.mainImage = event.currentFiles[0];
        break;
      }
      case 'subImage': {
        this.subImage = event.currentFiles[0];
        break;
      }
      case 'productImages': {
        this.productImages = event.currentFiles;
        break;
      }
      default: {
        break;
      }
    }
  }
}
