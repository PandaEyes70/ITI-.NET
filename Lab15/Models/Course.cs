using System.ComponentModel.DataAnnotations;

namespace Lab15_StudentPortalWeb.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = "";

        public int Credits { get; set; }

        public int? InstructorId { get; set; }
        public Instructor? Instructor { get; set; }
    }
}
