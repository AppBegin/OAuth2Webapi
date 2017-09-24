using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace auth
{
    [Route("/")]
    public class HomeController : ControllerBase
    {
        public IActionResult Home()
        {
        	return Redirect("https://github.com/AppBegin/OAuth2Webapi");
        }
    }
}
