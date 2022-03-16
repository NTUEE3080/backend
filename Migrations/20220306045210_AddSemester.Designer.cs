﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PitaPairing.Database;

#nullable disable

namespace PitaPairing.Migrations
{
    [DbContext(typeof(CoreDbContext))]
    [Migration("20220306045210_AddSemester")]
    partial class AddSemester
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("IndexDataPostData", b =>
                {
                    b.Property<Guid>("LookingForId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RelatedPostsId")
                        .HasColumnType("uuid");

                    b.HasKey("LookingForId", "RelatedPostsId");

                    b.HasIndex("RelatedPostsId");

                    b.ToTable("PostsIndexes", (string)null);
                });

            modelBuilder.Entity("PitaPairing.Domain.Application.ApplicationData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<byte>("Status")
                        .HasColumnType("smallint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("PostId", "UserId")
                        .IsUnique();

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("PitaPairing.Domain.Index.IndexData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ApplicationDataId")
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ModuleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationDataId");

                    b.HasIndex("ModuleId", "Code")
                        .IsUnique();

                    b.ToTable("Indexes");
                });

            modelBuilder.Entity("PitaPairing.Domain.Index.IndexPropsData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Day")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("EndTime")
                        .HasColumnType("integer");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("IndexDataId")
                        .HasColumnType("uuid");

                    b.Property<int>("StartTime")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Venue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("IndexDataId");

                    b.ToTable("IndexPropsData");
                });

            modelBuilder.Entity("PitaPairing.Domain.Module.ModuleData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AcademicUnit")
                        .HasColumnType("integer");

                    b.Property<string>("CourseCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Semester")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("Semester", "CourseCode")
                        .IsUnique();

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("PitaPairing.Domain.Post.PostData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Completed")
                        .HasColumnType("boolean");

                    b.Property<Guid>("IndexId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ModuleId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("IndexId");

                    b.HasIndex("ModuleId");

                    b.HasIndex("UserId", "IndexId")
                        .IsUnique();

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("PitaPairing.Domain.Semester.SemesterData", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Semester");
                });

            modelBuilder.Entity("PitaPairing.User.UserData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Sub")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.HasIndex("Sub")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IndexDataPostData", b =>
                {
                    b.HasOne("PitaPairing.Domain.Index.IndexData", null)
                        .WithMany()
                        .HasForeignKey("LookingForId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PitaPairing.Domain.Post.PostData", null)
                        .WithMany()
                        .HasForeignKey("RelatedPostsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PitaPairing.Domain.Application.ApplicationData", b =>
                {
                    b.HasOne("PitaPairing.Domain.Post.PostData", "Post")
                        .WithMany("Applications")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PitaPairing.User.UserData", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PitaPairing.Domain.Index.IndexData", b =>
                {
                    b.HasOne("PitaPairing.Domain.Application.ApplicationData", null)
                        .WithMany("Offers")
                        .HasForeignKey("ApplicationDataId");

                    b.HasOne("PitaPairing.Domain.Module.ModuleData", "Module")
                        .WithMany("Indexes")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Module");
                });

            modelBuilder.Entity("PitaPairing.Domain.Index.IndexPropsData", b =>
                {
                    b.HasOne("PitaPairing.Domain.Index.IndexData", null)
                        .WithMany("Info")
                        .HasForeignKey("IndexDataId");
                });

            modelBuilder.Entity("PitaPairing.Domain.Post.PostData", b =>
                {
                    b.HasOne("PitaPairing.Domain.Index.IndexData", "Index")
                        .WithMany()
                        .HasForeignKey("IndexId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PitaPairing.Domain.Module.ModuleData", "Module")
                        .WithMany()
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PitaPairing.User.UserData", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Index");

                    b.Navigation("Module");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PitaPairing.Domain.Application.ApplicationData", b =>
                {
                    b.Navigation("Offers");
                });

            modelBuilder.Entity("PitaPairing.Domain.Index.IndexData", b =>
                {
                    b.Navigation("Info");
                });

            modelBuilder.Entity("PitaPairing.Domain.Module.ModuleData", b =>
                {
                    b.Navigation("Indexes");
                });

            modelBuilder.Entity("PitaPairing.Domain.Post.PostData", b =>
                {
                    b.Navigation("Applications");
                });
#pragma warning restore 612, 618
        }
    }
}