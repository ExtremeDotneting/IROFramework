using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IROFramework.Core.Consts;
using IROFramework.Core.Models;
using IROFramework.Core.Tools;
using IROFramework.Core.Tools.AbstractDatabase;
using IROFramework.Web.Models;
using IROFramework.Web.Tools.JwtAuth;
using IROFramework.Web.Tools.JwtAuth.Data;

namespace IROFramework.Web.Services.Auth
{
    public class UserAuthService : IUserAuthService
    {
        readonly IDatabaseSet<UserModel, Guid> _dbSet;

        readonly IJwtAuthManager _authManager;

        public UserAuthService(AbstractDbContext db, IJwtAuthManager authManager)
        {
            _authManager = authManager;
            _dbSet = db.Users;
        }

        public async Task<AuthResult> RefreshToken(string accessToken, string refreshToken)
        {
            return _authManager.Refresh(accessToken, refreshToken);
        }

        public async Task<AuthResult> Login(string nick, string pass)
        {
            nick = nick.Trim();
            pass = pass.Trim();
            var user = await _dbSet.GetByPropertyAsync(r => r.Nickname, nick);
            if (!HashExtensions.Compare(pass.Trim(), user.PasswordHash))
            {
                throw new Exception("Password doesn't match.");
            }
            return GenerateToken(user);
        }

        public async Task<AuthResult> Register(string nick, string pass)
        {
            if (nick == null) throw new ArgumentNullException(nameof(nick));
            if (pass == null) throw new ArgumentNullException(nameof(pass));
            nick = nick.Trim();
            pass = pass.Trim();
            if (pass.Length < 4)
            {
                throw new Exception("Password length must be 4+.");
            }
            var newUser = new UserModel()
            {
                Id = Guid.NewGuid(),
                Nickname = nick,
                Role = UserRoles.BasicUser,
                PasswordHash = HashExtensions.HashString(pass)
            };
            return await Register(newUser);
        }

        public async Task<UserModel> ChangeRole(UserModel user, string role)
        {
            user.Role = role;
            await _dbSet.UpdateAsync(user);
            return user;
        }

        public async Task<UserModel> GetByToken(string accessToken)
        {
            var decoded = _authManager.DecodeJwtToken(accessToken);
            var userIdStr = decoded.Principal.FindFirstValue(nameof(UserModel.Id));
            var guid = Guid.Parse(userIdStr);
            return await _dbSet.GetByIdAsync(guid);
        }

        /// <summary>
        /// Nickname, pass hash, role and id must be set.
        /// Nickname must be unique.
        /// </summary>
        async Task<AuthResult> Register(UserModel newUser)
        {
            if (newUser.Nickname == null)
            {
                throw new NullReferenceException("New user nickname is null.");
            }
            if (newUser.PasswordHash == null)
            {
                throw new NullReferenceException("New user pass hash is null.");
            }

            var user = await _dbSet.TryGetByPropertyAsync(r => r.Nickname, newUser.Nickname);
            if (user != null)
            {
                throw new Exception("User with this nickname exists in db.");
            }
            await _dbSet.InsertAsync(newUser);
            return GenerateToken(newUser);
        }
        
        public AuthResult GenerateToken(UserModel userModel)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Role,
                    userModel.Role
                    ),
                new Claim(
                    nameof(UserModel.Id),
                    userModel.Id.ToString()
                    )
            };
            return _authManager.GenerateTokens(claims);
        }

    }
}
