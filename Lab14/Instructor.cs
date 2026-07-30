

using System.ComponentModel.DataAnnotations;

namespace StudentPortalConsole
{
    public class Instructor
    {
        public int Id { get; set; }

        // TODO 4 (part one) — DONE. Same required/max-length rule.
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";
       

        public int YearsOfExperience { get; set; }

        // AssignedCourseName is DELETED (TODO 4, part two).
        // Keeping it alongside the real relationship would be a mistake:
        // it would become a second, competing source of truth for "who
        // teaches what". The two could drift apart the moment somebody
        // updated one and not the other — which is exactly what already
        // happened once, silently, with Sara Nabil in the Session 13
        // bridge. A real foreign key is supposed to REPLACE that string,
        // not sit next to it.
        //
        // TODO 4 (part two) — DONE. The "one" side of the relationship.
        // Initializing to `new()` matters: without it, looping over this
        // property before it's loaded throws a NullReferenceException
        // instead of just (unhelpfully) printing nothing.
        public List<Course> Courses { get; set; } = new();
    }


    //public class Instructor
    //{
    //    public int Id { get; set; }
    //    public string FullName { get; set; }
    //    public int YearsOfExperience { get; set; }
    //    public string AssignedCourseName { get; set; }
    //}


}
