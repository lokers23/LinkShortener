﻿// <auto-generated />
using System;
using LinkShortener.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LinkShortener.DAL.Migrations
{
    [DbContext(typeof(ShortLinkDbContext))]
    partial class ShortLinkDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.17");

            modelBuilder.Entity("LinkShortener.DAL.Models.Link", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)");

                    b.Property<int>("CountClick")
                        .HasColumnType("int(11)");

                    b.Property<DateTime>("DateCreate")
                        .HasColumnType("date");

                    b.Property<string>("LongUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("links");
                });
#pragma warning restore 612, 618
        }
    }
}
