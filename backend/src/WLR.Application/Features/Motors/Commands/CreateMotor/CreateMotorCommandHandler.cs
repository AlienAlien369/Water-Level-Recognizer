using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Entities;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Motors.Commands.CreateMotor;

public class CreateMotorCommandHandler : IRequestHandler<CreateMotorCommand, MotorDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public CreateMotorCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<MotorDto> Handle(CreateMotorCommand request, CancellationToken cancellationToken)
    {
        var location = await _context.Locations
            .Include(l => l.Center)
            .FirstOrDefaultAsync(l => l.Id == request.LocationId && !l.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Location", request.LocationId);

        var motor = Motor.Create(request.LocationId, request.MotorNumber, request.Description, request.WaterCapacityLiters);
        _context.Motors.Add(motor);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("CreateMotor", "Motor", motor.Id.ToString(), newValues: new { motor.MotorNumber }, cancellationToken: cancellationToken);

        return new MotorDto(motor.Id, motor.LocationId, location.Name,
            location.CenterId, location.Center?.Name ?? "",
            motor.MotorNumber, motor.Description, motor.WaterCapacityLiters,
            motor.Status, motor.CurrentState, motor.LastOpenedAt, motor.LastClosedAt,
            motor.TotalRunningMinutes, motor.IsActive,
            motor.AssignedSewadaarId, null, motor.CreatedAt);
    }
}
