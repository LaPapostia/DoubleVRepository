import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7269/api/User'; // tu API backend

  constructor(private http: HttpClient) {}

  login(correo: string, contrasenia: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, { correo, contrasenia });
  }

  register(usuario: string, correo: string, contrasenia: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, { usuario, correo, contrasenia });
  }
}
