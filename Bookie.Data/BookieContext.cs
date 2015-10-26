using System;
using System.IO;
using Windows.Storage;
using Bookie.Common.Model;
using Microsoft.Data.Entity;

namespace Bookie.Data
{
    public class BookieContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Cover> Covers { get; set; }
        public DbSet<BookMark> BookMarks { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseFilePath = "bookie.db";
            try
            {
                databaseFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseFilePath);
            }
            catch (InvalidOperationException)
            { }

            optionsBuilder.UseSqlite($"Data source={databaseFilePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().Ignore(e => e.Image);
            modelBuilder.Entity<Book>().HasOne(e => e.Source).WithMany(r=> r.Books);
            modelBuilder.Entity<Book>().HasOne(e => e.Cover).WithOne(r => r.Book);
            modelBuilder.Entity<Book>().HasMany(e => e.BookMarks).WithOne(t => t.Book);
        }
    }
}