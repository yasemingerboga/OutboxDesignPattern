using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAll();
        Task<List<T>> GetWhere(Expression<Func<T, bool>> method);
        Task AddAsync(T model);
        Task<T> UpdateAsync(T model);
        Task SaveChangesAsync();
        Task<T> AddAsyncT(T model);
        Task<T> GetAsync(Expression<Func<T, bool>> method);
        T Get(Expression<Func<T, bool>> method);
    }
}
