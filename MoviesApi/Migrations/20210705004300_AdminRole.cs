using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesApi.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Insert into AspNetRoles(Id, [name], [NormalizedName]) values ('4af64945-13c2-4e60-ba31-3592f62b49b3', 'Admin', 'Admin')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete AspNetRoles where Id = '4af64945-13c2-4e60-ba31-3592f62b49b3'");
        }
    }
}