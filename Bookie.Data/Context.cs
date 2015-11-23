using System;
using System.IO;
using Windows.Storage;
using Bookie.Common.Model;
using Microsoft.Data.Entity;

namespace Bookie.Data
{
    public class Context : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<BookMark> Bookmarks { get; set; }
        public DbSet<Cover> Covers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databaseFilePath = "bookie.db";
            try
            {
                databaseFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseFilePath);
            }
            catch (InvalidOperationException)
            {
            }

            optionsBuilder.UseSqlite($"Data source={databaseFilePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().Ignore(e => e.Image);
            modelBuilder.Entity<Book>().HasOne(e => e.Source).WithMany(r => r.Books).ForeignKey(r => r.SourceId);

            modelBuilder.Entity<Book>().HasOne(e => e.Cover).WithOne(r => r.Book);
            modelBuilder.Entity<Book>().HasMany(e => e.BookMarks).WithOne(t => t.Book);
        }
    }
}