﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Kooboo.Commerce.Data
{
    public class CommerceRepository<T> : IRepository<T> where T : class
    {
        private CommerceDatabase _database;

        public ICommerceDatabase Database
        {
            get
            {
                return _database;
            }
        }

        protected DbContext DbContext
        {
            get
            {
                return _database.DbContext;
            }
        }

        // Passing in ICommerceDatabase instead of CommerceDatabase is because,
        // in IoC container the service type is ICommerceDatabase.
        // If we ask for CommerceDatabase here, the IoC container will not be able to provide the CommerceDatabase instance.
        public CommerceRepository(ICommerceDatabase database)
        {
            Require.NotNull(database, "database");
            Require.That(database is CommerceDatabase, "Requires type " + typeof(CommerceDatabase) + ".");

            _database = (CommerceDatabase)database;
        }

        public IQueryable<T> Query()
        {
            return DbContext.Set<T>();
        }

        public T Find(params object[] id)
        {
            return DbContext.Set<T>().Find(id);
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            return Query().Where(predicate).FirstOrDefault();
        }

        public void Create(T entity)
        {
            Require.NotNull(entity, "entity");

            var table = DbContext.Set<T>();
            table.Add(entity);

            DbContext.SaveChanges();
        }

        public void Update(T entity)
        {
            var entry = DbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var keys = DbContext.GetKeys(entity);
                var dbEntity = DbContext.Set<T>().Find(keys);
                var dbEntry = DbContext.Entry(dbEntity);
                dbEntry.CurrentValues.SetValues(entity);
            }

            DbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            Require.NotNull(entity, "entity");

            DbContext.Set<T>().Remove(entity);
            DbContext.SaveChanges();
        }
    }
}
