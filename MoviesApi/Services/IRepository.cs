using MoviesApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public interface IRepository
    {
        /// <summary>
        /// Method to return the current list of Genres
        /// </summary>
        /// <returns>List of Genres</returns>
        Task<List<Genre>> GetAllGenres();

        /// <summary>
        /// Returns one genre with the matching ID
        /// </summary>
        /// <param name="id">ID of the Genre to get</param>
        /// <returns>Genre</returns>
        Genre GetGenreByID(int id);

        /// <summary>
        /// Adds a new Genre to the genre list
        /// </summary>
        /// <param name="genre">New genre to add</param>
        public void AddGenre(Genre genre);
    }
}