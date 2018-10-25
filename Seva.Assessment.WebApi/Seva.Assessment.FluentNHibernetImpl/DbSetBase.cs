using NHibernate;
using System;

namespace Seva.Assessment.FluentNHibernetImpl
{
    public abstract class DbSetBase
    {
        private ISession _nSession;

        public ISession Session { get { return _nSession; } }

        public void SetSession(ISession nSession)
        {
            _nSession = nSession;
        }

        #region Auto Transaction Wrapping

        protected virtual TResult Transact<TResult>(Func<TResult> func)
        {
            if (!Session.Transaction.IsActive)
            {
                // Wrap in transaction
                TResult result;
                using (var tx = Session.BeginTransaction())
                {
                    try
                    {
                        result = func.Invoke();
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }

                return result;
            }

            // Don't wrap;
            return func.Invoke();
        }

        protected virtual void Transact(Action action)
        {
            Transact(() =>
            {
                action.Invoke();
                return false;
            });
        }

        #endregion
    }
}
