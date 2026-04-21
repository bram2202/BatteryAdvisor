using System.ComponentModel.DataAnnotations;
using BatteryAdvisor.Core.Contracts.Models;

namespace BatteryAdvisor.Core.Models.Database;

/// <summary>
/// Abstract base class for all entities in the database, providing a unique identifier and common properties.
/// </summary>
public abstract class AbstractEntity : IEntity
{
    [Required]
    public required Guid Id { get; set; }
}
