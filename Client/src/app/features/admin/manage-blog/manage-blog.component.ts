import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TableModule } from 'primeng/table';
import { Blog } from '../../../shared/models/blog';
import { AdminService } from '../../../core/services/admin.service';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-manage-blog',
  standalone: true,
  imports: [CommonModule, RouterLink, TableModule, ButtonModule],
  templateUrl: './manage-blog.component.html',
  styleUrl: './manage-blog.component.css'
})
export class ManageBlogComponent implements OnInit{
  blogs: Blog[] = [];
  
  constructor(
      private adminService: AdminService,
    ) { }
  ngOnInit(): void {
    this.getBlogs();
  }

  getBlogs() {
  this.adminService.getBlogs().subscribe({
    next: (res: any) => {
      this.blogs = res;
    },
  });
}
}
