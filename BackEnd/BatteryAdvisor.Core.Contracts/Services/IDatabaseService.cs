using BatteryAdvisor.Core.Contracts.Models;

namespace BatteryAdvisor.Core.Contracts.Services;

public interface IDatabaseService
{
    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity with the specified identifier.</returns>
    Task<T> GetAsync<T>(Guid id) where T : class, IEntity;

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync<T>(T entity) where T : class, IEntity;

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync<T>(T entity) where T : class, IEntity;

    /// <summary>
    /// Deletes an entity from the database by its unique identifier.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync<T>(Guid id) where T : class, IEntity;
}