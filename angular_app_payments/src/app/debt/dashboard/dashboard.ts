import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
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
  deudasQueDebo: Debt[] = [];
  deudasQueMeDeben: Debt[] = [];
  filteredDebts: Debt[] = [];
  filterForm!: FormGroup;
  usuarioId!: number;
  activeTab: 'debo' | 'meDeben' = 'debo'; // control de tabs

  totalDebo: number = 0;
  totalMeDeben: number = 0;

  constructor(private debtService: DebtService, private fb: FormBuilder, private cd: ChangeDetectorRef) {}

  ngOnInit(): void {
    const storedUserId = localStorage.getItem('userId');
    if (storedUserId) {
      this.usuarioId = Number(storedUserId);
    }

    this.filterForm = this.fb.group({
      estado: [''],
      acreedor: [''],
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

      // dividir las deudas
      this.deudasQueDebo = this.debts.filter(d => d.deudor_id === this.usuarioId);
      this.deudasQueMeDeben = this.debts.filter(d => d.acreedor_id === this.usuarioId);

      // calcular totales
      this.totalDebo = this.deudasQueDebo.reduce((sum, d) => sum + d.monto, 0);
      this.totalMeDeben = this.deudasQueMeDeben.reduce((sum, d) => sum + d.monto, 0);

      // inicializar lista filtrada con el tab activo
      this.filteredDebts = [...this.deudasQueDebo];

       this.switchTab(this.activeTab);
       this.cd.detectChanges();
    });
  }

  switchTab(tab: 'debo' | 'meDeben'): void {
    this.activeTab = tab;
    this.filteredDebts = tab === 'debo'
      ? [...this.deudasQueDebo]
      : [...this.deudasQueMeDeben];
    this.applyFilters();
  }

  applyFilters(): void {
    const { estado, acreedor, fechaInicio, fechaFin } = this.filterForm.value;

    const source = this.activeTab === 'debo'
      ? this.deudasQueDebo
      : this.deudasQueMeDeben;

    this.filteredDebts = source.filter((d) => {
      debugger
      let match = true;

      if (estado && d.estado !== estado) match = false;
      if (acreedor && !d.acreedor.toLowerCase().includes(acreedor.toLowerCase())) match = false;
      if (fechaInicio && new Date(d.fecha_creacion) < new Date(fechaInicio)) match = false;
      if (fechaFin && new Date(d.fecha_creacion) > new Date(fechaFin)) match = false;

      return match;
    });
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.switchTab(this.activeTab);
  }
}
