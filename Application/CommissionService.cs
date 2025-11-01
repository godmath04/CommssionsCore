using Microsoft.EntityFrameworkCore;
using APICoreComisiones.Data;
using APICoreComisiones.ViewModels;
using APICoreComisiones.Models;

namespace APICoreComisiones.Application
{
    public class CommissionService : ICommissionService
    {
        private readonly AppDbContext _db;

        public CommissionService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CommissionRowVm>> CalculateAsync(DateTime start, DateTime end)
        {
            if (end < start) throw new ArgumentException("Fecha fin no puede ser menor que fecha inicio");

            var totales = await _db.Ventas
                .AsNoTracking()
                .Where(v => v.FechaVenta >= start && v.FechaVenta <= end)
                .GroupBy(v => new { v.VendedorId, v.Vendedor.Nombre })
                .Select(g => new
                {
                    g.Key.VendedorId,
                    Nombre = g.Key.Nombre,
                    Total = g.Sum(x => x.Monto)
                })
                .ToListAsync();

            var reglas = await _db.Reglas
                .AsNoTracking()
                .OrderBy(r => r.MontoMinimo)
                .ToListAsync();

            var filas = new List<CommissionRowVm>(totales.Count);
            foreach (var t in totales)
            {
                var rate = GetRate(t.Total, reglas); 
                filas.Add(new CommissionRowVm
                {
                    VendedorId = t.VendedorId,
                    Vendedor = t.Nombre,
                    TotalVentas = t.Total,
                    PorcentajeAplicado = rate,
                    ComisionCalculada = Math.Round(t.Total * rate, 2, MidpointRounding.ToEven)
                });
            }
            return filas;
        }

        private decimal GetRate(decimal total, IReadOnlyList<Regla> reglas)
        {
            if (reglas == null || reglas.Count == 0) return 0;

            var rule = reglas
                .Where(r => r.MontoMinimo <= total)
                .OrderByDescending(r => r.MontoMinimo)
                .FirstOrDefault();

            return rule?.Porcentaje ?? 0;
        }

    }
}
