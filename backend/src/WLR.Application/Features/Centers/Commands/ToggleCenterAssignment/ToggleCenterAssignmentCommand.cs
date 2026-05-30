using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Centers.Commands.ToggleCenterAssignment;
public record ToggleCenterAssignmentCommand(Guid CenterId, bool RequiresAssignment) : IRequest<CenterDto>;
