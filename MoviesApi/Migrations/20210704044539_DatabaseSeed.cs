using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesApi.Migrations
{
    public partial class DatabaseSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 4, "Adventure" },
                    { 5, "Animation" },
                    { 6, "Drama" },
                    { 7, "Romance" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "ID", "InTheaters", "Poster", "ReleaseDate", "Summary", "Title" },
                values: new object[,]
                {
                    { 5, true, null, new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Avengers: Endgame" },
                    { 6, false, null, new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Avengers: Infinity Wars" },
                    { 7, false, null, new DateTime(2020, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Sonic the Hedgehog" },
                    { 8, false, null, new DateTime(2020, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Emma" },
                    { 9, false, null, new DateTime(2020, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Greed" }
                });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "ID", "Biography", "DateOfBirth", "Name", "Picture" },
                values: new object[,]
                {
                    { 5, null, new DateTime(1962, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jim Carrey", null },
                    { 6, null, new DateTime(1965, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert Downey Jr.", null },
                    { 7, null, new DateTime(1981, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chris Evans", null }
                });

            migrationBuilder.InsertData(
                table: "MovieGenres",
                columns: new[] { "GenreID", "MovieID" },
                values: new object[,]
                {
                    { 6, 5 },
                    { 4, 5 },
                    { 6, 6 },
                    { 4, 6 },
                    { 4, 7 },
                    { 6, 8 },
                    { 7, 8 },
                    { 6, 9 },
                    { 7, 9 }
                });

            migrationBuilder.InsertData(
                table: "MoviesActors",
                columns: new[] { "PersonID", "MovieID", "Character", "Order" },
                values: new object[,]
                {
                    { 5, 7, "Dr. Ivo Robotnik", 1 },
                    { 6, 5, "Tony Stark", 1 },
                    { 6, 6, "Tony Stark", 1 },
                    { 7, 5, "Steve Rogers", 2 },
                    { 7, 6, "Steve Rogers", 2 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumns: new[] { "GenreID", "MovieID" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumns: new[] { "GenreID", "MovieID" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumns: new[] { "GenreID", "MovieID" },
                keyValues: new object[] { 4, 7 });

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumns: new[] { "GenreID", "MovieID" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumns: new[] { "GenreID", "MovieID" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumns: new[] { "GenreID", "MovieID" },
                keyValues: new object[] { 6, 8 });

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumns: new[] { "GenreID", "MovieID" },
                keyValues: new object[] { 6, 9 });

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumns: new[] { "GenreID", "MovieID" },
                keyValues: new object[] { 7, 8 });

            migrationBuilder.DeleteData(
                table: "MovieGenres",
                keyColumns: new[] { "GenreID", "MovieID" },
                keyValues: new object[] { 7, 9 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonID", "MovieID" },
                keyValues: new object[] { 5, 7 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonID", "MovieID" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonID", "MovieID" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonID", "MovieID" },
                keyValues: new object[] { 7, 5 });

            migrationBuilder.DeleteData(
                table: "MoviesActors",
                keyColumns: new[] { "PersonID", "MovieID" },
                keyValues: new object[] { 7, 6 });

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "ID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "ID",
                keyValue: 7);
        }
    }
}
