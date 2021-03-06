﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using myFeedback.Models;

namespace myFeedback.Migrations
{
    [DbContext(typeof(myDataContext))]
    [Migration("20190413174223_Feedback")]
    partial class Feedback
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("myFeedback.Models.Feedback", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Complete");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Issue");

                    b.Property<string>("Session");

                    b.HasKey("Id");

                    b.ToTable("Feedback");
                });
#pragma warning restore 612, 618
        }
    }
}
