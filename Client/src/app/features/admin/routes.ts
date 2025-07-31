import { Route } from '@angular/router';
import { ManageProductComponent } from './manage-product/manage-product.component';
import { AdminComponent } from './admin.component';
import { ManageOrderComponent } from './manage-order/manage-order.component';
import { ShopDashboardComponent } from './shop-dashboard/shop-dashboard.component';
import { ManagePromotionComponent } from './manage-promotion/manage-promotion.component';
import { ManageAccountComponent } from './manage-account/manage-account.component';
import { ManageCategoryComponent } from './manage-category/manage-category.component';
import { ManageVariantComponent } from './manage-variant/manage-variant.component';
import { ManageBlogComponent } from './manage-blog/manage-blog.component';
import { CreateBlogComponent } from './manage-blog/create-blog/create-blog.component';

export const adminRoutes: Route[] = [
  {
    path: '',
    component: AdminComponent,
    children: [
      { path: '', component: ShopDashboardComponent },
      { path: 'product', component: ManageProductComponent },
      { path: 'order', component: ManageOrderComponent },
      { path: 'promotion', component: ManagePromotionComponent },
      { path: 'blog', component: ManageBlogComponent },
      { path: 'blog/create', component: CreateBlogComponent },
      { path: 'account', component: ManageAccountComponent },
      { path: 'category', component: ManageCategoryComponent },
      { path: 'variant', component: ManageVariantComponent },
      { path: '**', redirectTo: '', pathMatch: 'full' },
    ],
  },
];
