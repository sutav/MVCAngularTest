using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCAngularTest.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        protected string ConnectionString;
        private MVCAngularTestDataContext context;

        public UnitOfWork(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public IDbContext DbContext
        {
            get
            {
                if (context == null)
                {
                    context = new MVCAngularTestDataContext(this.ConnectionString);
                }
                return context;
            }
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
