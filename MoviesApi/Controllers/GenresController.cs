using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesApi.DTO;
using MoviesApi.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/genres")] // Route for the URL to take when accessing the API
    [ApiController] // Automatic Checking for request errors
    public class GenresController : ControllerBase
    {
        private readonly ILogger<GenresController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GenresController
            (
                ApplicationDbContext context,
                IMapper mapper,
                ILogger<GenresController> logger
            )
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Sets this end point to require Authorization before use
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var genre = await _context.Genres.AsNoTracking().ToListAsync();
            var genreDTOs = _mapper.Map<List<GenreDTO>>(genre);

            return genreDTOs;
        }

        [HttpGet("{id:int}", Name = "getGenre")]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(x => x.ID == id);

            if (genre == null)
            {
                return NotFound();
            }

            var genreDTO = _mapper.Map<GenreDTO>(genre);
            return genreDTO;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")] // Sets this end point to require Authorization before use and only accessible by the Admin role
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreation)
        {
            var genre = _mapper.Map<Genre>(genreCreation);

            _context.Add(genre);
            await _context.SaveChangesAsync();

            var genreDTO = _mapper.Map<GenreDTO>(genre);

            return new CreatedAtRouteResult("getGenre", new { genreDTO.ID }, genreDTO);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreation)
        {
            var genre = _mapper.Map<Genre>(genreCreation);
            genre.ID = id;

            _context.Entry(genre).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Genres.AnyAsync(x => x.ID == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Genre() { ID = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}