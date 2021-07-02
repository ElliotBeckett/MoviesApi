using Microsoft.Extensions.Logging;
using MoviesApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public class InMemoryRepository : IRepository
    {
        private List<Genre> _genres;
        private readonly ILogger<InMemoryRepository> _logger;

        public InMemoryRepository(ILogger<InMemoryRepository> logger)
        {
            _genres = new List<Genre>()
            {
                new Genre(){ ID = 1, Name = "Comedy" },
                new Genre(){ ID = 2, Name = "Action"}
            };
            _logger = logger;
        }

        /// <summary>
        /// Method to return the current list of Genres
        /// </summary>
        /// <returns>List of Genres</returns>

        /*
         * Using Async allows the method to run while the program is completing other tasks
         * However, Async programs have a larger performance hit (as they use new threads)
         * When working with databases or other web platforms, Async calls are preferred
         */

        public async Task<List<Genre>> GetAllGenres()
        {
            _logger.LogInformation("Executing GetAllGenres");
            await Task.Delay(1); // A one milisecond delay
            return _genres;
        }

        /// <summary>
        /// Returns one genre with the matching ID
        /// </summary>
        /// <param name="id">ID of the Genre to get</param>
        /// <returns>Genre</returns>
        public Genre GetGenreByID(int id)
        {
            return _genres.FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// Adds a new Genre to the genre list
        /// </summary>
        /// <param name="genre">New genre to add</param>
        public void AddGenre(Genre genre)
        {
            genre.ID = _genres.Max(x => x.ID) + 1;
            _genres.Add(genre);
        }
    }
}