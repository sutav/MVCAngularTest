using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace MVCAngularTest.DataAccess
{

    public abstract class BaseEfDataProvider : IEfDataProvider
    {
        /// <summary>
        /// Get connection factory
        /// </summary>
        /// <returns>Connection factory</returns>
        public abstract IDbConnectionFactory GetConnectionFactory();

        /// <summary>
        /// Initialize connection factory
        /// </summary>
        public void InitConnectionFactory()
        {
            Database.DefaultConnectionFactory = GetConnectionFactory();
        }


        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitDatabase()
        {
            InitConnectionFactory();

        }

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// </summary>
        public abstract bool StoredProceduredSupported { get; }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public abstract DbParameter GetParameter();
    }
}
