import { Route } from '@angular/router';
import { ManageProductComponent } from './manage-product/manage-product.component';
import { AdminComponent } from './admin.component';
import { ManageCategoryComponent } from './manage-category/manage-category.component';

export const adminRoutes: Route[] = [
  {
    path: '',
    component: AdminComponent,
    children: [
      { path: 'product', component: ManageProductComponent },
      { path: 'promotion', component: ManagePromotionComponent },
      { path: 'category', component: ManageCategoryComponent },
      { path: 'variant', component: ManageVariantComponent },
    ],
  },
];
