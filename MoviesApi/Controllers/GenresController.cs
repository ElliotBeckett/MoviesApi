using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using MoviesApi.Entities;
using MoviesApi.Filters;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/genres")] // Route for the URL to take when accessing the API
    [ApiController] // Automatic Checking for request errors
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Adding in authorization
    public class GenresController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger<GenresController> _logger;

        public GenresController(IRepository repository, ILogger<GenresController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // You can use multiple routes for one method
        [HttpGet]
        [HttpGet("list")] // This will add to the default route - so it becomes api/genres/list
        [HttpGet("/allgenres")] // This will override the default route - so it becomes api/allgenres
        [ResponseCache(Duration = 60)] // Caches the data for 60 seconds
        [ServiceFilter(typeof(MyActionFilter))] // Custom filter
        public async Task<ActionResult<List<Genre>>> Get()
        {
            _logger.LogInformation("Getting all the genres");
            return await _repository.GetAllGenres();
        }

        [HttpGet("{id:int}", Name = "getGenre")] // Added a route contraint
        // [HttpGet("{id:int}/{param2=Elliot}")] // Routes can also have multiple paramaters and can be set with a default value
        public ActionResult<Genre> Get(int id, string param2)
        {
            _logger.LogDebug($"Get by ID method executing using ID {id}");

            var genre = _repository.GetGenreByID(id);

            if (genre == null)
            {
                _logger.LogWarning($"Genre with ID {id} not found");
                return NotFound();
            }
            return genre;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre genre)
        {
            _repository.AddGenre(genre);
            return new CreatedAtRouteResult("getGenre", new { id = genre.ID }, genre);
        }

        [HttpPut]
        public ActionResult Put([FromBody] Genre genre)
        {
            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();
        }
    }
}