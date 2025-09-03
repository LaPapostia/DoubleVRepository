using DataLayer.DbConnection;
using DataLayer.DbConnection.Factory;
using DataLayer.DbConnection.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer
{
    public class DependencyInjection
    {
        /// <summary>
        /// Method that generates the serives config
        /// </summary>
        /// <returns></returns>
        private static async Task<IServiceCollection> ConfigureServices()
        {
            try
            {
                /// Create a new servie instance
                var services = new IServiceCollection();
                /// Creating the services for the database connection
                
                return services;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Method for the generation iof the services collection
        /// </summary>
        /// <returns></returns>
        public static async Task<IServiceCollection> GenerateService()
        {
            try
            {
                return await ConfigureServices();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar servicios: {ex.Message}");
                throw;
            }
        }
    }
}
