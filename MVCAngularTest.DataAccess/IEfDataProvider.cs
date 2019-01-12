using System.Data.Entity.Infrastructure;
using MVCAngularTest.Core.Data;

namespace MVCAngularTest.DataAccess
{
    public interface IEfDataProvider : IDataProvider
    {
        /// <summary>
        /// Get connection factory
        /// </summary>
        /// <returns>Connection factory</returns>
        IDbConnectionFactory GetConnectionFactory();

        /// <summary>
        /// Initialize connection factory
        /// </summary>
        void InitConnectionFactory();


    }
}
