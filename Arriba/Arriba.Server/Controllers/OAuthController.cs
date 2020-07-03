using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Arriba.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {

        [HttpGet]
        public IActionResult SignIn()
        {
            const string redirect = "http://localhost:8080";
            const string tenant = "c3611820-5bdd-4423-a1fc-18834a47ae78";
            const string appId = "051ef594-8e5a-4156-a8ce-93fae3220779";            
            return this.Redirect($"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize?client_id={appId}&response_type=id_token&redirect_uri={redirect}&scope=openid&response_mode=fragment&state=12345&nonce=678910");
        }

    }
}
