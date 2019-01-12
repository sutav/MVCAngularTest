using System;

namespace MVCAngularTest.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContext DbContext { get; }
        int Save();
    }
}