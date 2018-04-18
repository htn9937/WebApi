using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IGenericRepository <T> where T : class
    {
        IEnumerable<T> GetAll();

        T GetById(object id);

        void Insert(T entity);

        void Delete(object id);

        void Delete(T entity);

        void Update(T entity);

        IEnumerable<T> GetMany(Func<T, bool> where);

        IQueryable<T> GetManyQueryable(Func<T, bool> where);

        IEnumerable<T> Get();

        T Get(Func<T, Boolean> where);

        void Delete(Func<T, Boolean> where);

        IQueryable<T> GetWithInclude(System.Linq.Expressions.Expression<Func<T, bool>> predicate, params string[] include);

        bool Exists(object primaryKey);

        T GetSingle(Func<T, bool> predicate);

        T GetFirst(Func<T, bool> predicate);
    }
}
