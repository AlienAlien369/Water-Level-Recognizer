using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Motors.Commands.DeleteMotor;

public class DeleteMotorCommandHandler : IRequestHandler<DeleteMotorCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditService _auditService;

    public DeleteMotorCommandHandler(IApplicationDbContext context, ICurrentUser currentUser, IAuditService auditService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeleteMotorCommand request, CancellationToken cancellationToken)
    {
        var motor = await _context.Motors.FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Motor", request.Id);

        motor.SoftDelete(_currentUser.UserId?.ToString() ?? "system");
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("DeleteMotor", "Motor", motor.Id.ToString(), cancellationToken: cancellationToken);
        return true;
    }
}
