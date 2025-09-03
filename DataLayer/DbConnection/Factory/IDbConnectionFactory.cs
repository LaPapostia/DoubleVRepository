using System.Data;

namespace DataLayer.DbConnection.Factory
{
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Method for the create of the connection engine
        /// </summary>
        /// <returns></returns>
        Task<IDbConnection> CreateConnection();
    }
}
