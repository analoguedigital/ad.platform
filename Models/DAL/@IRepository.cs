using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using LightMethods.Survey.Models.Entities;

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
