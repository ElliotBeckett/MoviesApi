using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesGenres>().HasKey(x => new { x.GenreID, x.MovieID });
            modelBuilder.Entity<MoviesActors>().HasKey(x => new { x.PersonID, x.MovieID });

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder); // If overriding the OnModelCreating method, this line MUST be here
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            modelBuilder.Entity<MovieTheater>()
                .HasData(new List<MovieTheater>
                {
                    new MovieTheater{ID = 1, Name = "Agora", Location = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))},
                    new MovieTheater{ID = 2, Name = "Sambil", Location = geometryFactory.CreatePoint(new Coordinate(-69.9118804, 18.4826214))},
                    new MovieTheater{ID = 3, Name = "Megacentro", Location = geometryFactory.CreatePoint(new Coordinate(-69.856427, 18.506934))},
                    new MovieTheater{ID = 4, Name = "Village East Cinema", Location = geometryFactory.CreatePoint(new Coordinate(-73.986227, 40.730898))}
                });

            var adventure = new Genre() { ID = 4, Name = "Adventure" };
            var animation = new Genre() { ID = 5, Name = "Animation" };
            var drama = new Genre() { ID = 6, Name = "Drama" };
            var romance = new Genre() { ID = 7, Name = "Romance" };

            modelBuilder.Entity<Genre>()
                .HasData(new List<Genre>
                {
                    adventure, animation, drama, romance
                });

            var jimCarrey = new Person() { ID = 5, Name = "Jim Carrey", DateOfBirth = new DateTime(1962, 01, 17) };
            var robertDowney = new Person() { ID = 6, Name = "Robert Downey Jr.", DateOfBirth = new DateTime(1965, 4, 4) };
            var chrisEvans = new Person() { ID = 7, Name = "Chris Evans", DateOfBirth = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Person>()
                .HasData(new List<Person>
                {
                    jimCarrey, robertDowney, chrisEvans
                });

            var endgame = new Movie()
            {
                ID = 5,
                Title = "Avengers: Endgame",
                InTheaters = true,
                ReleaseDate = new DateTime(2019, 04, 26)
            };

            var iw = new Movie()
            {
                ID = 6,
                Title = "Avengers: Infinity Wars",
                InTheaters = false,
                ReleaseDate = new DateTime(2019, 04, 26)
            };

            var sonic = new Movie()
            {
                ID = 7,
                Title = "Sonic the Hedgehog",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 28)
            };
            var emma = new Movie()
            {
                ID = 8,
                Title = "Emma",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 21)
            };
            var greed = new Movie()
            {
                ID = 9,
                Title = "Greed",
                InTheaters = false,
                ReleaseDate = new DateTime(2020, 02, 21)
            };

            modelBuilder.Entity<Movie>()
                .HasData(new List<Movie>
                {
                    endgame, iw, sonic, emma, greed
                });

            modelBuilder.Entity<MoviesGenres>().HasData(
                new List<MoviesGenres>()
                {
                    new MoviesGenres(){MovieID = endgame.ID, GenreID = drama.ID},
                    new MoviesGenres(){MovieID = endgame.ID, GenreID = adventure.ID},
                    new MoviesGenres(){MovieID = iw.ID, GenreID = drama.ID},
                    new MoviesGenres(){MovieID = iw.ID, GenreID = adventure.ID},
                    new MoviesGenres(){MovieID = sonic.ID, GenreID = adventure.ID},
                    new MoviesGenres(){MovieID = emma.ID, GenreID = drama.ID},
                    new MoviesGenres(){MovieID = emma.ID, GenreID = romance.ID},
                    new MoviesGenres(){MovieID = greed.ID, GenreID = drama.ID},
                    new MoviesGenres(){MovieID = greed.ID, GenreID = romance.ID},
                });

            modelBuilder.Entity<MoviesActors>().HasData(
                new List<MoviesActors>()
                {
                    new MoviesActors(){MovieID = endgame.ID, PersonID = robertDowney.ID, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieID = endgame.ID, PersonID = chrisEvans.ID, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieID = iw.ID, PersonID = robertDowney.ID, Character = "Tony Stark", Order = 1},
                    new MoviesActors(){MovieID = iw.ID, PersonID = chrisEvans.ID, Character = "Steve Rogers", Order = 2},
                    new MoviesActors(){MovieID = sonic.ID, PersonID = jimCarrey.ID, Character = "Dr. Ivo Robotnik", Order = 1}
                });
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesGenres> MovieGenres { get; set; }
        public DbSet<MoviesActors> MoviesActors { get; set; }
        public DbSet<MovieTheater> MovieTheaters { get; set; }
    }
}