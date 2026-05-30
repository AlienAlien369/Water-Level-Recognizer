using WLR.Domain.Common;

namespace WLR.Domain.Entities;

public sealed class Location : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Floor { get; private set; }
    public string? Zone { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid CenterId { get; private set; }
    public Center? Center { get; private set; }

    private readonly List<Motor> _motors = new();
    public IReadOnlyCollection<Motor> Motors => _motors.AsReadOnly();

    private readonly List<Assignment> _assignments = new();
    public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();

    private Location() { }

    public static Location Create(Guid centerId, string name, string? description, string? floor, string? zone)
    {
        return new Location
        {
            CenterId = centerId,
            Name = name,
            Description = description,
            Floor = floor,
            Zone = zone
        };
    }

    public void Update(string name, string? description, string? floor, string? zone)
    {
        Name = name;
        Description = description;
        Floor = floor;
        Zone = zone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
