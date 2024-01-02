using AptitudeWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AptitudeWebApp.DAL
{
    public class AptitudeContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(local);Database=AptitudeDB;Encrypt=False;User=sa;Password=123456789.t", options => options.EnableRetryOnFailure());
        }
        public AptitudeContext(DbContextOptions<AptitudeContext> options)
         : base(options)
        {
        }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<ApplicantCompanies> ApplicantCompanies { get; set; }
        public DbSet<ApplicantEducation> ApplicantEducations { get; set; }
        public DbSet<ApplicantExam> ApplicantExams { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestions> ExamQuestions { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Answer> Answers { get; set; }



    }
}
