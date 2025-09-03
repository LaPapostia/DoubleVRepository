import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Debt {
  deuda_id: number;
  usuario_id: number;
  monto: number;
  estado: string;
  fecha_creacion: string;
  deudor_id: number;
  acreedor_id: number;
  deudor: string;
  acreedor: string;
}

@Injectable({ providedIn: 'root' })
export class DebtService {
  private apiUrl = 'https://localhost:7269/api/Debt';

  constructor(private http: HttpClient) {}

  // obtiene deudas por usuario con filtros opcionales
  listDebtsByUser(
    usuarioId: number
  ): Observable<Debt[]> {
    return this.http.get<Debt[]>(`${this.apiUrl}/list/user/${usuarioId}`);
  }

  createDebt(debt: { deudor_id: number; acreedor_id: number; monto: number }): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, debt);
  }
}
