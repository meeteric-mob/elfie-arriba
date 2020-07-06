using System;
using System.Collections.Generic;
using System.Text;

namespace Arriba.Configuration
{
    public class OAuthConfig : IOAuthConfig
    {
        public OAuthConfig()
        {
            this.RedirectUrl = "http://localhost:42784/api/oauth/auth-code";
            this.TenantId = "c3611820-5bdd-4423-a1fc-18834a47ae78";
            this.AudienceId = "051ef594-8e5a-4156-a8ce-93fae3220779";
            this.Prompt = "login";
            this.Scopes = new[] { "openid" };
        }

        public string TenantId { get; }

        public string AudienceId { get; }

        public string RedirectUrl { get; }

        public IList<string> Scopes { get; }

        public string Prompt { get; }

        public string AppSecret { get; }
    }
}
