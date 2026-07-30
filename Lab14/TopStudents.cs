using System;
using System.Collections.Generic;
using System.Text;

namespace StudentPortalConsole
{


    public static class StudentQueryExtensions
    {
        
        public static IEnumerable<Student> HonorRoll(this IEnumerable<Student> source)
        {
            return source.Where(s => s.Gpa >= 3.5);
        }

        // Part E.5 — trainee's own operator, using the Part C threshold (3.4).
        // Deferred: its body is just a Where(...), and Where is deferred,
        // so this inherits that behaviour for free
        public static IEnumerable<Student> MyTopStudents(this IEnumerable<Student> source)
        {
            return source.Where(s => s.Gpa >= 3.4); // Part C threshold: 2.5 + (31 mod 4 * 0.3) = 3.4
        }
    }
}
