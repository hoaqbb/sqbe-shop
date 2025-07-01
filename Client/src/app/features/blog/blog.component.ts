import { Component, OnInit } from '@angular/core';
import { Blog } from '../../shared/models/blog';
import { CommonModule } from '@angular/common';
import { BlogItemComponent } from './blog-item/blog-item.component';
import { ShopService } from '../../core/services/shop.service';

@Component({
  selector: 'app-blog',
  standalone: true,
  imports: [CommonModule, BlogItemComponent],
  templateUrl: './blog.component.html',
  styleUrl: './blog.component.css'
})
export class BlogComponent implements OnInit{
  blogItems: Blog[] = [];

  constructor(private shopService: ShopService) {}

  ngOnInit(): void {
    this.shopService.getBlogs().subscribe({
      next: (blogs: Blog[]) => {
        this.blogItems = blogs;
      },
      error: (error) => {
        console.error('Error fetching blogs:', error);
      }
    })
  }
  
}
