import { Routes } from '@angular/router';
/// Login and register
import { LoginComponent } from './auth/login/login';
import { RegisterComponent } from './auth/register/register';
/// Debts
import { DashboardComponent } from './debt/dashboard/dashboard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent }
];