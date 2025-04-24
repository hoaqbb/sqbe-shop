import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'account', loadChildren: () => import('./features/account/routes')
        .then(r => r.accountRoutes)},
    {path: '**', redirectTo: '', pathMatch: 'full'},
];
