using Agendai.Data.Database.Context;
using Agendai.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agendai.Data.Database.Repositories.Interfaces;

public class Repository<T> : IRepository<T> where T : Entity
{
    private readonly AppDbContext _db;
    private readonly DbSet<T> _data;

    public Repository(AppDbContext db)
    {
        _db = db;
        _data = db.Set<T>();
    }

    public async Task<T?> AddAsync(T entity)
    {
        try
        {
            _data.Add(entity);

            await _db.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error adding entity: {ex.Message}");

            return null;
        }
    }

    public virtual async Task<T?> DeleteAsync(T entity)
    {
        try
        {
            var existingEntity = await _data.FindAsync(entity.Id);

            if (existingEntity is null) return entity;

            _data.Remove(entity);

            await _db.SaveChangesAsync();

            return null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting entity: {ex.Message}");

            return entity;
        }
    }

    public virtual async Task<bool> ExistsAsync(T entity)
    {
        try
        {
            return await _data.AnyAsync(e => e.Id == entity.Id);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error checking existence of entity: {ex.Message}");

            return false;
        }
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
