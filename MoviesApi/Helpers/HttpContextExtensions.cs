using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Helpers
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Custom method to count the total amount of pages of requested records.
        /// </summary>

        public async static Task InsertPaginationParametersInResponse<T>(this HttpContext httpContext, IQueryable<T> queryable, int recordsPerPage)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); } // If the context is empty, return an exception.

            double count = await queryable.CountAsync(); // Counts the amount of records in the database.
            double totalAmountPages = Math.Ceiling(count / recordsPerPage);

            httpContext.Response.Headers.Add("totalAmountOfPages", totalAmountPages.ToString());
        }
    }
}