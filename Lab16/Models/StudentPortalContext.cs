// =====================================================================
// StudentPortalContext — CARRIED FORWARD FROM SESSION 15 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
//
// This file arrives EXACTLY as Session 15 left it, with both of
// yesterday's changes already made: the options-accepting constructor is
// here, and OnConfiguring is gone. The connection string lives in
// Program.cs now, where the DI container can hand it in.
//
// Nothing in this file changes today. Routing is decided before a
// controller exists, let alone a database connection — today's whole
// topic happens upstream of everything below.
//
// ⚠️ MIGRATION OWNERSHIP — unchanged from Session 15:
//    The Session 14 CONSOLE project is still the owner of this
//    database's migrations. This web project deliberately has NO
//    Migrations/ folder, and nobody runs Add-Migration or Update-Database
//    from it. Today changes zero schema.
// =====================================================================

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortalWeb.Models
{
    // =================================================================
    // THE ENTITIES — unchanged since Session 14.
    // =================================================================
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearOfStudy { get; set; }

        public double Gpa { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = "";

        public int Credits { get; set; }

        public int InstructorId { get; set; }

        public Instructor Instructor { get; set; } = null!;
    }

    public class Instructor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearsOfExperience { get; set; }

        public List<Course> Courses { get; set; } = new();
    }

    // =================================================================
    // THE CONTEXT — Session 15's version, unchanged.
    // =================================================================
    public class StudentPortalContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        // Session 15, TODO 1: the constructor that makes this class
        // constructible by somebody else. This is what lets Program.cs
        // decide the connection string instead of this file deciding it.
        public StudentPortalContext(DbContextOptions<StudentPortalContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Session 14 Block 2 — Fluent API wins over annotations.
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            // Session 14 Block 3 — the real relationship. Restrict means
            // the database refuses to delete an instructor who still has
            // courses, rather than silently deleting the courses too.
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
