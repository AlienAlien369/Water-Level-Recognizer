using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Motors.Commands.UpdateMotor;

public class UpdateMotorCommandHandler : IRequestHandler<UpdateMotorCommand, MotorDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UpdateMotorCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<MotorDto> Handle(UpdateMotorCommand request, CancellationToken cancellationToken)
    {
        var motor = await _context.Motors
            .Include(m => m.Location).ThenInclude(l => l!.Center)
            .Include(m => m.AssignedSewadaar)
            .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Motor", request.Id);

        motor.Update(request.MotorNumber, request.Description, request.WaterCapacityLiters);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("UpdateMotor", "Motor", motor.Id.ToString(), cancellationToken: cancellationToken);

        return new MotorDto(motor.Id, motor.LocationId,
            motor.Location?.Name ?? "", motor.Location?.CenterId ?? Guid.Empty,
            motor.Location?.Center?.Name ?? "",
            motor.MotorNumber, motor.Description, motor.WaterCapacityLiters,
            motor.Status, motor.CurrentState, motor.LastOpenedAt, motor.LastClosedAt,
            motor.TotalRunningMinutes, motor.IsActive,
            motor.AssignedSewadaarId, motor.AssignedSewadaar?.Name, motor.CreatedAt);
    }
}
