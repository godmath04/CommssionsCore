# 🧾 APICoreComisiones

API desarrollada en **.NET 8 Web API** para el cálculo de **comisiones de vendedores** según reglas configuradas.
El proyecto utiliza una arquitectura por capas simplificada (Controlador, Servicio, Capa de Datos) que separa las responsabilidades principales para mantener el código organizado y fácil de entender.

## 📘 Descripción general

El sistema permite a una aplicación (como un frontend de Angular) solicitar el cálculo de comisiones enviando un rango de fechas
(`FechaInicio`, `FechaFin`).

La API calcula las comisiones basándose en:
    - Las ventas registradas para cada vendedor.
    - Las reglas de comisión almacenadas en la base de datos (ej. "Si vendes más de $10,000, obtienes un 5%").
    - El cálculo aplica una lógica de "comisión plana":

---

## 🏗️ Arquitectura del proyecto

El proyecto utiliza una **estructura por capas**, inspirada en *Clean Architecture*:
```
APICoreComisiones
│
├── Controllers/ # Endpoints HTTP (API pública)
│ └── CommissionController.cs
│
├── Application/ # Capa de lógica del negocio
│ ├── ICommissionService.cs
│ └── CommissionService.cs
│
├── Data/ # Acceso a base de datos (EF Core)
│ └── AppDbContext.cs
│
├── Models/ # Entidades persistentes
│ ├── Regla.cs
│ ├── Vendedor.cs
│ └── Venta.cs
│
├── ViewModels/ # Moldes (DTOs) para los datos que entran y salen de la API.
│ ├── CalculateCommissionVm.cs
│ └── CommissionRowVm.cs
│ 
│
├── appsettings.json # Configuración (cadena de conexión)
└── Program.cs # Configuración principal
```
## ⚙️ Flujo de ejecución

1. El frontend (Angular) envía una solicitud:
   ```json
   {
     "fechaInicio": "2025-10-01T00:00:00Z",
     "fechaFin": "2025-10-31T23:59:59Z"
   }
   ```
2. CommissionController valida las fechas y llamas al servicio
3. CommissionService:
   - Consulta BDD para obtener las ventas
   - Consulta las ventas por vendedor y suma sus montos totales.
   - Carga las reglas
   - Para cada vendedor se determina el porcetaje que le corresponde comparando su total de ventas con las teglas. 
4. La API responde con un JSON:
 ```json
   {
     "vendedorId": 2,
     "vendedor": "Luis",
     "totalVentas": 26000,
     "porcentajeAplicado": 0.15,
     "comisionCalculada": 3900
   }
   ```

## EJECUCIÓN DEL PROYECTO
1. Clonar el repositorio
  - git clone https://github.com/godmath04/CommssionsCore.git
  - cd APICoreComisiones
2. Configurar la cadena de conexión en appsettings.json
3. Crear la base de datos y migraciones
4. Ejecutar la API
