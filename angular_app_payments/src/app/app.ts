import { Component, signal } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd  } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('angular_app_payments');
  isLoggedIn = false;

  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.isLoggedIn = !!localStorage.getItem('userId');
      }
    });
  }

  logout(): void {
    localStorage.removeItem('userId');
    this.router.navigate(['/login']);
  }
}
