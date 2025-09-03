using DataLayer.DbConnection.Factory;
using DataLayer.DbConnection.Repository;
using DataLayer.SharedInstance;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer.Repositories
{
    public class DeudaRepository
    {
        /// <summary>
        /// Method for the creation of a new 
        /// </summary>
        /// <param name="deudorId"></param>
        /// <param name="acreedorId"></param>
        /// <param name="monto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task CreateDebt(int debtorId, int creditorId, decimal total)
        {
            /// Validate if the share instance is created for the database execution methods
            if (SharedDataInstance.Instance.ServiceProvider == null)
                throw new Exception("Se tienen problemas en el uso de inyección de dependencias.");
            using var scope = SharedDataInstance.Instance.ServiceProvider.CreateScope();

        }

    }
}
