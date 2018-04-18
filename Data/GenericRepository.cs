using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Private member variables...  
        internal BookManagementEntities Context;
        internal DbSet<T> DbSet;
        #endregion

        public GenericRepository(BookManagementEntities context)
        {
            this.Context = context;
            this.DbSet = context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }

        public virtual T GetById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual void Insert(T entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            T entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
        }

        public virtual void Update(T entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public IEnumerable<T> GetMany(Func<T, bool> where)
        {
            return DbSet.Where(where).ToList();
        }

        public IQueryable<T> GetManyQueryable( Func<T,bool> where)
        {
            return DbSet.Where(where).AsQueryable();
        }

        public IEnumerable<T> Get()
        {
            IQueryable<T> query = DbSet;
            return DbSet.ToList();
        }

        public T Get(Func<T,Boolean> where)
        {
            return DbSet.Where(where).FirstOrDefault<T>();
        }

        public virtual void Delete(Func<T,Boolean> where)
        {
            IQueryable<T> objects = DbSet.Where<T>(where).AsQueryable();
            foreach (T obj in objects)
            {
                DbSet.Remove(obj);
            }
        }

        public IQueryable<T> GetWithInclude(System.Linq.Expressions.Expression<Func<T,bool>> predicate, params string[] include)
        {
            IQueryable<T> query = this.DbSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate);
        }

        public bool Exists(object primaryKey)
        {
            return DbSet.Find(primaryKey) != null;
        }

        public T GetSingle(Func<T, bool> predicate)
        {
            return DbSet.Single<T>(predicate);
        }

        public T GetFirst(Func<T, bool> predicate)
        {
            return DbSet.First<T>(predicate);
        }
    }
}
