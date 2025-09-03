using DataLayer.DbConnection.Repository;
using DataLayer.SharedInstance;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer.Repositories
{
    public class UserRepository
    {
        /// <summary>
        /// Method for the creation of a new 
        /// </summary>
        /// <param name="deudorId"></param>
        /// <param name="acreedorId"></param>
        /// <param name="monto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task CreateUser(string p_usuario, string p_correo, string p_contrasenia)
        {
            /// Validate if the share instance is created for the database execution methods
            if (SharedDataInstance.Instance.ServiceProvider == null)
                throw new Exception("Se tienen problemas en el uso de inyección de dependencias.");
            using var scope = SharedDataInstance.Instance.ServiceProvider.CreateScope();
            /// Creating the instance for the repository
            PostgresRepository<List<int>> repository = scope.ServiceProvider.GetRequiredService<PostgresRepository<List<int>>>();
            /// Creating the params object
            object param = new
            {
                p_usuario,
                p_correo,
                p_contrasenia
            };
            /// Executing the sentence
            await repository.PostAsync("fn_usuario_crear");
        }

    }
}
