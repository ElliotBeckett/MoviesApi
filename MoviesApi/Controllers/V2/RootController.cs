using Microsoft.AspNetCore.Mvc;
using MoviesApi.DTO;
using MoviesApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers.V2
{
    [ApiController]
    [Route("api/v2")]
    // [HttpHeaderIsPresent("x-version", "2")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "getRoot")]
        public ActionResult<IEnumerable<Link>> Get()
        {
            List<Link> links = new List<Link>();

            links.Add(new Link(href: Url.Link("getRoot", new { }), rel: "self", method: "GET"));

            return links;
        }
    }
}