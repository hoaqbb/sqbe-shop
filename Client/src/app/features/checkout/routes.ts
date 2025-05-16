import { Route } from '@angular/router';
import { CheckoutComponent } from './checkout.component';
import { CheckoutResultComponent } from './checkout-result/checkout-result.component';
import { checkoutGuard } from '../../core/guards/checkout.guard';

export const checkoutRoutes: Route[] = [
  { path: '', canActivate: [checkoutGuard], component: CheckoutComponent },
  { path: 'result', component: CheckoutResultComponent },
];
