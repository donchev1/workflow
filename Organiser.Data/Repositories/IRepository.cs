﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        void Load();
        IQueryable<T> GetAllToIQuerable();
        List<T> GetAllToList();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        List<T> GetFilteredToList(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entitiesRange);
        void Remove(T entity);
        void RemoveRange(Expression<Func<T, bool>> predicate);
        void RemoveRange(IEnumerable<T> entitiesRange);
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
