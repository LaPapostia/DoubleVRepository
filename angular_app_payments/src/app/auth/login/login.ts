import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string | null = null;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.loginForm = this.fb.group({
      correo: ['', [Validators.required, Validators.email]],
      contrasenia: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    const { correo, contrasenia } = this.loginForm.value;

    this.authService.login(correo, contrasenia).subscribe({
      next: (res) => {
        debugger
        alert('Bienvenido ' + res.usuario);
        localStorage.setItem('userId', res.userId);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error('Error en login:', err);

        if (err.status === 401) {
          alert('Credenciales incorrectas');
        } else if (err.status === 500) {
          alert('Error interno del servidor: ' + (err.error || 'Intenta nuevamente más tarde'));
        } else {
          alert('Error en el login: ' + (err.error?.message || err.message));
        }
      }
    });
  }

}
