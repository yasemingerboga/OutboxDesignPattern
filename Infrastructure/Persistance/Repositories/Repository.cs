using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly OrderDbContext _context;
        public Repository(OrderDbContext context)
        {
            _context = context;
        }
        public DbSet<T> Table { get => _context.Set<T>(); }
        public async Task AddAsync(T model)
            => await _context.AddAsync(model);

        public async Task<T> AddAsyncT(T model)
        {
            await _context.AddAsync(model);
            return model;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> method)
        => await Table.Where(method).SingleOrDefaultAsync();

        public async Task<List<T>> GetAll() => await Table.ToListAsync();

        public async Task<List<T>> GetWhere(System.Linq.Expressions.Expression<Func<T, bool>> method)
            => await Table.Where(method).ToListAsync();

        public async Task SaveChangesAsync()
         => await _context.SaveChangesAsync();

        public async Task<T> UpdateAsync(T model)
        {
            _context.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public T Get(Expression<Func<T, bool>> method)
        => Table.Where(method).FirstOrDefault();
    }
}
