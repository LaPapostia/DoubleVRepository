import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Debt {
  deudaId: number;
  usuarioId: number;
  monto: number;
  estado: string;
  fecha: string;
  deudor: string;
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
}
