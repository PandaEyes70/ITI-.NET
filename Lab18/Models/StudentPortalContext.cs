// =====================================================================
// StudentPortalContext — CARRIED FORWARD FROM SESSION 17 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
//
// This file arrives EXACTLY as Session 15 left it, with both of
// yesterday's changes already made: the options-accepting constructor is
// here, and OnConfiguring is gone. The connection string lives in
// Program.cs now, where the DI container can hand it in.
//
// Nothing in this file changes today either. The [Range] validation
// rules from Session 17 are still here and still working; Session 18
// never touches the model or the controller.
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
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";
        // TODO 5 —  Add ONE validation attribute above each of the two
        //         properties below — the attribute whose name means "the
        //         value must fall between these two numbers, inclusive".
        //         For the year of study, the two numbers are 1 and 4,
        //         which is what an academic year is at this university.
        //         For the grade point average, they are 0.0 and 4.0.
        //         Give each one a named ErrorMessage argument as well,
        //         written in plain English, because the sentence you put
        //         there is the sentence a real student will read on the
        //         form when they get it wrong. The default message is
        //         understandable but robotic; write the one you would want
        //         to see.
        //         ⚠️ Neither of these changes the database. Do not run
        //         Add-Migration after typing them. If Visual Studio ever
        //         tells you a migration is pending after adding a
        //         validation attribute, you have used a schema attribute
        //         by mistake — check which one you typed.
        [Range(1,4,ErrorMessage = "Year of study must be between 1 and 4.")]
        public int YearOfStudy { get; set; }
        
        [Range(0.0,4.0,ErrorMessage = "GPA must be between 0.0 and 4.0.")]
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
