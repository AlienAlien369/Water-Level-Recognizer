using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Entities;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Centers.Commands.CreateCenter;

public class CreateCenterCommandHandler : IRequestHandler<CreateCenterCommand, CenterDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public CreateCenterCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<CenterDto> Handle(CreateCenterCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Centers.AnyAsync(c => c.Name == request.Name && !c.IsDeleted, cancellationToken);
        if (exists)
            throw new ConflictException($"A center named '{request.Name}' already exists.");

        var center = Center.Create(request.Name, request.Description, request.Address, request.City, request.State, request.Country, request.ContactPhone, request.ContactEmail);
        _context.Centers.Add(center);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("CreateCenter", "Center", center.Id.ToString(), newValues: new { center.Name }, cancellationToken: cancellationToken);

        return new CenterDto(center.Id, center.Name, center.Description, center.Address, center.City, center.State, center.Country,
            center.ContactPhone, center.ContactEmail, center.IsActive, 0, 0, center.CreatedAt, center.RequiresAssignment);
    }
}
