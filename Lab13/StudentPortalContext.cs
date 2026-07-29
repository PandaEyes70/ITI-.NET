using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentPortalConsole
{
    public class StudentPortalContext : DbContext
    {
        public DbSet<Student> Students { get; set; } // Represents the Students table
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                    "Server=DESKTOP-67SE8UE\\SQLEXPRESS;Database=ITI_StudentPortal_EF;Trusted_Connection=True;TrustServerCertificate=True"
                )
                .LogTo(Console.WriteLine , LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

    }
}
