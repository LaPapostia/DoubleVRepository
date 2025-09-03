using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.SharedInstance
{
    public interface ISharedDataInstance
    {
        /// <summary>
        /// Provides for the dependency injection
        /// </summary>
        IServiceProvider? ServiceProvider { get; set; }
    }
}
