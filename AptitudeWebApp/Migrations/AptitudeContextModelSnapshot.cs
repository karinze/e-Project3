﻿// <auto-generated />
using System;
using AptitudeWebApp.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AptitudeWebApp.Migrations
{
    [DbContext(typeof(AptitudeContext))]
    partial class AptitudeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AptitudeWebApp.Applicant", b =>
                {
                    b.Property<Guid>("ApplicantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("Age")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool?>("HasPassedExam")
                        .HasColumnType("bit");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Notes")
                        .HasMaxLength(3000)
                        .HasColumnType("nvarchar(3000)");

                    b.Property<string>("Password")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("Username")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.HasKey("ApplicantId");

                    b.ToTable("Applicants");
                });

            modelBuilder.Entity("AptitudeWebApp.Manager", b =>
                {
                    b.Property<Guid>("ManagerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("LastName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.HasKey("ManagerId");

                    b.ToTable("Managers");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ApplicantCategory", b =>
                {
                    b.Property<int>("ApplicantCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ApplicantCategoryId"));

                    b.Property<Guid>("ApplicantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<bool>("HasCleared")
                        .HasColumnType("bit");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.HasKey("ApplicantCategoryId");

                    b.ToTable("ApplicantCategories");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ApplicantCompanies", b =>
                {
                    b.Property<int>("ApplicantCompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ApplicantCompanyId"));

                    b.Property<Guid>("ApplicantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CompanyName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .HasMaxLength(3000)
                        .HasColumnType("nvarchar(3000)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ApplicantCompanyId");

                    b.HasIndex("ApplicantId");

                    b.ToTable("ApplicantCompanies");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ApplicantEducation", b =>
                {
                    b.Property<int>("ApplicantEducationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ApplicantEducationId"));

                    b.Property<Guid>("ApplicantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("COEName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Notes")
                        .HasMaxLength(3000)
                        .HasColumnType("nvarchar(3000)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ApplicantEducationId");

                    b.HasIndex("ApplicantId");

                    b.ToTable("ApplicantEducations");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ApplicantExam", b =>
                {
                    b.Property<int>("ApplicantExamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ApplicantExamId"));

                    b.Property<Guid>("ApplicantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ApplicantScore")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExamStartDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.HasKey("ApplicantExamId");

                    b.ToTable("ApplicantExams");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.Exam", b =>
                {
                    b.Property<int>("ExamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExamId"));

                    b.Property<int>("ApplicantId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("HasStartedYet")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ExamId");

                    b.ToTable("Exams");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ExamCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("CategoryName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("ExamId")
                        .HasColumnType("int");

                    b.HasKey("CategoryId");

                    b.HasIndex("ExamId");

                    b.ToTable("ExamCategories");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ExamQuestion", b =>
                {
                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("CorrectQuestion")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<int?>("ExamCategoryCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("QuestionA")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("QuestionB")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("QuestionC")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("QuestionD")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<int?>("QuestionMark")
                        .HasColumnType("int");

                    b.Property<string>("QuestionText")
                        .HasMaxLength(3000)
                        .HasColumnType("nvarchar(3000)");

                    b.HasKey("QuestionId");

                    b.HasIndex("ExamCategoryCategoryId");

                    b.ToTable("ExamQuestions");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ApplicantCompanies", b =>
                {
                    b.HasOne("AptitudeWebApp.Applicant", null)
                        .WithMany("ApplicantCompanies")
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ApplicantEducation", b =>
                {
                    b.HasOne("AptitudeWebApp.Applicant", null)
                        .WithMany("ApplicantEducation")
                        .HasForeignKey("ApplicantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ExamCategory", b =>
                {
                    b.HasOne("AptitudeWebApp.Models.Exam", null)
                        .WithMany("ExamCategories")
                        .HasForeignKey("ExamId");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ExamQuestion", b =>
                {
                    b.HasOne("AptitudeWebApp.Models.ExamCategory", null)
                        .WithMany("ExamQuestions")
                        .HasForeignKey("ExamCategoryCategoryId");
                });

            modelBuilder.Entity("AptitudeWebApp.Applicant", b =>
                {
                    b.Navigation("ApplicantCompanies");

                    b.Navigation("ApplicantEducation");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.Exam", b =>
                {
                    b.Navigation("ExamCategories");
                });

            modelBuilder.Entity("AptitudeWebApp.Models.ExamCategory", b =>
                {
                    b.Navigation("ExamQuestions");
                });
#pragma warning restore 612, 618
        }
    }
}
