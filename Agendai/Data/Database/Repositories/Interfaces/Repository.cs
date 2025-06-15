using Agendai.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agendai.Data.Database.Repositories.Interfaces;

class Repository<T> : IRepository<T>
    where T : Entity
{
    public Task<T?> AddAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public virtual Task<T?> DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public virtual Task<bool> ExistsAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>?> FindAsync(Predicate<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public virtual Task<IEnumerable<T>?> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetByIdAsync(ulong id)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<T?> UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }
}
