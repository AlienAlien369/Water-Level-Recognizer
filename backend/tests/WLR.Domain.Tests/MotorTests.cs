using WLR.Domain.Entities;
using WLR.Domain.Enums;
using FluentAssertions;

namespace WLR.Domain.Tests;

public class MotorTests
{
    private static Motor CreateMotor()
        => Motor.Create(Guid.NewGuid(), "M-001", "Test motor", 1000m);

    [Fact]
    public void Create_ShouldInitializeWithCorrectDefaults()
    {
        var motor = CreateMotor();
        motor.MotorNumber.Should().Be("M-001");
        motor.WaterCapacityLiters.Should().Be(1000m);
        motor.Status.Should().Be(MotorStatus.Inactive);
        motor.CurrentState.Should().Be(MotorState.Off);
        motor.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Open_ShouldChangeStateToOn()
    {
        var motor = CreateMotor();
        motor.Activate();
        var log = motor.Open(Guid.NewGuid());
        motor.CurrentState.Should().Be(MotorState.On);
        motor.Status.Should().Be(MotorStatus.Running);
        log.Action.Should().Be("Open");
        motor.DomainEvents.Should().ContainSingle(e => e is WLR.Domain.Events.MotorOpenedEvent);
    }

    [Fact]
    public void Open_WhenAlreadyOn_ShouldThrow()
    {
        var motor = CreateMotor();
        motor.Activate();
        motor.Open(Guid.NewGuid());
        Action act = () => motor.Open(Guid.NewGuid());
        act.Should().Throw<InvalidOperationException>().WithMessage("*already running*");
    }

    [Fact]
    public void Close_ShouldChangeStateToOff_AndCalculateDuration()
    {
        var motor = CreateMotor();
        motor.Activate();
        motor.Open(Guid.NewGuid());
        var log = motor.Close(Guid.NewGuid(), Guid.Empty);
        motor.CurrentState.Should().Be(MotorState.Off);
        motor.Status.Should().Be(MotorStatus.Active);
        log.Action.Should().Be("Close");
        log.DurationMinutes.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Close_WhenNotRunning_ShouldThrow()
    {
        var motor = CreateMotor();
        motor.Activate();
        Action act = () => motor.Close(Guid.NewGuid(), Guid.Empty);
        act.Should().Throw<InvalidOperationException>().WithMessage("*already stopped*");
    }

    [Fact]
    public void AssignSewadaar_ShouldSetAssignedId()
    {
        var motor = CreateMotor();
        var userId = Guid.NewGuid();
        motor.AssignSewadaar(userId);
        motor.AssignedSewadaarId.Should().Be(userId);
    }

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        var motor = CreateMotor();
        motor.SoftDelete("admin-id");
        motor.IsDeleted.Should().BeTrue();
        motor.DeletedAt.Should().NotBeNull();
        motor.DeletedBy.Should().Be("admin-id");
    }
}
