import { Component, SecurityContext } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DiscountPipe } from '../../../shared/pipes/discount.pipe';
import { Product, ProductDetail, ProductVariant } from '../../../shared/models/product';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { GalleryModule, ImageItem, GalleryItem } from 'ng-gallery';
import { ColorVariant } from '../../../shared/models/color';
import { SizeVariant } from '../../../shared/models/size';
import { DomSanitizer } from '@angular/platform-browser';
import { CartService } from '../../../core/services/cart.service';
import { ToastrService } from 'ngx-toastr';
import { SidebarService } from '../../../core/services/sidebar.service';
import { DialogModule } from 'primeng/dialog';
import { ProductItemComponent } from '../product-item/product-item.component';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DiscountPipe,
    GalleryModule,
    DialogModule,
    ProductItemComponent
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css',
})
export class ProductDetailsComponent {
  slug: string;
  productDetail!: ProductDetail;

  colorVariants: ColorVariant[];
  selectedColor: ColorVariant | null = null;
  selectedSize?: string;
  selectedVariantId: number | null;
  sizeChartImageUrl: string = '';

  images: GalleryItem[] = [];

  displaySizeChartDialog: boolean = false;

  relatedProducts: Product[] = [];

  constructor(
    public shopService: ShopService,
    private cartService: CartService,
    private sanitizer: DomSanitizer,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private sidebarService: SidebarService,
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.slug = params.get('slug');
      this.getProductDetail(this.slug);
      this.getRelatedProducts(this.slug);
    });
  }

  getRelatedProducts(slug) {
    this.relatedProducts = this.shopService.getRelatedProducts(slug)
  }

  getProductDetail(slug: string) {
    if (slug) {
      this.shopService.getProductBySlug(this.slug).subscribe((res) => {
        this.productDetail = res;
        // reset product imgs every get product details
        this.images = [];
        this.productDetail.productImages.forEach((img) =>
          this.images.push(
            new ImageItem({ src: img.imageUrl, thumb: img.imageUrl })
          )
        );
        this.productDetail.description = this.sanitizeHTML(
          this.productDetail.description
        );
        this.colorVariants = this.groupVariantsByColor(
          this.productDetail.productVariants
        );

        this.selectedColor = this.colorVariants[0];
        this.selectedSize = this.getFirstAvailableSize();

        this.sizeChartImageUrl = this.getSizeChartByProductCategory(this.productDetail.category.slug);
      });
    }
  }

  groupVariantsByColor(productVariants: ProductVariant[]): ColorVariant[] {
    return productVariants.reduce((acc: ColorVariant[], variant) => {
      const existingColor = acc.find((c) => c.color === variant.color);

      if (!existingColor) {
        acc.push({
          color: variant.color,
          colorCode: variant.colorCode,
          sizeVariants: [
            { id: variant.id, size: variant.size, quantity: variant.quantity },
          ],
        });
      } else {
        existingColor.sizeVariants?.push({
          id: variant.id,
          size: variant.size,
          quantity: variant.quantity,
        });
      }

      return acc;
    }, []);
  }

  //auto check the first size available
  isFirstAvailableVariant(variant: SizeVariant, $index: number): boolean {
    // Find the first available size in the selected color
    if (this.selectedColor?.sizeVariants) {
      var firstAvailableIndex = this.selectedColor.sizeVariants.findIndex(
        (v) => v.quantity > 0
      );
      // Check if the current variant matches that first available one
      return variant.quantity > 0 && $index === firstAvailableIndex;
    }

    return false;
  }

  // Update getFirstAvailableSize to also set selectedVariantId
  getFirstAvailableSize(): string | undefined {
    if (this.selectedColor?.sizeVariants) {
      const firstAvailable = this.selectedColor.sizeVariants.find(
        (v) => v.quantity > 0
      );
      if (firstAvailable) {
        this.selectedVariantId = firstAvailable.id;
        return firstAvailable.size;
      }
    }
    return undefined;
  }

  onColorChange(color: ColorVariant) {
    this.selectedColor = color;
    // Get first available size for the new color
    this.selectedSize = this.getFirstAvailableSize();
    // Update selectedVariantId based on new color and size
    this.updateSelectedVariantId();
  }

  onSizeChange(size: string) {
    this.selectedSize = size;
    // Update selectedVariantId when size changes
    this.updateSelectedVariantId();
  }

  // Add new method to update selectedVariantId
  private updateSelectedVariantId() {
    if (this.selectedColor && this.selectedSize) {
      const selectedVariant = this.selectedColor.sizeVariants.find(
        (variant) => variant.size === this.selectedSize
      );
      if (selectedVariant) {
        this.selectedVariantId = selectedVariant.id;
      }
    }
  }

  addToCart(): void {
    if (!this.selectedVariantId) return;

    this.cartService
      .addToCartAndUpdate(this.selectedVariantId)
      .subscribe(() => {
        this.sidebarService.toggle('cart');
      });
  }

  //sanitizes the product description HTML content to prevent XSS attacks
  sanitizeHTML(html: string): string {
    return this.sanitizer.sanitize(SecurityContext.HTML, html) || '';
  }

  
  //show size chart dialog
  showSizeChartDialog(): void {
    this.displaySizeChartDialog = true;
  }

  getSizeChartByProductCategory(category: string): string {
    switch (category.toLowerCase()) {
      case 'top':
        return 'assets/top-size-chart.jpg';
      case 'bottom':
        return 'assets/bottom-size-chart.jpg';
      case 'outerwear':
        return 'assets/outerwear-size-chart.jpg';
      default:
        return '';
    }
  }
}
