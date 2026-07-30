using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentPortalConsole
{
    public class StudentPortalContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-67SE8UE\\SQLEXPRESS;Database=ITI_StudentPortal_EF;Trusted_Connection=True;TrustServerCertificate=True;")


                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO 6 — DONE. Fluent API, configuring the SAME rule as the
            // [Required]/[MaxLength(100)] attributes on Student.FullName.
            // Written both ways on purpose. Precedence, lowest to highest:
            // conventions -> Data Annotations -> Fluent API. If these two
            // ever disagreed, the Fluent API line below would win.
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            // TODO 7 — DONE. The Instructor <-> Course relationship.
            // Lab ID 31 -> 31 mod 2 = 1 -> DeleteBehavior.SetNull.
            // Meaning: deleting an Instructor sets InstructorId to NULL on
            // every course they taught, instead of deleting the courses
            // (Cascade) or refusing the delete outright (Restrict). This
            // is only legal because InstructorId above is nullable.

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }


        //public class StudentPortalContext : DbContext
        //{
        //    public DbSet<Student> Students { get; set; } // Represents the Students table
        //    public DbSet<Course> Courses { get; set; }
        //    public DbSet<Instructor> Instructors { get; set; }


        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        optionsBuilder.UseSqlServer(
        //                "Server=DESKTOP-67SE8UE\\SQLEXPRESS;Database=ITI_StudentPortal_EF;Trusted_Connection=True;TrustServerCertificate=True"
        //            )
        //            .LogTo(Console.WriteLine , LogLevel.Information)
        //            .EnableSensitiveDataLogging();
        //    }

        //}

    }

