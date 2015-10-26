using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Bookie.Data;

namespace Bookie.Data.Migrations
{
    [DbContext(typeof(BookieContext))]
    [Migration("20151023085529_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Annotation("ProductVersion", "7.0.0-beta8-15964");

            modelBuilder.Entity("Bookie.Common.Model.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Abstract");

                    b.Property<DateTime?>("DatePublished");

                    b.Property<string>("FileName");

                    b.Property<string>("FullPathAndFileName");

                    b.Property<int?>("Pages");

                    b.Property<int>("Rating");

                    b.Property<int?>("SourceId");

                    b.Property<string>("Title");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Bookie.Common.Model.BookMark", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BookId");

                    b.Property<int>("PageNumber");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Bookie.Common.Model.Cover", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("FileName");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Bookie.Common.Model.Source", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Path");

                    b.Property<string>("Token");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Bookie.Common.Model.Book", b =>
                {
                    b.HasOne("Bookie.Common.Model.Source")
                        .WithMany()
                        .ForeignKey("SourceId");
                });

            modelBuilder.Entity("Bookie.Common.Model.BookMark", b =>
                {
                    b.HasOne("Bookie.Common.Model.Book")
                        .WithMany()
                        .ForeignKey("BookId");
                });

            modelBuilder.Entity("Bookie.Common.Model.Cover", b =>
                {
                    b.HasOne("Bookie.Common.Model.Book")
                        .WithOne()
                        .ForeignKey("Bookie.Common.Model.Cover", "Id");
                });
        }
    }
}
