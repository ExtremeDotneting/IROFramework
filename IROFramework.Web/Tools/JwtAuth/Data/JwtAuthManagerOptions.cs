using System;

namespace IROFramework.Web.Tools.JwtAuth.Data
{
    public class JwtAuthManagerOptions
    {
        public string Secret { get; set; }


        public string Issuer { get; set; }


        public string Audience { get; set; }


        public TimeSpan AccessTokenExpiration { get; set; }


        public TimeSpan RefreshTokenExpiration { get; set; }
    }
}
