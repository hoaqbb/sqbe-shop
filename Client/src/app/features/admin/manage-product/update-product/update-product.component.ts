import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AdminService } from '../../../../core/services/admin.service';
import { FileUpload, FileUploadModule } from 'primeng/fileupload';
import { EditorModule } from 'primeng/editor';
import { ToastrService } from 'ngx-toastr';
import {
  ProductDetail,
  UpdateProduct,
} from '../../../../shared/models/product';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ColorCheckboxGroupComponent } from '../../../../shared/components/color-checkbox-group/color-checkbox-group.component';
import { SizeCheckboxGroupComponent } from '../../../../shared/components/size-checkbox-group/size-checkbox-group.component';

@Component({
  selector: 'app-update-product',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FileUploadModule,
    EditorModule,
    ColorCheckboxGroupComponent,
    SizeCheckboxGroupComponent,
  ],
  templateUrl: './update-product.component.html',
  styleUrl: './update-product.component.css',
})
export class UpdateProductComponent implements OnInit {
  updateProductForm: FormGroup;
  product: ProductDetail;
  productColors: any;
  productSizes: any;
  selectedColors: number[] = [];
  selectedSizes: number[] = [];
  @ViewChild('fileUpload') fileUpload: FileUpload;

  constructor(
    private formBuilder: FormBuilder,
    public adminService: AdminService,
    public toastr: ToastrService,
    private ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {}

  ngOnInit(): void {
    this.product = this.config.data;
    this.getProductColors();
    this.getProductSizes();
    this.initializeForm(this.product);
  }

  initializeForm(product: ProductDetail) {
    this.updateProductForm = this.formBuilder.group({
      name: [product.name, Validators.required],
      slug: [product.slug, Validators.required],
      price: [product.price, Validators.required],
      discount: [product.discount, Validators.required],
      categoryId: [
        product.category ? product.category.id : null,
        [Validators.required],
      ],
      description: [product.description, Validators.required],
      colors: this.formBuilder.control(this.selectedColors),
      sizes: this.formBuilder.control(this.selectedSizes),
    });
  }

  getProductColors() {
    this.productColors = this.adminService.colors.map((color) => {
      const isChecked = this.product.productVariants.some(
        (pColor) => pColor.colorCode == color.colorCode
      );
      if (isChecked) this.selectedColors.push(color.id);
    });
  }

  getProductSizes() {
    this.productSizes = this.adminService.sizes.map((size) => {
      const isChecked = this.product.productVariants.some(
        (pSize) => pSize.size == size.name
      );
      if (isChecked) this.selectedSizes.push(size.id);
    });
  }

  onUpload(event: any) {
    console.log(event.files);

    this.adminService.addProductImages(this.product.id, event.files).subscribe({
      next: (response: any) => {
        if (!this.product.productImages) {
          this.product.productImages = [];
        }
        this.product.productImages =
          this.product.productImages.concat(response);
        this.fileUpload.clear();
      },
      error: (err) => {
        console.error('Upload error', err);
      },
    });
  }

  updateProduct() {
    console.log(this.updateProductForm.value);
    const updateProduct: UpdateProduct = {
      name: this.updateProductForm.get('name').value,
      price: this.updateProductForm.get('price').value,
      discount: this.updateProductForm.get('discount').value,
      categoryId: this.updateProductForm.get('categoryId').value,
      description: this.updateProductForm.get('description').value,
      productColors: this.updateProductForm.get('colors').value,
      productSizes: this.updateProductForm.get('sizes').value,
    };

    this.adminService.updateProduct(this.product.id, updateProduct).subscribe({
      next: (res: any) => {
        this.ref.destroy();
        this.toastr.success('Cập nhật sản phẩm thành công!');
      },
      error: (err: any) => {
        console.log(err);
        this.toastr.error('Cập nhật sản phẩm thất bại!');
      },
    });
  }

  setMainPhoto(productId: string, imageId: number) {
    this.adminService.setMainImage(productId, imageId).subscribe(() => {
      this.product.productImages.forEach((p) => {
        if (p.isMain) p.isMain = false;
        if (p.id === imageId) p.isMain = true;
      });
    });
  }

  setSubPhoto(productId: string, imageId: number) {
    this.adminService.setSubImage(productId, imageId).subscribe(() => {
      this.product.productImages.forEach((p) => {
        if (p.isSub) p.isSub = false;
        if (p.id === imageId) p.isSub = true;
      });
    });
  }

  deleteImage(productId: string, imageId: number) {
    this.adminService.deleteProductImage(productId, imageId).subscribe(() => {
      this.product.productImages = this.product.productImages.filter(
        (x) => x.id !== imageId
      );
    });
  }
}
