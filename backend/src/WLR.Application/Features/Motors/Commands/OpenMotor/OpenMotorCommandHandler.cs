using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;

namespace WLR.Application.Features.Motors.Commands.OpenMotor;

public class OpenMotorCommandHandler : IRequestHandler<OpenMotorCommand, MotorLogDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ISignalRService _signalR;
    private readonly IAuditService _auditService;

    public OpenMotorCommandHandler(IApplicationDbContext context, ICurrentUser currentUser, ISignalRService signalR, IAuditService auditService)
    {
        _context = context;
        _currentUser = currentUser;
        _signalR = signalR;
        _auditService = auditService;
    }

    public async Task<MotorLogDto> Handle(OpenMotorCommand request, CancellationToken cancellationToken)
    {
        var motor = await _context.Motors
            .Include(m => m.Location)
            .Include(m => m.AssignedSewadaar)
            .FirstOrDefaultAsync(m => m.Id == request.MotorId && !m.IsDeleted, cancellationToken);

        if (motor == null) throw new NotFoundException("Motor", request.MotorId);

        if (_currentUser.Role == Domain.Enums.UserRole.User &&
            motor.AssignedSewadaarId != _currentUser.UserId)
            throw new ForbiddenException("You are not assigned to this motor.");

        var userId = _currentUser.UserId ?? throw new ForbiddenException();
        var log = motor.Open(userId, request.Notes);
        _context.MotorLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);

        await _signalR.SendMotorStatusUpdateAsync(motor.Id, new { motorId = motor.Id, state = motor.CurrentState, status = motor.Status }, cancellationToken);
        await _auditService.LogAsync("OpenMotor", "Motor", motor.Id.ToString(), newValues: new { motor.CurrentState }, cancellationToken: cancellationToken);

        return new MotorLogDto(log.Id, log.MotorId, motor.MotorNumber, log.OperatedByUserId, _currentUser.UserName ?? "", log.Action, log.ActionTime, log.DurationMinutes, log.Notes);
    }
}
