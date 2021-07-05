using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MoviesApi.Services
{
    public class MovieInTheaterService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer timer;

        public MovieInTheaterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1)); // Sets the timer to only check once a day
            return Task.CompletedTask;
        }

        /// <summary>
        /// Custom method to check if a movie release date is the same as today.
        /// If it is, then set the movie to be InTheaters
        /// </summary>
        private async void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var today = DateTime.Today;
                var movies = await context.Movies.Where(x => x.ReleaseDate == today).ToListAsync();

                if (movies.Any()) // Only triggers if there are any movies in the database.
                {
                    foreach (var movie in movies)
                    {
                        movie.InTheaters = true;
                    }

                    await context.SaveChangesAsync();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}