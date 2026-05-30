using WLR.Domain.Common;

namespace WLR.Domain.Entities;

public sealed class Center : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? ContactEmail { get; private set; }
    public bool IsActive { get; private set; } = true;
    /// <summary>
    /// When true (default), only assigned sewadars can operate motors.
    /// When false, any user of this center can operate any motor.
    /// </summary>
    public bool RequiresAssignment { get; private set; } = true;

    private readonly List<Location> _locations = new();
    public IReadOnlyCollection<Location> Locations => _locations.AsReadOnly();

    private Center() { }

    public static Center Create(string name, string? description, string? address, string? city, string? state, string? country, string? contactPhone, string? contactEmail)
    {
        return new Center
        {
            Name = name,
            Description = description,
            Address = address,
            City = city,
            State = state,
            Country = country,
            ContactPhone = contactPhone,
            ContactEmail = contactEmail
        };
    }

    public void Update(string name, string? description, string? address, string? city, string? state, string? country, string? contactPhone, string? contactEmail)
    {
        Name = name;
        Description = description;
        Address = address;
        City = city;
        State = state;
        Country = country;
        ContactPhone = contactPhone;
        ContactEmail = contactEmail;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRequiresAssignment(bool value)
    {
        RequiresAssignment = value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
