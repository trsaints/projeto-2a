using Agendai.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agendai.Data.Repositories.Interfaces;

public interface IRepository<T> where T : Entity
{
    public Task<T?> Read(int id);
    public Task<IEnumerable<T>?> ReadByName(string name);
    public Task<IEnumerable<T>?> ReadAll();
    public Task<T?> Create(T entity);
    public Task<T?> Update(T entity);
    public Task<int?> Delete(int id);
    public Task<bool> Exists(T entity);
    public Task<IEnumerable<T>?> Search(Expression<Func<T, bool>> predicate);
}
