using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTO;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;
        private readonly ILogger<MoviesController> _logger;
        private readonly string containerName = "movies";

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorage, ILogger<MoviesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _logger = logger;
        }

        [HttpGet] // Gets the upcoming and currently released movies
        public async Task<ActionResult<IndexMoviePageDTO>> Get()
        {
            var top = 6;
            var today = DateTime.Today;
            var upcomingReleases = await _context.Movies.Where(x => x.ReleaseDate > today).OrderBy(x => x.ReleaseDate).Take(top).ToListAsync();

            var inTheaters = await _context.Movies.Where(x => x.InTheaters).Take(top).ToListAsync();

            var result = new IndexMoviePageDTO();
            result.InTheaters = _mapper.Map<List<MovieDTO>>(inTheaters);
            result.UpcomingReleases = _mapper.Map<List<MovieDTO>>(upcomingReleases);

            return result;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] FilterMoviesDTO filterMovies)
        {
            var moviesQueryable = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMovies.Title)) // Only show the movie with the matching title
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(filterMovies.Title));
            }

            if (filterMovies.InTheaters) // Only show movies that are in theaters
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }

            if (filterMovies.UpcomingReleases) // Only show upcoming releases
            {
                var today = DateTime.Today;

                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (filterMovies.GenreID != 0) // Only show movies with matching genre IDs
            {
                moviesQueryable = moviesQueryable.Where(x => x.MoviesGenres.Select(y => y.GenreID).Contains(filterMovies.GenreID));
            }

            if (!string.IsNullOrWhiteSpace(filterMovies.OrderingField)) // Order the list by the field the user want to order by
            {
                try
                {
                    moviesQueryable = moviesQueryable.OrderBy($"{filterMovies.OrderingField} {(filterMovies.AscendingOrder ? "ascending" : "descending")}");
                }
                catch
                {
                    _logger.LogWarning($"Could not order by field: {filterMovies.OrderingField}");
                }
            }

            await HttpContext.InsertPaginationParametersInResponse(moviesQueryable, filterMovies.RecordsPerPage); // Setting up pagination for the results, as there could be a lot.

            var movies = await moviesQueryable.Paginate(filterMovies.Pagination).ToListAsync(); // Build a new list of the results after they have been paginated

            return _mapper.Map<List<MovieDTO>>(movies);
        }

        [HttpGet("{id}", Name = "getMovie")]
        public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person) // Gets the Actors name, by moving into the Person Entity via MovieActors
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre) // Gets the Genre name, by moving into the Genre Entity via MovieGenres
                .FirstOrDefaultAsync(x => x.ID == id);

            if (movie == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieDetailsDTO>(movie);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreation)
        {
            var movie = _mapper.Map<Movie>(movieCreation);

            if (movieCreation.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreation.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreation.Poster.FileName);
                    movie.Poster =
                        await _fileStorage.SaveFile(content, extension, containerName,
                                                            movieCreation.Poster.ContentType);
                }
            }

            AnnotateActorsOrder(movie);

            _context.Add(movie);
            await _context.SaveChangesAsync();
            var movieDTO = _mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("getMovie", new { id = movie.ID }, movieDTO);
        }

        /// <summary>
        /// Custom method to mark the order of actors as they appear in the array
        /// </summary>
        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreation)
        {
            var movieDB = await _context.Movies.FirstOrDefaultAsync(x => x.ID == id);

            if (movieDB == null)
            {
                return NotFound();
            }

            movieDB = _mapper.Map(movieCreation, movieDB);

            // Check to see if the update contains a new picture
            // If a new picture is sent, we will update
            // If not, this will be skipped

            if (movieCreation.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreation.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreation.Poster.FileName);
                    movieDB.Poster =
                        await _fileStorage.EditFile(content, extension, containerName,
                                                            movieDB.Poster,
                                                            movieCreation.Poster.ContentType);
                }
            }

            await _context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movieDB.ID}; delete from MoviesGenres where MovieId = {movieDB.ID}");

            AnnotateActorsOrder(movieDB);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entityFromDB = await _context.Movies.FirstOrDefaultAsync(x => x.ID == id); // Get the entity from the database with the matching ID

            if (entityFromDB == null)
            {
                return NotFound();
            }

            var entityDTO = _mapper.Map<MoviePatchDTO>(entityFromDB);

            patchDocument.ApplyTo(entityDTO, ModelState); // Applies the changes from the patchDocument (updates) to the entityDTO

            var isValid = TryValidateModel(entityDTO); // Check that the updates have the correct names for the model

            if (!isValid)
            {
                return BadRequest(ModelState); // If the updated model isn't valid, return the error.
            }

            _mapper.Map(entityDTO, entityFromDB); // Appling the changes back to the entity from the database

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Movies.AnyAsync(x => x.ID == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Remove(new Movie() { ID = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}