import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AdminService } from '../../../../core/services/admin.service';
import { CommonModule } from '@angular/common';
import { QuillModule } from 'ngx-quill';
import { ImageResize } from 'quill-image-resize-module-ts';
import { nanoid } from 'nanoid';
import Quill from 'quill';

Quill.register('modules/imageResize', ImageResize);

@Component({
  selector: 'app-create-blog',
  standalone: true,
  imports: [CommonModule, QuillModule, ReactiveFormsModule],
  templateUrl: './create-blog.component.html',
  styleUrl: './create-blog.component.css',
})
export class CreateBlogComponent implements OnInit {
  quillModules: any;
  blogForm: FormGroup;
  thumbnailUrl: string | ArrayBuffer | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private adminService: AdminService
  ) {
    this.quillModules = {
      imageResize: {
        modules: ['Resize', 'DisplaySize', 'Toolbar'],
        handleStyles: {
          border: 'none',
        },
        overlayStyles: {
          boxSizing: 'border-box',
          border: '5px solid #57b093',
        },
      },
    };
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.blogForm = this.formBuilder.group({
      title: ['', Validators.required],
      excerpt: [''],
      content: ['', Validators.required],
      thumbnailFile: [null, Validators.required],
      status: [false, Validators.required],
    });
  }

  createBlog() {
    if (this.blogForm.valid) {
      console.log(this.blogForm.value);
      this.adminService.createBlog(this.blogForm.value).subscribe({
        next: (response) => {
          console.log(response);
        },
        error: (err) => {
          console.log(err);
        },
      });
    }
  }

  selectedThumbnailFile: File | null = null;
  onThumbnailSelected(event: Event): void {
    const element = event.currentTarget as HTMLInputElement;
    let fileList: FileList | null = element.files;
    if (fileList && fileList.length > 0) {
      this.selectedThumbnailFile = fileList[0];
      this.blogForm.patchValue({ thumbnailFile: this.selectedThumbnailFile });
      this.blogForm.get('thumbnailFile')?.updateValueAndValidity(); // Kích hoạt validation

      // Tạo URL để hiển thị preview ảnh
      const reader = new FileReader();
      reader.onload = () => {
        this.thumbnailUrl = reader.result;
      };
      reader.readAsDataURL(this.selectedThumbnailFile);
    } else {
      this.selectedThumbnailFile = null;
      this.thumbnailUrl = null;
      this.blogForm.patchValue({ thumbnailFile: null });
      this.blogForm.get('thumbnailFile')?.updateValueAndValidity(); // Kích hoạt validation
    }
  }

  imageFiles: File[] = [];
  extractImageAndProcessHtml(htmlContent: string): string {
    // Create a temporary DOM element to parse the HTML content
    const tempElement = document.createElement('div');
    tempElement.innerHTML = htmlContent;

    // Find all img tags in the content
    const imgElements = tempElement.querySelectorAll('img');

    // If no images found, return the original content
    if (imgElements.length === 0) {
      return htmlContent;
    }

    // Process each image element
    Array.from(imgElements).map((img) => {
      const originalSrc = img.getAttribute('src');

      // Skip if no src attribute or already a server URL
      if (!originalSrc || originalSrc.startsWith('http')) {
        return;
      }

      // For data URLs (base64 encoded images)
      if (originalSrc.startsWith('data:image')) {
        try {
          // Convert base64 to file
          const imageFile = this.base64ToImageFile(
            originalSrc,
            `${nanoid()}.png`
          );

          //set data-filename for img tag is file name
          img.setAttribute('data-filename', imageFile.name);
          this.imageFiles.push(imageFile);
        } catch (error) {
          console.error('❌ Upload thất bại:', error);
        }
      }
    });

    return tempElement.innerHTML;
  }

  // Convert the base64 encoded string to image file
  base64ToImageFile(dataUrl: string, filename: string): File {
    const arr = dataUrl.split(',');
    const mimeMatch = arr[0].match(/:(.*?);/);
    const mime = mimeMatch ? mimeMatch[1] : '';
    const bstr = atob(arr[1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);

    while (n--) {
      u8arr[n] = bstr.charCodeAt(n);
    }

    return new File([u8arr], filename, { type: mime });
  }
}
