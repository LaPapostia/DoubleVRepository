import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {
 registerForm: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.registerForm = this.fb.group(
      {
        usuario: ['', Validators.required],
        correo: ['', [Validators.required, Validators.email]],
        contrasenia: ['', [Validators.required, Validators.minLength(6)]],
        confirmContrasenia: ['', Validators.required]
      },
      { validators: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(form: FormGroup) {
    const pass = form.get('contrasenia')?.value;
    const confirm = form.get('confirmContrasenia')?.value;
    return pass === confirm ? null : { passwordMismatch: true };
  }

  
  onSubmit(): void {
    if (this.registerForm.invalid) return;

    const { usuario, correo, contrasenia } = this.registerForm.value;

    this.authService.register(usuario, correo, contrasenia).subscribe({
      next: (res) => {
        this.successMessage = 'Usuario creado con éxito. Redirigiendo al login...';
        this.errorMessage = null;
        this.registerForm.reset();

        // 👇 redirigir a login después de 2 segundos (puedes quitar el delay si no lo quieres)
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.successMessage = null;
        this.errorMessage = err.error?.message || 'Error al crear usuario';
        console.error('Error en registro:', err);

        if (err.status === 400) {
          alert('Ya existe un usuario con este correo');
        } else if (err.status === 500) {
          alert('Error interno del servidor: ' + (err.error || 'Intenta nuevamente más tarde'));
        } else {
          alert('Error en el registro: ' + (err.error?.message || err.message));
        }
      }
    });
  }
}
