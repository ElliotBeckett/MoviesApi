using MoviesApi.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTO
{
    public class GenreCreationDTO
    {
        [Required] // Makes this a required field
        [StringLength(40)] // Sets the min & max length a string/value can be
        [FirstLetterUppercase] // Custom attribute validation - This can be used anywhere on any model.
        public string Name { get; set; }
    }
}