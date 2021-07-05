using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using MoviesApi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public class LinksGenerator
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LinksGenerator(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper GetUrlHelper()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        public async Task Generate<T>(ResultExecutingContext context, ResultExecutionDelegate next) where T : class, IGenerateHATEOASLinks, new()
        {
            var urlHelper = GetUrlHelper();
            var result = context.Result as ObjectResult;
            var model = result.Value as T;

            if (model == null)
            {
                var modelList = result.Value as List<T> ?? throw new ArgumentNullException($"Was expecting an instance of {typeof(T)}");
                modelList.ForEach(dto => dto.GenerateLinks(urlHelper));
                var individual = new T();

                result.Value = individual.GenerateLinksCollection(modelList, urlHelper);
                await next();
            }
            else
            {
                model.GenerateLinks(urlHelper);
                await next();
            }
        }
    }
}