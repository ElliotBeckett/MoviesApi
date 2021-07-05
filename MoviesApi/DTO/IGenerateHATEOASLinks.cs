using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.DTO
{
    public interface IGenerateHATEOASLinks
    {
        void GenerateLinks(IUrlHelper urlHelper);

        ResourceCollection<T> GenerateLinksCollection<T>(List<T> dtos, IUrlHelper urlHelper);
    }
}