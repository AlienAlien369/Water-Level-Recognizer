using WLR.Domain.Common;
using WLR.Domain.Enums;
using WLR.Domain.Events;

namespace WLR.Domain.Entities;

public sealed class User : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string MobileNumber { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? PasswordHash { get; private set; }
    public UserRole Role { get; private set; } = UserRole.User;
    public bool IsActive { get; private set; } = true;
    public bool IsLocked { get; private set; } = false;
    public int FailedLoginAttempts { get; private set; } = 0;
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public string? ProfileImageUrl { get; private set; }
    public Guid? CenterId { get; private set; }

    private readonly List<Assignment> _assignments = new();
    public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();

    private readonly List<UserSession> _sessions = new();
    public IReadOnlyCollection<UserSession> Sessions => _sessions.AsReadOnly();

    private User() { }

    public static User Create(string name, string mobileNumber, string? email, UserRole role = UserRole.User, Guid? centerId = null)
    {
        var user = new User
        {
            Name = name,
            MobileNumber = mobileNumber,
            Email = email,
            Role = role,
            CenterId = centerId
        };
        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.MobileNumber, user.Role));
        return user;
    }

    public void SetPassword(string passwordHash) => PasswordHash = passwordHash;

    public void UpdateProfile(string name, string? email, string? profileImageUrl)
    {
        Name = name;
        Email = email;
        ProfileImageUrl = profileImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void PromoteToAdmin(Guid centerId)
    {
        Role = UserRole.Admin;
        CenterId = centerId;
        AddDomainEvent(new UserRoleChangedEvent(Id, UserRole.User, UserRole.Admin));
    }

    public void DemoteToUser()
    {
        Role = UserRole.User;
        CenterId = null;
        AddDomainEvent(new UserRoleChangedEvent(Id, UserRole.Admin, UserRole.User));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        IsLocked = false;
        LockedUntil = null;
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            IsLocked = true;
            LockedUntil = DateTime.UtcNow.AddMinutes(30);
            AddDomainEvent(new UserLockedEvent(Id, LockedUntil.Value));
        }
    }

    public void Unlock()
    {
        IsLocked = false;
        LockedUntil = null;
        FailedLoginAttempts = 0;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public bool CanLogin() => IsActive && (!IsLocked || (LockedUntil.HasValue && LockedUntil.Value < DateTime.UtcNow));
}
