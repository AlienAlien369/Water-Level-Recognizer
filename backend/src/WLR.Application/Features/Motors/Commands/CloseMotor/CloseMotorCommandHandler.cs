using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;

namespace WLR.Application.Features.Motors.Commands.CloseMotor;

public class CloseMotorCommandHandler : IRequestHandler<CloseMotorCommand, MotorLogDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ISignalRService _signalR;
    private readonly IAuditService _auditService;

    public CloseMotorCommandHandler(IApplicationDbContext context, ICurrentUser currentUser, ISignalRService signalR, IAuditService auditService)
    {
        _context = context;
        _currentUser = currentUser;
        _signalR = signalR;
        _auditService = auditService;
    }

    public async Task<MotorLogDto> Handle(CloseMotorCommand request, CancellationToken cancellationToken)
    {
        var motor = await _context.Motors
            .Include(m => m.Location).ThenInclude(l => l!.Center)
            .FirstOrDefaultAsync(m => m.Id == request.MotorId && !m.IsDeleted, cancellationToken);

        if (motor == null) throw new NotFoundException("Motor", request.MotorId);

        if (_currentUser.Role == Domain.Enums.UserRole.User)
        {
            var centerRequiresAssignment = motor.Location?.Center?.RequiresAssignment ?? true;
            var userBelongsToCenter = motor.Location?.CenterId == _currentUser.CenterId;

            if (centerRequiresAssignment)
            {
                if (motor.AssignedSewadaarId != _currentUser.UserId)
                    throw new ForbiddenException("You are not assigned to this motor.");
            }
            else if (!userBelongsToCenter)
            {
                throw new ForbiddenException("You do not belong to this center.");
            }
        }

        var userId = _currentUser.UserId ?? throw new ForbiddenException();
        var log = motor.Close(userId, Guid.Empty, request.Notes);
        _context.MotorLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);

        await _signalR.SendMotorStatusUpdateAsync(motor.Id, new { motorId = motor.Id, state = motor.CurrentState, status = motor.Status, durationMinutes = log.DurationMinutes }, cancellationToken);
        await _auditService.LogAsync("CloseMotor", "Motor", motor.Id.ToString(), newValues: new { motor.CurrentState, log.DurationMinutes }, cancellationToken: cancellationToken);

        return new MotorLogDto(log.Id, log.MotorId, motor.MotorNumber, log.OperatedByUserId, _currentUser.UserName ?? "", log.Action, log.ActionTime, log.DurationMinutes, log.Notes);
    }
}
