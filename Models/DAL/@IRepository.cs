using LightMethods.Survey.Models.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace LightMethods.Survey.Models.DAL
{
    public interface IRepository<T> where T : IEntity
    {
        IQueryable<T> All { get; }

        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        T Find(System.Guid id);

        void InsertOrUpdate(T entity);

        void Delete(System.Guid id);

        void Save();
    }
}
