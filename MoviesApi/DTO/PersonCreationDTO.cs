using Microsoft.AspNetCore.Http;
using MoviesApi.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTO
{
    public class PersonCreationDTO : PersonPatchDTO
    {
        [FileSizeValidator(4)] // Custom Validator that sets the max size (Mb) that a file can be
        [ContentTypeValidator(ContentTypeGroup.Image)] // Custom validator to check the type of file uploaded
        public IFormFile Picture { get; set; }
    }
}