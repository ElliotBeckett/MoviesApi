using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTO;
using MoviesApi.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/movietheaters")]
    public class MovieTheatersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public MovieTheatersController(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MovieTheaterDTO>>> Get([FromQuery] FilteredMovieTheatersDTO filteredMovieTheatersDTO)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var usersLocation = geometryFactory.CreatePoint(new Coordinate(filteredMovieTheatersDTO.Long, filteredMovieTheatersDTO.Lat));

            var theaters = await _context.MovieTheaters
                .OrderBy(x => x.Location.Distance(usersLocation))
                .Where(x => x.Location.IsWithinDistance(usersLocation, filteredMovieTheatersDTO.DistanceInKms * 1000))
                .Select(x => new MovieTheaterDTO { ID = x.ID, Name = x.Name, DistanceInMeters = Math.Round(x.Location.Distance(usersLocation)) })
                .ToListAsync();

            return theaters;
        }
    }
}