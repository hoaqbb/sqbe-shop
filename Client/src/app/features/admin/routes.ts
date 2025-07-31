import { Route } from '@angular/router';
import { ManageProductComponent } from './manage-product/manage-product.component';
import { AdminComponent } from './admin.component';
import { ShopDashboardComponent } from './shop-dashboard/shop-dashboard.component';
import { ManageAccountComponent } from './manage-account/manage-account.component';
import { ManageCategoryComponent } from './manage-category/manage-category.component';
import { ManageVariantComponent } from './manage-variant/manage-variant.component';

export const adminRoutes: Route[] = [
  {
    path: '',
    component: AdminComponent,
    children: [
      { path: '', component: ShopDashboardComponent },
      { path: 'product', component: ManageProductComponent },
      { path: 'promotion', component: ManagePromotionComponent },
      { path: 'account', component: ManageAccountComponent },
      { path: 'category', component: ManageCategoryComponent },
      { path: 'variant', component: ManageVariantComponent },
      { path: '**', redirectTo: '', pathMatch: 'full' },
    ],
  },
];
