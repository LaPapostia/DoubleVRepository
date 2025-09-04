# DoubleVRepository

Repository for the **Technical Test Fullstack Developer**
Stack: **Angular + .NET API + PostgreSQL**

---

## 📌 Table of Contents

1. [Project Overview](#project-overview)
2. [Folder Structure](#folder-structure)
3. [Backend - .NET](#backend---net)

   * [Requirements](#requirements)
   * [Packages](#packages)
   * [Running the API](#running-the-api)
4. [Frontend - Angular](#frontend---angular)

   * [Requirements](#requirements-1)
   * [Packages](#packages-1)
   * [Running the App](#running-the-app)
5. [Database](#database)
6. [UI & Styling](#ui--styling)
7. [Bibliography](#bibliography)

---

## 🔹 Project Overview

This repository contains a **fullstack application** for managing debts:

* **Backend**: .NET 8 Web API using **Dapper** and **Entity Framework Core**.
* **Frontend**: Angular 16 SPA with **Material**, **TailwindCSS**, and **Excel export functionality**.
* **Database**: PostgreSQL.

---

## 🔹 Folder Structure

```text
DoubleVRepository/
│
├─ backend/                 # .NET API project
│   ├─ Controllers/         # API controllers
│   ├─ Models/              # Data models
│   ├─ Services/            # Dapper services
│   ├─ Data/                # EF Core DbContext & migrations
│   └─ Program.cs           # Entry point
│
├─ frontend/                # Angular project
│   ├─ src/app/
│   │   ├─ components/      # Angular components (Login, Register, Dashboard, etc.)
│   │   ├─ auth_guard/      # Angular components (Login, Register, Dashboard, etc.)
│   │   ├─ services/        # API services
│   │   └─ app.module.ts    # Angular module
│   └─ tailwind.config.js   # TailwindCSS config
│
└─ README.md                # Project documentation
```

---

## 🔹 Backend - .NET

### Requirements

* [.NET SDK 8+](https://dotnet.microsoft.com/download)
* PostgreSQL database running locally

### Packages

Install via **NuGet**:

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Dapper
dotnet add package BCrypt.Net-Next
```

* **Npgsql.EntityFrameworkCore.PostgreSQL** → EF Core provider for PostgreSQL
* **Dapper** → Micro ORM for fast database access
* **BCrypt.Net-Next** → Password hashing

### Running the API

1. Navigate to the backend folder:

```bash
cd backend
```

2. Restore packages and build:

```bash
dotnet restore
dotnet build
```

3. Run the API:

```bash
dotnet run
```

API will run on `https://localhost:5001` or `http://localhost:5000`.

---

## 🔹 Frontend - Angular

### Requirements

* [Node.js 18+](https://nodejs.org/)
* Angular CLI v16+

### Packages

```bash
# Excel export
npm install xlsx file-saver
npm i --save-dev @types/file-saver

# Angular Material
ng add @angular/material

# TailwindCSS & PostCSS
npm install -D autoprefixer
npm install tailwindcss @tailwindcss/postcss postcss --force
```

### Running the App

1. Navigate to the frontend folder:

```bash
cd frontend
```

2. Install dependencies:

```bash
npm install
```

3. Run the Angular app:

```bash
ng serve
```

App will run at `http://localhost:4200`.

---

## 🔹 Database

* **PostgreSQL** used for storing users, debts, and payments.
* Tables include:

  * `usuarios`
  * `deudas`
  * `pagos`
* Connection handled via **Dapper**.
* Example connection string (appsettings.json):

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=doublevdb;Username=postgres;Password=yourpassword"
}
```

---

## 🔹 UI & Styling

* **TailwindCSS** for modern, minimalistic, responsive UI
* **Angular Material** for forms, tables, modals
* Consistent input height, spacing, and buttons throughout the app
* Dashboard with **balance cards**, **tabs**, **filters**, **table**, and **modal details**
* Login/Register forms with **high inputs**, responsive layout, and clear validation messages

---

## 🔹 Bibliography

* [TailwindCSS + Angular guide](https://tailwindcss.com/docs/installation/framework-guides/angular)


------------------------------------------------------------------------------------------------------------------
# Fase 2: Preguntas de Arquitectura y Experiencia

1. Microservicios
Si el sistema creciera y necesitara pasar de monolito a microservicios, propondría la siguiente división:
- Usuarios y autenticación → manejo de login, roles y tokens JWT.
- Gestión de deudas → creación, actualización, listado de deudas.
- Pagos y transacciones → registro de pagos, conciliaciones y saldos.
- Reportes / exportación → generación de Excel o PDF, dashboards agregados.
Consideraciones de comunicación:
- API REST para comunicación sincrónica entre servicios críticos (usuarios ↔ deudas).
- Colas de mensajería (AWS SQS, RabbitMQ o Kafka) para procesos asincrónicos como notificaciones o generación de reportes.
- Gateway API (API Gateway) para centralizar la entrada y aplicar seguridad, rate limiting y logging.

## Optimización en la nube (AWS)
| Componente                | Servicio AWS recomendado        | Razones                                                                                                      |
| ------------------------- | ------------------------------- | ------------------------------------------------------------------------------------------------------------ |
| **Autenticación segura**  | **IAM de AWS + JWT**            | Maneja usuarios, roles, autenticación multifactor y tokens seguros sin reinventar el manejo de credenciales. |
| **Base de datos**         | **Amazon RDS (PostgreSQL)**     | Escalable, con backups automáticos, alta disponibilidad y compatibilidad con SQL relacional.                 |
| **Cache y escalabilidad** | **ElastiCache (Redis)**         | Mejora rendimiento con datos frecuentes en memoria; reduce carga en la DB.                                   |
| **Balanceo de carga**     | **Elastic Load Balancer (ALB)** | Distribuye tráfico entre instancias EC2 o servicios ECS/EKS; soporta HTTPS y alta disponibilidad.            |


3. Buenas prácticas de seguridad
Al menos 3 prácticas clave para garantizar seguridad:

Backend:
- Uso de JWT con expiración y roles para acceso a APIs.
- Validación y sanitización de inputs para prevenir SQL Injection y XSS.

Frontend:
- HTTPS obligatorio para todas las comunicaciones.
- Almacenamiento seguro de tokens (ej. HttpOnly cookies o session storage con cuidado).

Despliegue en la nube:
- Configuración de grupos de seguridad (Security Groups) limitando puertos y IPs.
- Habilitar logs y monitoreo (CloudWatch) para detectar accesos sospechosos.

4. PostgreSQL vs NoSQL
- PostgreSQL (SQL) → cuando se requiere integridad de datos y relaciones complejas.
Ejemplo: Sistema de deudas, pagos y usuarios, donde es importante mantener consistencia y relaciones FK.

- NoSQL (MongoDB, DynamoDB) → cuando los datos son semi-estructurados y con consultas flexibles.
Ejemplo: Logs de actividad de usuarios, almacenamiento de archivos JSON de transacciones, historial de eventos o dashboards agregados.

5. Despliegue / CI-CD
Pipeline recomendado para producción:

- CI (Integración continua)
Ejecución de pruebas unitarias (xUnit en backend, Karma/Jasmine en Angular).
Build de frontend y backend.

- CD (Despliegue continuo)
Revisión automática de seguridad y vulnerabilidades (Dependabot, Snyk).
Push a producción solo si staging tests pasan.
Despliegue en AWS ECS/Fargate o EC2 + ALB, con rollback automático en caso de errores.


