using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DbConnection.Repository
{
    public interface IPostgresRepository<T>
    {
        /// <summary>
        /// Method to obtain the data
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(string storedProcedure, object? parameters = null);
        /// <summary>
        /// Method for the creation of data
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<int> PostAsync(string storedProcedure, object? parameters = null);
        /// <summary>
        /// Method for the register update
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(string storedProcedure, object parameters);
        /// <summary>
        /// Method for the register delete
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string storedProcedure, object parameters);
    }
}
