import { Route } from '@angular/router';
import { ManageProductComponent } from './manage-product/manage-product.component';
import { AdminComponent } from './admin.component';


export const adminRoutes: Route[] = [
  {
    path: '',
    component: AdminComponent,
    children: [
      { path: 'product', component: ManageProductComponent },
  },
];
