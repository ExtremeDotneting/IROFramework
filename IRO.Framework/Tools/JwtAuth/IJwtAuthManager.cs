using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IROFramework.Web.Tools.JwtAuth.Data;

namespace IROFramework.Web.Tools.JwtAuth
{
    public interface IJwtAuthManager
    {
        AuthResult GenerateTokens(IEnumerable<Claim> claims);
        AuthResult Refresh(string accessToken, string refreshToken);
        (ClaimsPrincipal Principal, JwtSecurityToken JwtSecurityToken) DecodeJwtToken(string token);
    }
}