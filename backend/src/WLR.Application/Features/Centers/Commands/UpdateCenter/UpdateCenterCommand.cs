using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Centers.Commands.UpdateCenter;
public record UpdateCenterCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? ContactPhone,
    string? ContactEmail
) : IRequest<CenterDto>;
