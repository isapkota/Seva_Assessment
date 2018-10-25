using LinqSpecs;
using NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Seva.Assessment.FluentNHibernetImpl
{
    public interface IDbSet<T> : ICollection<T> where T : Entity
    {
        void SetSession(ISession nSession);
        IQueryable<T> FindAll(Specification<T> specification);
        IQueryable<T> FindAll(Expression<Func<T, bool>> filter);
        IQueryable<T> FindAll();
        T FindOne(Specification<T> specification);
        T FindOne(Expression<Func<T, bool>> filter);
        void Update(T item);
    }

    public class DbSet<T> : DbSetBase, IDbSet<T> where T : Entity
    {
        public DbSet() { }

        public DbSet(ISession nSession)
        {
            SetSession(nSession);
        }

        public int Count
        {
            get { return Session.Query<T>().Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            Transact(() => Session.Save(item));
        }

        public void Update(T item)
        {
            Transact(() => Session.Update(item));
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            if (item.Id == 0)
                return false;
            return Session.Get<T>(item.Id) != null;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public IQueryable<T> FindAll(Specification<T> specification)
        {
            return GetQuery(specification).AsQueryable();
        }
        public IQueryable<T> FindAll(Expression<Func<T, bool>> filter)
        {
            return GetQuery(filter).AsQueryable();
        }
        public IQueryable<T> FindAll()
        {
            return Session.Query<T>().AsQueryable();
        }

        public T FindOne(Specification<T> specification)
        {
            return GetQuery(specification).SingleOrDefault();
        }

        public T FindOne(Expression<Func<T, bool>> filter)
        {
            var query = GetQuery(filter);
            return FindOne(query);
        }

        private T FindOne(IEnumerable<T> query)
        {
            var data = query.ToList<T>();
            if (data != null)
            {
                return data.FirstOrDefault<T>();
            }
            return null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Session.Query<T>().Take(1000).GetEnumerator();
        }

        public bool Remove(T item)
        {
            Transact(() => Session.Delete(item));
            return true;
        }

        private IQueryable<T> GetQuery(
          Specification<T> specification)
        {

            return Session.Query<T>().AsQueryable()
              .Where(specification.ToExpression());
        }

        private IQueryable<T> GetQuery(
          Expression<Func<T, bool>> filter)
        {
            var query = Session.Query<T>().Where(filter);
            return query.AsQueryable();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Session.Query<T>().Take(1000).GetEnumerator();
        }
    }
}
