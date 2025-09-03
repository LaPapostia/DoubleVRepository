using DataLayer.DbConnection.Factory;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace DataLayer.DbConnection
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        /// Variable for the stablishment of the connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Constructor for the assignment of the connection string value
        /// </summary>
        /// <param name="config"></param>
        public DbConnectionFactory(IConfiguration config)
        {
            try
            {
                _connectionString = config.GetConnectionString("Postgres")
                    ?? string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Method that resturn the create ofg the connection engine
        /// </summary>
        /// <returns></returns>
        public async Task<IDbConnection> CreateConnection()
        {
            try
            {
                /// Validate if the connection string is empty
                if (string.IsNullOrEmpty(_connectionString))
                    throw new Exception("La cadena de conexión de 'Postgres' no se encuentra configurada");
                /// Return the connection engine
                var conn = new NpgsqlConnection(_connectionString);
                /// Retun the connection open
                await conn.OpenAsync();
                return conn;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
