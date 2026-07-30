

using System.ComponentModel.DataAnnotations;

namespace StudentPortalConsole
{

    public class Course
    {
        public int Id { get; set; }

        // TODO 2 - Same idea as Student.FullName, 150 chars allowed.
        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = "";

        public int Credits { get; set; }

        // TODO 3 —
        // Lab ID 31 -> 31 mod 2 = 1 -> Part E assigns SetNull as the delete
        // behaviour. SetNull REQUIRES the foreign key to be nullable, so
        // InstructorId is `int?`, not `int`, and the navigation property
        // is the nullable `Instructor?
        public int? InstructorId { get; set; }
        public Instructor? Instructor { get; set; }
    }
    //public class Course
    //{
    //    public int Id { get; set; }
    //    public string CourseName { get; set; }
    //    public int Credits { get; set; }
    //}
}

   
