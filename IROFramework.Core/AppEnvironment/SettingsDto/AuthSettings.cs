namespace IROFramework.Core.AppEnvironment.SettingsDto
{
    public class AuthSettings
    {
        public string Secret { get; set; }

        public int AccessTokenExpirationMinutes { get; set; }

        public int RefreshTokenExpirationMinutes { get; set; }
    }

}