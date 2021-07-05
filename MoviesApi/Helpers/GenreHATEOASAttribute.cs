using Microsoft.AspNetCore.Mvc.Filters;
using MoviesApi.DTO;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helpers
{
    public class GenreHATEOASAttribute : HATEOASAttribute
    {
        private readonly LinksGenerator _linksGenerator;

        public GenreHATEOASAttribute(LinksGenerator linksGenerator)
        {
            _linksGenerator = linksGenerator;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var includeHATEOAS = ShouldIncludeHATEOAS(context);

            if (!includeHATEOAS)
            {
                await next();
                return;
            }

            await _linksGenerator.Generate<GenreDTO>(context, next);
        }
    }
}