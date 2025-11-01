using APICoreComisiones.ViewModels;

namespace APICoreComisiones.Application
{
    public interface ICommissionService
    {
        Task<List<CommissionRowVm>> CalculateAsync(DateTime start, DateTime end);
    }
}
