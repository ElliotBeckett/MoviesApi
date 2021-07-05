using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Entities
{
    public class Person
    {
        public int ID { get; set; }

        [Required]
        [StringLength(120)]
        public string Name { get; set; }

        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Picture { get; set; } // Url of picture.
        public List<MoviesActors> MoviesActors { get; set; }
    }
}