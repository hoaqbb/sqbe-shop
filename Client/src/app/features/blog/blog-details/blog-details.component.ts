import { Component, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { BlogDetail } from '../../../shared/models/blog';
import { CommonModule } from '@angular/common';
import { QuillViewHTMLComponent } from 'ngx-quill';

@Component({
  selector: 'app-blog-details',
  standalone: true,
  imports: [CommonModule, QuillViewHTMLComponent],
  templateUrl: './blog-details.component.html',
  styleUrl: './blog-details.component.css'
})
export class BlogDetailsComponent implements OnInit {
  slug: string;
  blogDetail!: BlogDetail;

  constructor(
    private shopService: ShopService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.slug = params.get('slug');
      this.getBlogDetail(this.slug);
    });
  }

  getBlogDetail(slug) {
    this.shopService.getBlogBySlug(slug).subscribe({
      next: (res: BlogDetail) => {
        this.blogDetail = res;
        this.blogDetail.content = this.replaceNbspWithSpace(this.blogDetail.content);
      },
      error: (error) => {
        console.error('Error fetching blog details:', error);
      }
    })
  }

  replaceNbspWithSpace(content) {
  // Replace all occurrences of "&nbsp;" with a space
    return content.replace(/&nbsp;/g, ' ');
  }
}
