using Arriba.Communication.Server.Application;
using Arriba.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arriba.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArribaController : ControllerBase
    {
       private readonly IArribaManagementService _arribaManagement;

        public ArribaController(IArribaManagementService arribaManagement)
        {
            _arribaManagement = arribaManagement;
        }

        [HttpGet]
        public IActionResult GetTables ()
        {
            return Ok(_arribaManagement.GetTables());
        }

        [HttpGet("allBasics")]
        public IActionResult GetAllBasics()
        {
            return Ok(_arribaManagement.GetAllBasic());
        }

    }

}
