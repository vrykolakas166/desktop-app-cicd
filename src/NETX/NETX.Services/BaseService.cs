using Microsoft.EntityFrameworkCore;
using NETX.Core;
using NETX.Services.Interfaces;
using System.Collections.ObjectModel;

namespace NETX.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly NXDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseService(NXDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public T Add(T entity)
        {
            var added = _dbSet.Add(entity);
            _context.SaveChanges();
            return added.Entity;
        }

        public bool Contains(T entity)
        {
            return _dbSet.Contains(entity);
        }

        public async Task<T> AddAsync(T entity)
        {
            var added = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return added.Entity;
        }

        public T? Get(string id)
        {
            return _dbSet.Find(id);
        }

        public ObservableCollection<T> GetAll()
        {
            var result = new ObservableCollection<T>();
            foreach (var entity in _dbSet)
            {
                result.Add(entity);
            }
            return result;
        }

        public void Remove(string id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
        }

        public async Task RemoveAsync(string id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public T Update(T entity)
        {
            var upd = _dbSet.Update(entity);
            _context.SaveChanges();
            return upd.Entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var upd = _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return upd.Entity;
        }
    }
}
