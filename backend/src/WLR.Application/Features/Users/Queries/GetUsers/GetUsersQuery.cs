using MediatR;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Users.Queries.GetUsers;
public record GetUsersQuery(QueryParams Params, Guid? CenterId) : IRequest<PaginatedResult<UserDto>>;
