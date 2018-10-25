using NHibernate;
using System;

namespace Seva.Assessment.FluentNHibernetImpl
{
    public interface IDataContext<T> : IDisposable where T : Entity
    {
        IDbSet<T> Data { get; }
    }

    public class DataContext<T> : BaseDbContext, IDataContext<T> where T : Entity
    {
        public IDbSet<T> Data { get; }

        public DataContext(ISessionFactory sessionFactory) : base(sessionFactory)
        {
            StartSession();
            Data = new DbSet<T>(CurrentSession);
        }
    }
}
