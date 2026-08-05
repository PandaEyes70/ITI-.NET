// =====================================================================
// StudentPortalContext — CARRIED FORWARD, with ONE change today (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
//
// This file arrives EXACTLY as Session 15 left it, with both of
// yesterday's changes already made: the options-accepting constructor is
// here, and OnConfiguring is gone. The connection string lives in
// Program.cs now, where the DI container can hand it in.
//
// One thing changes today, and it is small: TODO 5 adds VALIDATION
// rules to the Student class. Everything else is Sessions 13-16's work,
// untouched.
//
// ⚠️ READ THIS BEFORE YOU TYPE TODO 5. There are two different kinds of
//    attribute that look identical and are not:
//
//      [Required] and [MaxLength(100)]  →  SCHEMA attributes. Entity
//        Framework turns these into real column definitions. Adding one
//        changes the database and demands a migration. These are already
//        here, from Session 14, and already migrated.
//
//      [Range(...)]                     →  a VALIDATION-only attribute.
//        Entity Framework does not map it to anything. It changes no
//        column, needs no migration, and exists purely so the framework
//        can check a submitted form before your code runs.
//
//    That is why today adds no migration. Block 4 makes the distinction
//    explicit, because guessing wrong here produces a schema change
//    nobody intended.
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
    // Answer: [Range] is a validation-only attribute.
    // You can prove this to someone by showing that changing the range limits or error message does not prompt Entity Framework
    // to detect any changes (running Add-Migration would result in an empty migration with no table or column alterations),
    // whereas modifying [Required] or [MaxLength] changes table metadata and generates structural schema changes.
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        [Range(1, 3, ErrorMessage = "Year of study must be between 1 and 3.")]
        public int YearOfStudy { get; set; }
        
        [Range(2.3, 4.0, ErrorMessage = "GPA must be between 2.30 and 4.00.")]
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

        // TODO 1: the constructor that makes this class
        // constructible by somebody else. This is what lets Program.cs
        // decide the connection string instead of this file deciding it.
        public StudentPortalContext(DbContextOptions<StudentPortalContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Block 2 — Fluent API wins over annotations.
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            // Block 3 — the real relationship. Restrict means
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
