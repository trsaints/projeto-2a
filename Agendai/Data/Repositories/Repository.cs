using Agendai.Data.Database;
using Agendai.Data.Models;
using Agendai.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Agendai.Data.Repositories;

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly DbSet<T> Data;
    protected readonly AppDbContext dbContext;

    protected Repository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
        Data = dbContext.Set<T>();
    }

    public virtual async Task<T?> Create(T entity)
    {
        try
        {
            var addResult = await Data.AddAsync(entity);

            if (addResult is null || addResult.State is not EntityState.Added)
            {
                return null;
            }

            await dbContext.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating entity: {ex.Message}");

            return null;
        }
    }

    public async Task<int?> Delete(int id)
    {
        try
        {
            var existingEntity = await Data.FindAsync(id);

            if (existingEntity is null)
            {
                return null;
            }

            Data.Remove(existingEntity);

            await dbContext.SaveChangesAsync();

            return null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting entity with ID {id}: {ex.Message}");

            return id;
        }
    }

    public virtual async Task<bool> Exists(T entity)
    {
        try
        {
            return await Data.AnyAsync(e => e.Id == entity.Id || e.Name == entity.Name);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error checking existence of entity: {ex.Message}");

            return false;
        }
    }

    public async Task<T?> Read(int id)
    {
        try
        {
            return await Data.FindAsync(id);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading entity with ID {id}: {ex.Message}");

            return null;
        }
    }

    public async Task<IEnumerable<T>?> ReadAll()
    {
        try
        {
            return await Data.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading all entities: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<T>?> ReadByName(string name)
    {
        try
        {
            return await Data.Where(e => e.Name == name).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading entity with Name '{name}': {ex.Message}");

            return null;
        }
    }

    public async Task<IEnumerable<T>?> Search(Expression<Func<T, bool>> predicate)
    {
        try
        {
            return await Data.Where(predicate).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error searching entities: {ex.Message}");

            return null;
        }
    }

    public async Task<T?> Update(T entity)
    {
        try
        {
            var entityExists = await Exists(entity);

            if (!entityExists) return null;

            var updateResult = Data.Update(entity);

            if (updateResult is null || updateResult.State is not EntityState.Modified)
            {
                return null;
            }

            await dbContext.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating entity: {ex.Message}");

            return null;
        }
    }
}
