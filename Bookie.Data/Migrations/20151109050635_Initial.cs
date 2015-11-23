using System;
using Microsoft.Data.Entity.Migrations;

namespace Bookie.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("Source", table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Path = table.Column<string>(nullable: true),
                Token = table.Column<string>(nullable: true)
            },
                constraints: table => { table.PrimaryKey("PK_Source", x => x.Id); });
            migrationBuilder.CreateTable("Book", table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Abstract = table.Column<string>(nullable: true),
                Author = table.Column<string>(nullable: true),
                CurrentPage = table.Column<int>(nullable: true),
                DatePublished = table.Column<DateTime>(nullable: true),
                Favourite = table.Column<bool>(nullable: false),
                FileName = table.Column<string>(nullable: true),
                FullPathAndFileName = table.Column<string>(nullable: true),
                Isbn = table.Column<string>(nullable: true),
                Pages = table.Column<int>(nullable: true),
                Publisher = table.Column<string>(nullable: true),
                Rating = table.Column<int>(nullable: false),
                Scraped = table.Column<bool>(nullable: false),
                Shelf = table.Column<bool>(nullable: false),
                SourceId = table.Column<int>(nullable: false),
                Title = table.Column<string>(nullable: true)
            },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.Id);
                    table.ForeignKey("FK_Book_Source_SourceId", x => x.SourceId, "Source", "Id");
                });
            migrationBuilder.CreateTable("BookMark", table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                BookId = table.Column<int>(nullable: true),
                PageNumber = table.Column<int>(nullable: false)
            },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookMark", x => x.Id);
                    table.ForeignKey("FK_BookMark_Book_BookId", x => x.BookId, "Book", "Id");
                });
            migrationBuilder.CreateTable("Cover", table => new
            {
                Id = table.Column<int>(nullable: false),
                FileName = table.Column<string>(nullable: true)
            },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cover", x => x.Id);
                    table.ForeignKey("FK_Cover_Book_Id", x => x.Id, "Book", "Id");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("BookMark");
            migrationBuilder.DropTable("Cover");
            migrationBuilder.DropTable("Book");
            migrationBuilder.DropTable("Source");
        }
    }
}