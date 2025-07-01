import { Route } from '@angular/router';
import { BlogComponent } from './blog.component';
import { BlogDetailsComponent } from './blog-details/blog-details.component';

export const blogRoutes: Route[] = [
  { path: '', component: BlogComponent },
  { path: ':slug', component: BlogDetailsComponent },
];
