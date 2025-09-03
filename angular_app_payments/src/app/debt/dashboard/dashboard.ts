import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { DebtService, Debt } from '../../services/debt.service';

@Component({
  selector: 'app-dashboard',
  imports: [DatePipe, CurrencyPipe, CommonModule, ReactiveFormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})

export class DashboardComponent implements OnInit {
    debts: Debt[] = [];
    filteredDebts: Debt[] = [];
    filterForm!: FormGroup;
    usuarioId!: number;
    constructor(private debtService: DebtService, private fb: FormBuilder) {}

    ngOnInit(): void {
      const storedUserId = localStorage.getItem('userId');
      if (storedUserId) {
        this.usuarioId = Number(storedUserId);
      }

      this.filterForm = this.fb.group({
        estado: [''],
        deudor: [''],
        fechaInicio: [''],
        fechaFin: ['']
      });

      if (this.usuarioId) {
        this.loadDebts(this.usuarioId);
      }
    }

    loadDebts(usuarioId: number): void {
    this.debtService.listDebtsByUser(usuarioId).subscribe((res) => {
      this.debts = res;
      this.filteredDebts = [...this.debts]; // copia inicial
    });
  }

  applyFilters(): void {
    const { estado, deudor, fechaInicio, fechaFin } = this.filterForm.value;

    this.filteredDebts = this.debts.filter((d) => {
      let match = true;

      if (estado && d.estado !== estado) {
        match = false;
      }

      if (deudor && d.deudor?.toLowerCase().includes(deudor.toLowerCase()) === false) {
        match = false;
      }

      if (fechaInicio && new Date(d.fecha) < new Date(fechaInicio)) {
        match = false;
      }

      if (fechaFin && new Date(d.fecha) > new Date(fechaFin)) {
        match = false;
      }

      return match;
    });
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.filteredDebts = [...this.debts];
  }
}
