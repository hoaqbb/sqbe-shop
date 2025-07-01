import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ProductDetailsComponent } from './features/products/product-details/product-details.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { ServerErrorComponent } from './shared/components/server-error/server-error.component';
import { ShopComponent } from './features/shop/shop.component';
import { CartComponent } from './features/cart/cart.component';
import { SearchComponent } from './features/search/search.component';
import { ProductCategoryListComponent } from './features/products/product-category-list/product-category-list.component';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  {
    path: '',
    component: ShopComponent,
    children: [
      { path: '', component: HomeComponent },
      {
        path: 'account',
        loadChildren: () =>
          import('./features/account/routes').then((r) => r.accountRoutes),
      },
      { path: 'products/:slug', component: ProductDetailsComponent },
      {
        path: 'categories/:categorySlug',
        component: ProductCategoryListComponent,
      },
      { path: 'cart', component: CartComponent },
      { path: 'search', component: SearchComponent },
      {
        path: 'blog',
        loadChildren: () =>
          import('./features/blog/routes').then((r) => r.blogRoutes),
      },
    ],
  },
  {
    path: 'checkout',
    loadChildren: () =>
      import('./features/checkout/routes').then((m) => m.checkoutRoutes),
  },
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadChildren: () =>
      import('./features/admin/routes').then((m) => m.adminRoutes),
  },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
