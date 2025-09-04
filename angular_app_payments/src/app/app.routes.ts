import { Routes } from '@angular/router';
/// Login and register
import { LoginComponent } from './components/auth/login/login';
import { RegisterComponent } from './components/auth/register/register';
/// Debts
import { DashboardComponent } from './components/debt/dashboard/dashboard';
import { DebtCreateComponent } from './components/debt/debt-create/debt-create';
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
