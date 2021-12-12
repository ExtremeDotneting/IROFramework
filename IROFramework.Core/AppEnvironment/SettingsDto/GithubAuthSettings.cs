using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IROFramework.Core.AppEnvironment.SettingsDto
{
    public class GithubAuthSettings
    {
        public bool Enabled { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AppName { get; set; }

        public List<string> Scopes { get; set; }
    }
}