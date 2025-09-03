import { Routes } from '@angular/router';
/// Login and register
import { LoginComponent } from './auth/login/login';
import { RegisterComponent } from './auth/register/register';
/// Debts
import { DashboardComponent } from './debt/dashboard/dashboard';
import { DebtCreateComponent } from './debt/debt-create/debt-create';
// Auth Guard
import { authGuard } from '../app/auth_guard/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },

  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'debt_create', component: DebtCreateComponent, canActivate: [authGuard] },


  { path: '**', redirectTo: 'login' }
];
