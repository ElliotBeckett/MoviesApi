using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Entities
{
    public class MoviesGenres
    {
        public int MovieID { get; set; }
        public int GenreID { get; set; }
        public Movie Movie { get; set; }
        public Genre Genre { get; set; }
    }
}