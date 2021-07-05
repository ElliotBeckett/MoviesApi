using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Helpers;
using MoviesApi.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTO
{
    public class MovieCreationDTO : MoviePatchDTO
    {
        [FileSizeValidator(4)] // Custom Validator that sets the max size (Mb) that a file can be
        [ContentTypeValidator(ContentTypeGroup.Image)] // Custom validator to check the type of file uploaded
        public IFormFile Poster { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenresIDs { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorCreationDTO>>))]
        public List<ActorCreationDTO> Actors { get; set; }
    }
}