using Agendai.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agendai.Data.Database.Repositories.Interfaces;

public interface IRepository<T> where T : Entity
{
    public Task<T?> GetByIdAsync(ulong id);
    public Task<T?> GetByNameAsync(string name);
    public Task<IEnumerable<T>?> GetAllAsync();
    public Task<IEnumerable<T>?> FindAsync(Predicate<Func<T, bool>> predicate);
    public Task<T?> AddAsync(T entity);
    public Task<T?> UpdateAsync(T entity);
    public Task<T?> DeleteAsync(T entity);
    public Task<bool> ExistsAsync(T entity);
}
