using WLR.Domain.Entities;
namespace WLR.Application.Common.Interfaces;
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid? ValidateTokenAndGetUserId(string token);
}
