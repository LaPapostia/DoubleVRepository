import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DebtService } from '../../../services/debt.service';
import { UserService, User } from '../../../services/user.service';

@Component({
  selector: 'app-debt-create',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './debt-create.html',
  styleUrl: './debt-create.css'
})
export class DebtCreateComponent implements OnInit {
  debtForm!: FormGroup;
  usuarioId!: number;
  successMessage: string | null = null;
  errorMessage: string | null = null;
  users: User[] = [];

  constructor(
    private fb: FormBuilder,
    private debtService: DebtService,
    private userService: UserService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const storedUserId = localStorage.getItem('userId');
    if (storedUserId) {
      this.usuarioId = Number(storedUserId);
    }

    this.debtForm = this.fb.group({
      deudor_id: [this.usuarioId, Validators.required],
      acreedor_id: ['', Validators.required],
      monto: ['', [Validators.required, Validators.min(1)]],
      estado: ['PENDIENTE', Validators.required]
    });

    this.userService.listUsers().subscribe((res) => {
      this.users = res.filter(u => u.usuario_id !== this.usuarioId);
      this.cd.detectChanges();
    });
  }

  onSubmit(): void {
    if (this.debtForm.invalid) return;

    this.debtService.createDebt(this.debtForm.value).subscribe({
      next: () => {
        alert('Deuda creada con éxito');
        this.errorMessage = null;
        setTimeout(() => this.router.navigate(['/dashboard']), 1000);
      },
      error: (err) => {
        alert('Error al crear deuda');
        this.successMessage = null;
        this.errorMessage = err.error?.message || 'Error al crear deuda';
        console.error('Error al crear deuda:', err);
      }
    });
  }
}
