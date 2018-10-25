using NHibernate;
using System;

namespace Seva.Assessment.FluentNHibernetImpl
{
    public abstract class BaseDbContext : IDisposable
    {
        public void SetupDbSets<T>(IDbSet<T> entity) where T : Entity
        {
            entity.SetSession(CurrentSession);
        }

        protected readonly ISessionFactory _sessionFactory;

        private ISession _session;
        private ITransaction _transaction;

        public ISession CurrentSession { get { return _session; } }

        protected BaseDbContext(ISessionFactory sessionFactory)
        {
            if (sessionFactory != null)
                _sessionFactory = sessionFactory;
            else
                _sessionFactory = new NHibernate.Cfg.Configuration().Configure().BuildSessionFactory();
        }

        public void StartSession(bool withTransaction = false)
        {
            _session = _sessionFactory.OpenSession();
            if (withTransaction)
            {
                StartTransaction();
            }
        }

        public void StartTransaction()
        {
            if (_transaction != null)
                _transaction = _session.BeginTransaction();
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void RollBack()
        {
            _transaction?.Rollback();
        }

        public void CompleteSession()
        {
            if (_session.Transaction.IsActive)
                _session.Transaction.Commit();
            if (_session.IsOpen)
            {
                _session.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CompleteSession();
            }
        }

        ~BaseDbContext()
        {
            Dispose(false);
        }
    }
}
