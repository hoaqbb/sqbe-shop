import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ProductDetailsComponent } from './features/products/product-details/product-details.component';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { ServerErrorComponent } from './shared/components/server-error/server-error.component';
import { ShopComponent } from './features/shop/shop.component';
import { CartComponent } from './features/cart/cart.component';

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
      { path: 'cart', component: CartComponent },
    ],
  },
  { path: 'not-found', component: NotFoundComponent },
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
