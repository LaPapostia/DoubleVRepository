import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DebtService } from '../../services/debt.service';

@Component({
  selector: 'app-debt-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './debt-create.html',
  styleUrl: './debt-create.css'
})
export class DebtCreateComponent {
  createForm: FormGroup;

  constructor(private fb: FormBuilder, private debtService: DebtService) {
    this.createForm = this.fb.group({
      deudorId: ['', Validators.required],
      acreedorId: ['', Validators.required],
      monto: ['', [Validators.required, Validators.min(1)]]
    });
  }

  onSubmit(): void {
    if (this.createForm.invalid) return;

    const newDebt = this.createForm.value;

    this.debtService.createDebt(newDebt).subscribe({
      next: (res) => {
        alert('Deuda creada con éxito');
        this.createForm.reset();
      },
      error: (err) => {
        console.error('Error al crear deuda:', err);
        alert('Error al crear la deuda');
      }
    });
  }
}
