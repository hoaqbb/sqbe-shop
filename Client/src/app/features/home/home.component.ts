import { Component, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { ProductItemComponent } from '../products/product-item/product-item.component';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { Banner } from '../../shared/models/banner';
import { CommonModule } from '@angular/common';
import { ProductFilterParams } from '../../shared/models/productParams';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ProductItemComponent, RouterModule, GalleryModule, CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{
  newArrivalproducts: Product[] = [];
  slides: GalleryItem[] = [];
  standees: any[] = [];
  hero: any;
  banners: Banner[] = [];

  constructor(private shopService: ShopService, private router: Router) {
  }
  
  ngOnInit(): void {
    this.getProducts();
    this.getBanners();
  }

  getBanners() {
    this.shopService.getBanners().subscribe({
      next: (response: any) => {
        this.banners = response;
        this.processBanner();
      },
      error: (error) => console.error(error)
    });
  }

  processBanner() {
    this.banners.forEach((banner) => {
      if(banner.displayType == "Slider")
        this.slides.push(new ImageItem({ src: banner.imageUrl }));
      if(banner.displayType == "Standee")
        this.standees.push(banner);
      if(banner.displayType == "Hero")
        this.hero = banner;
    })
  }

  navigateToBannerUrl(index) {
    if(this.banners[index] && this.banners[index].linkUrl)
      this.router.navigateByUrl(this.banners[index].linkUrl);
  }

  getProducts() {
    let params = new ProductFilterParams();
    this.shopService.getProducts(params).subscribe({
      next: (res) => this.newArrivalproducts = res.data,
      error: (err) => console.log(err)
    });
  }
}
