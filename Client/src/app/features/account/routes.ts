import { Route } from "@angular/router";
import { LoginComponent } from "./login/login.component";
import { RegisterComponent } from "./register/register.component";

export const accountRoutes: Route[] = [
    {path:'', component: AccountComponent},
    {path: 'login', component: LoginComponent},
    {path: 'register', component: RegisterComponent},
    {path: 'order/:id', component: OrderDetailComponent},
    {path: '**', redirectTo: 'not-found', pathMatch: 'full' },
]