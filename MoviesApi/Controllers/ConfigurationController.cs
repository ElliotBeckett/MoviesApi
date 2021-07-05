using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    /*
    [ApiController]
    [Route("api/configuration")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_configuration["LastName"]);
            //return Ok(_configuration["ConnectionStrings:DefaultConnection"]); // A colon can be used to access a child field of a configuration setting
        }
    }
    */
}