import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { DebtService, Debt } from '../../services/debt.service';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

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

  selectedDebt: Debt | null = null;
  showModal = false;

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
     
      let match = true;

      if (estado && d.estado !== estado) match = false;
      if (acreedor && !d.acreedor.toLowerCase().includes(acreedor.toLowerCase())) match = false;
      if (fechaInicio && new Date(d.fecha_creacion) < new Date(fechaInicio)) match = false; // Va a fallar porque no puse la region :c
      if (fechaFin && new Date(d.fecha_creacion) > new Date(fechaFin)) match = false;

      return match;
    });
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.switchTab(this.activeTab);
  }

  openModal(debt: Debt): void {
    this.selectedDebt = debt;
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedDebt = null;
  }


  exportToExcel(): void {
    // Preparamos los datos
    const data = this.filteredDebts.map(d => ({
      ID: d.deuda_id,
      Deudor: d.deudor,
      Acreedor: d.acreedor,
      Monto: d.monto,
      Saldo: d.saldo,
      Estado: d.estado,
      Fecha: new Date(d.fecha_creacion).toLocaleDateString()
    }));

    // Convertir a hoja Excel
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(data);
    const workbook: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Deudas');

    // Exportar
    const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob: Blob = new Blob([excelBuffer], { type: 'application/octet-stream' });
    saveAs(blob, `deudas_${this.activeTab}_${new Date().toISOString().slice(0,10)}.xlsx`);
  }
}
