using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Entities
{
    public class MovieTheater
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public Point Location { get; set; }
    }
}