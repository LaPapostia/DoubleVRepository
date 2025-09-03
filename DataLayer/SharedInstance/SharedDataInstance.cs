using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.SharedInstance
{
    class SharedDataInstance
    {
        /// <summary>
        ///  Unique instance for the data shared
        /// </summary>
        private static readonly Lazy<SharedDataInstance> instance = new(() => new SharedDataInstance());
        /// <summary>
        ///  Public propertu for the acces of the data shared instance
        /// </summary>
        public static SharedDataInstance Instance => instance.Value;
        /// <summary>
        /// Provider for the dependency injection instance
        /// </summary>
        public IServiceProvider? ServiceProvider { get; set; }
    }
}
