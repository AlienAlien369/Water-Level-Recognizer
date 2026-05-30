using WLR.Domain.Entities;
using WLR.Domain.Enums;
using FluentAssertions;

namespace WLR.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Create_ShouldInitializeWithUserRole()
    {
        var user = User.Create("John Doe", "+919876543210", null);
        user.Name.Should().Be("John Doe");
        user.MobileNumber.Should().Be("+919876543210");
        user.Role.Should().Be(UserRole.User);
        user.IsActive.Should().BeTrue();
        user.IsLocked.Should().BeFalse();
    }

    [Fact]
    public void RecordFailedLogin_FiveTimes_ShouldLockAccount()
    {
        var user = User.Create("Jane", "+911234567890", null);
        for (int i = 0; i < 5; i++) user.RecordFailedLogin();
        user.IsLocked.Should().BeTrue();
        user.LockedUntil.Should().NotBeNull();
        user.DomainEvents.Should().ContainSingle(e => e is WLR.Domain.Events.UserLockedEvent);
    }

    [Fact]
    public void PromoteToAdmin_ShouldChangeRole()
    {
        var user = User.Create("Sam", "+910000000001", null);
        var centerId = Guid.NewGuid();
        user.PromoteToAdmin(centerId);
        user.Role.Should().Be(UserRole.Admin);
        user.CenterId.Should().Be(centerId);
        user.DomainEvents.Should().ContainSingle(e => e is WLR.Domain.Events.UserRoleChangedEvent);
    }

    [Fact]
    public void CanLogin_WhenLocked_ShouldReturnFalse()
    {
        var user = User.Create("Locked", "+910000000002", null);
        for (int i = 0; i < 5; i++) user.RecordFailedLogin();
        user.CanLogin().Should().BeFalse();
    }

    [Fact]
    public void CanLogin_WhenInactive_ShouldReturnFalse()
    {
        var user = User.Create("Inactive", "+910000000003", null);
        user.Deactivate();
        user.CanLogin().Should().BeFalse();
    }
}
