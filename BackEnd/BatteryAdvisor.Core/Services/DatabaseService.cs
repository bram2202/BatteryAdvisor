using BatteryAdvisor.Core.Contracts.Models;
using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace BatteryAdvisor.Core.Services;

public class DatabaseService : IDatabaseService
{
    private readonly BatteryAdvisorContext _context;

    public DatabaseService(BatteryAdvisorContext context)
    {
        _context = context;
    }

    public async Task AddAsync<T>(T entity) where T : class, IEntity
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<T> GetAsync<T>(Guid id) where T : class, IEntity
    {
        var entity = await _context.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
        return entity ?? throw new KeyNotFoundException($"{typeof(T).Name} with id '{id}' was not found.");
    }

    public async Task UpdateAsync<T>(T entity) where T : class, IEntity
    {
        var exists = await _context.Set<T>().AnyAsync(e => e.Id == entity.Id);
        if (!exists)
        {
            throw new KeyNotFoundException($"{typeof(T).Name} with id '{entity.Id}' was not found.");
        }

        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync<T>(Guid id) where T : class, IEntity
    {
        var entity = await _context.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
        if (entity is null)
        {
            throw new KeyNotFoundException($"{typeof(T).Name} with id '{id}' was not found.");
        }

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}
