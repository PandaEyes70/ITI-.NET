

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace StudentPortalConsole
{
    public class Student
    {
        public int Id { get; set; }

        // TODO 1 — DONE.
        // [Required]  -> NOT NULL in the database
        // [MaxLength(100)] -> nvarchar(100) instead of nvarchar(max)

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearOfStudy { get; set; }
        public double Gpa { get; set; }
    }


    //public class Student
    //{
    //    public int Id { get; set; }
    //    public string FullName { get; set; }
    //    public int YearOfStudy { get; set; }
    //    public double Gpa { get; set; }

    //    // TODO 2 (Part G, fingerprinted — Lab ID 31 -> 31 mod 3 = 1):
    //    public int CreditsCompleted { get; set; }
    //}

   
}
