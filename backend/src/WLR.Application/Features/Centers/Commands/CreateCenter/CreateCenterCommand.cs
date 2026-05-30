using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Centers.Commands.CreateCenter;
public record CreateCenterCommand(
    string Name,
    string? Description,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? ContactPhone,
    string? ContactEmail
) : IRequest<CenterDto>;
