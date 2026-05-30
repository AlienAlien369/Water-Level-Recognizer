using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Users.Queries.GetUserById;
public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
