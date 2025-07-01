import { Route } from "@angular/router";
import { LoginComponent } from "./login/login.component";
import { RegisterComponent } from "./register/register.component";
import { AccountComponent } from "./account.component";
import { VerifyEmailComponent } from "./verify-email/verify-email.component";
import { ResetPasswordComponent } from "./reset-password/reset-password.component";
import { OrderDetailComponent } from "./order-detail/order-detail.component";

export const accountRoutes: Route[] = [
    {path:'', component: AccountComponent},
    {path: 'login', component: LoginComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'verify-email', component: VerifyEmailComponent},
    {path: 'reset-password', component: ResetPasswordComponent},
    {path: 'order/:id', component: OrderDetailComponent},
    {path: '**', redirectTo: 'not-found', pathMatch: 'full' },
]