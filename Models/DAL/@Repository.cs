using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using LightMethods.Survey.Models.Entities;
using AppHelper;
using AutoMapper;
using LightMethods.Survey.Models.Services;

namespace LightMethods.Survey.Models.DAL
{

    public class Repository<T> : IRepository<T> where T : class, IEntity, new()
    {

        protected SurveyContext Context;
        protected UnitOfWork CurrentUOW;

        protected DbSet<T> Entities;

        internal Repository(UnitOfWork uow)
        {
            this.CurrentUOW = uow;
            this.Context = uow.Context;
            this.Entities = Context.Set<T>();
        }

        public virtual IQueryable<T> All
        {
            get { return Entities; }
        }

        public virtual IQueryable<T> AllAsNoTracking
        {
            get { return All.AsNoTracking(); }
        }

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Entities;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public virtual IQueryable<T> AllIncludingNoTracking(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Entities.AsNoTracking();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public virtual T Find(System.Guid id)
        {
            return Entities.Find(id);
        }

        public T FindIncluding(System.Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Entities;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where(o => o.Id == id).Single();
        }

        public virtual void InsertOrUpdate(T entity)
        {
            if (entity.Id == default(System.Guid))
            {
                // New entity
                entity.Id = Guid.NewGuid();
                if (entity is Entity)
                    (entity as Entity).DateCreated = (entity as Entity).DateUpdated = DateTimeService.UtcNow;

                Entities.Add(entity);

            }
            else
            {
                // Existing entity
                var entry = Context.Entry(entity);

                if (entry.State == EntityState.Added || entry.State== EntityState.Detached)
                    Context.Entry(entity).State = EntityState.Modified;

                if (entity is Entity && entry.State == EntityState.Modified)
                    (entity as Entity).DateUpdated = DateTimeService.UtcNow;

            }
        }

        public virtual void InsertOrUpdate(IEnumerable<T> entities)
        {
            if (entities == null) return;
            foreach (var entity in entities)
                InsertOrUpdate(entity);

        }

        public virtual void Update(T entity, ICollection<KeyValuePair<string, string>> values)
        {
            ModelUpdater.UpdateModel<T>(entity, values, this.Context);
        }

        protected virtual void Map(T src, T dest)
        {
            Mapper.CreateMap<T, T>().ForMember("DateCreated", c => c.Ignore()).ForMember("DateUpdated", c => c.Ignore());
            Mapper.Map(src, dest);
        }

        public void Update(ICollection<T> dbSet, IEnumerable<T> newSet)
        {

            if (dbSet == null && newSet == null)
                return;

            if (newSet == null) newSet = new List<T>();

            if (dbSet == null)
            {
                InsertOrUpdate(newSet);
                return;
            }

            var itemsToUpdate = new List<T>();
            var itemsToAdd = new List<T>();
            var itemsToDelete = new List<T>();

            if (dbSet != null && newSet.Any())
                itemsToUpdate = dbSet.Intersect(newSet).ToList();

            if (newSet.Any())
                itemsToAdd = newSet.Where(i => i.Id == Guid.Empty).ToList();

            if (dbSet != null)
                itemsToDelete = dbSet.Except(newSet).ToList();


            itemsToUpdate.ForEach(d =>
            {
                Map(newSet.Single(i => i.Id == d.Id), d);
                if (d is Entity && Context.Entry(d).State == EntityState.Modified)
                    (d as Entity).DateUpdated = DateTimeService.UtcNow;
            });
            
            itemsToDelete.ForEach(d => Delete(d));
            itemsToAdd.ForEach(d =>
            {
                dbSet.Add(d);
                InsertOrUpdate(d);
            });


        }

        public void Delete(System.Guid id)
        {
            var entity = Find(id);
            Delete(entity);
            //Context.Entry(entity).State = EntityState.Deleted;

        }

        public virtual void Delete(IEnumerable<System.Guid> ids)
        {
            if (ids == null) return;
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        public virtual void Delete(T entity)
        {
            if (entity is IArchivable && ((IArchivable)entity).MustBeArchived(Context))
                ((IArchivable)entity).Archive();
            else
                Context.Entry(entity).State = EntityState.Deleted;
        }

        public void Delete(IEnumerable<T> entities)
        {
            if (entities == null) return;
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
