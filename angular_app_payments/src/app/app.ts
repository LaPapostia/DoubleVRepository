import { Component, signal } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, RouterLink],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('angular_app_payments');
  isLoggedIn = false;
  mobileMenuOpen = false;
  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if (event.urlAfterRedirects === '/login') {
          localStorage.removeItem('userId');
          this.isLoggedIn = false;
        } else {
          this.isLoggedIn = !!localStorage.getItem('userId');
        }
      }
    });
  }

  logout(): void {
    localStorage.removeItem('userId');
    this.isLoggedIn = false;
    this.router.navigate(['/login']);
    this.mobileMenuOpen = false;
  }
}
