using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightMethods.Survey.Models.Entities;

namespace LightMethods.Survey.Models.DAL
{
    public interface IAdultsRepository<T> : IRepository<T> where T: Adult
    {
        T InsertOrUpdate(T entity, Project project);
        T CreateOrUpdate(T dest, T src, Project project);
    }
}
