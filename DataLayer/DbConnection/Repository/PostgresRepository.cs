using Npgsql;
using System.Data;
using Dapper;
using DataLayer.DbConnection.Factory;

namespace DataLayer.DbConnection.Repository
{
    public class PostgresRepository<T>(IDbConnectionFactory dbConnectionFactory)
        : IPostgresRepository<T> where T : class
    {
        /// <summary>
        /// Variable for the use of the current connection
        /// </summary>
        private readonly IDbConnectionFactory _connection = dbConnectionFactory;
        /// <summary>
        /// Method that return the information in base of the params
        /// Note: Close the connection once it finish
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAsync(string storedProcedure, object? parameters = null)
        {
            /// Creates the new instance of the connection
            using IDbConnection? _connection = await dbConnectionFactory.CreateConnection()
                ?? throw new Exception("No se pudo generar la conexión para la BD");
            try
            {
                /// Return the data result
                return await _connection.QueryAsync<T>(
                                storedProcedure,
                                parameters,
                                commandType: CommandType.StoredProcedure
                            );
            }
            catch { throw; }
            finally { _connection.Close(); }
        }
        /// <summary>
        /// Method that creates a new register in base ob the params
        /// Note: Close the connection once it finish
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<int> PostAsync(string storedProcedure, object? parameters = null)
        {
            /// Creates the new instance of the connection
            using IDbConnection? _connection = await dbConnectionFactory.CreateConnection()
                ?? throw new Exception("No se pudo generar la conexión para la BD");
            try
            {
                /// Return the data result
                return await _connection.QueryFirstOrDefaultAsync<int>(
                    storedProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch { throw; }
            finally { _connection.Close(); }
        }
        /// <summary>
        /// Method that updates a register in base on the parameters
        /// Note: Close the connection once it finish
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(string storedProcedure, object parameters)
        {
            /// Creates the new instance of the connection
            using IDbConnection? _connection = await dbConnectionFactory.CreateConnection()
                ?? throw new Exception("No se pudo generar la conexión para la BD");
            try
            {
                /// Return the data result
                var affectedRows = await _connection.ExecuteAsync(
                    storedProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return affectedRows > 0;
            }
            catch { throw; }
            finally { _connection.Close(); }
        }
        /// <summary>
        /// Method that deletes a register in base of the parameters
        /// Note: Close the connection once it finish
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string storedProcedure, object parameters)
        {
            /// Creates the new instance of the connection
            using IDbConnection? _connection = await dbConnectionFactory.CreateConnection()
                ?? throw new Exception("No se pudo generar la conexión para la BD");
            try
            {
                /// Return the data result
                var affectedRows = await _connection.ExecuteAsync(
                    storedProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return affectedRows > 0;
            }
            catch { throw; }
            finally { _connection.Close(); }
        }
    }
}
