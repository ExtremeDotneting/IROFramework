using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using IROFramework.Web.Tools.JwtAuth.Data;
using Microsoft.IdentityModel.Tokens;

namespace IROFramework.Web.Tools.JwtAuth
{
    public class JwtAuthManager : IJwtAuthManager
    {
        readonly JwtAuthManagerOptions _opt;
        readonly byte[] _secret;
        readonly TokenValidationParameters _validationParams;

        public JwtAuthManager(JwtAuthManagerOptions opt)
        {
            _opt = opt;
            _secret = Encoding.ASCII.GetBytes(_opt.Secret);
            _validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _opt.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secret),
                ValidAudience = _opt.Audience,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        }

        public AuthResult GenerateTokens(IEnumerable<Claim> claims)
        {
            var now = DateTime.UtcNow;
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_secret),
                SecurityAlgorithms.HmacSha256Signature
                );
            var claimsArr = claims as Claim[] ?? claims.ToArray();
            var shouldAddAudienceClaim =
                string.IsNullOrWhiteSpace(claimsArr?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtAccessToken = new JwtSecurityToken(
                _opt.Issuer,
                shouldAddAudienceClaim ? _opt.Audience : string.Empty,
                claimsArr,
                expires: now + _opt.AccessTokenExpiration,
                signingCredentials: signingCredentials
                );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtAccessToken);

            var origTokenStart = accessToken.Remove(10);
            var refreshTokenClaims = new Claim[]
            {
                new Claim("OrigTokenStart" ,origTokenStart)
            };
            var jwtRefreshToken = new JwtSecurityToken(
                _opt.Issuer,
                shouldAddAudienceClaim ? _opt.Audience : string.Empty,
                refreshTokenClaims,
                expires: now + _opt.RefreshTokenExpiration,
                signingCredentials: signingCredentials
                );
            var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwtRefreshToken);

            return new AuthResult()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public AuthResult Refresh(string accessToken, string refreshToken)
        {
            var accessTokenDecoded = DecodeJwtToken(accessToken);
            ValidateTokenSigning(accessTokenDecoded.JwtSecurityToken);

            var refreshTokenDecoded = DecodeJwtToken(refreshToken);
            ValidateTokenSigning(refreshTokenDecoded.JwtSecurityToken);

            var origTokenStart_fromAccToken = accessToken.Remove(7);
            var origTokenStart_fromRefToken = refreshTokenDecoded
                .Principal
                .FindFirst("OrigTokenStart")
                .Value;
            if (origTokenStart_fromAccToken != origTokenStart_fromRefToken)
            {
                throw new SecurityTokenException("Invalid token.");
            }


            return GenerateTokens(accessTokenDecoded.Principal.Claims.ToArray());
        }

        public (ClaimsPrincipal Principal, JwtSecurityToken JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token, _validationParams, out var validatedToken);
            return (principal, (JwtSecurityToken)validatedToken);
        }

        void ValidateTokenSigning(JwtSecurityToken token)
        {
            if (token == null || token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token.");
            }
        }
    }
}
