using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Organiser.Data.Repositories
{
    public abstract class Repository<TC, T> : IRepository<T> where T : class where TC : DbContext
    {
        private readonly bool _disposed;

        public TC Context { get; set; }

        protected Repository(TC context)
        {
            try
            {
                Context = context;
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.RepositoryCtor" + e.Message);
            }
        }

        public void Add(T entity)
        {
            try
            {
                Context.Set<T>().Add(entity);

            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.Add" + e.Message);
            }

        }

        public void AddRange(IEnumerable<T> entitiesRange)
        {
            Context.Set<T>().AddRange(entitiesRange);
            ;
        }

        public IQueryable<T> GetAllToIQuerable()
        {
            try
            {
                DbSet<T> query = Context.Set<T>();
                return query;
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.GetAllToIQuerable" + e.Message);
            }
        }

        public List<T> GetAllToList()
        {
            try
            {
                List<T> queryToList = Context.Set<T>().ToList();
                return queryToList;
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.GetAllToList" + e.Message);
            }
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            try
            {
                IQueryable<T> query = Context.Set<T>().Where(predicate);
                return query;
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.GetFilteredToIQuerable" + e.Message);
            }
        }

        public List<T> GetFilteredToList(Expression<Func<T, bool>> predicate)
        {
            try
            {
                List<T> query = Context.Set<T>().Where(predicate).ToList();
                return query;
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.GetFilteredToList" + e.Message);
            }
        }

        public void Load()
        {
            try
            {
                Context.Set<T>().Load();
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.Load" + e.Message);
            }
        }

        public void Remove(T entity)
        {
            try
            {
                Context.Set<T>().Attach(entity);

                Context.Set<T>().Remove(entity);
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.Remove" + e.Message);
            }
        }

        public void RemoveRange(Expression<Func<T, bool>> predicate)
        {
            try
            {
                IQueryable<T> rangeToDelete = Context.Set<T>().Where(predicate);

                Context.Set<T>().RemoveRange(rangeToDelete);
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.Remove" + e.Message);
            }
        }

        public void RemoveRange(IEnumerable<T> entitiesRange)
        {
            try
            {
                IList<T> enumerable = entitiesRange as IList<T> ?? entitiesRange.ToList();

                foreach (T entity in enumerable)
                {
                    Context.Set<T>().Attach(entity);
                }

                Context.Set<T>().RemoveRange(enumerable);
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.RemoveRange" + e.Message);
            }
        }

        public void SaveChanges()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Organiser.Data.Repositories.Repository.SaveChanges" + e.Message);
            }
        }
    }
}
