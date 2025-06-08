
using Agendai.Data.Models;
using Agendai.Data.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace Agendai.Data.Repositories.DesignTime;

public abstract class DesignTimeRepository<T> : IRepository<T> where T : Entity
{
    public abstract Task<T?> Read(int id);
    public abstract Task<IEnumerable<T>?> ReadByName(string name);
    public abstract Task<IEnumerable<T>?> ReadAll();
    public abstract Task<T?> Create(T entity);
    public abstract Task<T?> Update(T entity);
    public abstract Task<int?> Delete(int id);
    public abstract Task<bool> Exists(T entity);
    public abstract Task<IEnumerable<T>?> Search(Expression<Func<T, bool>> predicate);
}
