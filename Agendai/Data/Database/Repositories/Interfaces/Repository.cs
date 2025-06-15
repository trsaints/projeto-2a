using Agendai.Data.Database.Context;
using Agendai.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public async Task<IEnumerable<T>?> FindAsync(Expression<Func<T, bool>> callback)
    {
        try
        {
            return await _data.Where(callback).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error finding entities: {ex.Message}");

            return null;
        }
    }

    public virtual async Task<IEnumerable<T>?> GetAllAsync()
    {
        try
        {
            return await _data.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting all entities: {ex.Message}");

            return null;
        }
    }

    public async Task<T?> GetByIdAsync(ulong id)
    {
        try
        {
            return await _data.FindAsync(id);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting entity by ID: {ex.Message}");

            return null;
        }
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
