// =====================================================================
// StudentPortalConsole
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 13 — LINQ Part 2 + EF Core (Code First)
// Name : Saif Elden Khaled Nazmy Lotfy
// Lab ID: 31
//
//   Part C  GPA threshold          = 2.5 + ((31 mod 4) * 0.3) = 2.5 + (3 * 0.3) = 3.4
//   Part D  Instructor experience  = (31 mod 5) + 3            = 1 + 3          = 4
//   Part G  Added property         = 31 mod 3 = 1              -> CreditsCompleted
// =====================================================================

using Microsoft.EntityFrameworkCore;

namespace StudentPortalConsole
{



    internal class Program
    {
        static void Main(string[] args)
        {
            // =========================================================
            // SEED DATA — unchanged, exactly matches the Student Guide.
            // =========================================================
            List<Student> students = new List<Student>
            {
                new Student { FullName = "Yara Adel",    YearOfStudy = 2, Gpa = 3.5 },
                new Student { FullName = "Omar Hesham",  YearOfStudy = 3, Gpa = 2.8 },
                new Student { FullName = "Nada Samir",   YearOfStudy = 1, Gpa = 3.9 },
                new Student { FullName = "Kareem Fouad", YearOfStudy = 4, Gpa = 3.2 }
            };

            List<Instructor> instructors = new List<Instructor>
            {
                new Instructor { FullName = "Hamdy",       YearsOfExperience = 10,
                                 AssignedCourseName = "Web Development Using .NET" },
                new Instructor { FullName = "Mona Khalil", YearsOfExperience = 6,
                                 AssignedCourseName = "Database Fundamentals" }
            };

            List<Course> courses = new List<Course>
            {
                new Course { CourseName = "Web Development Using .NET", Credits = 4 },
                new Course { CourseName = "Database Fundamentals",      Credits = 3 }
            };

            Console.WriteLine("===== WARM-UP: Session 12's chain =====");
            var warmUp = students
                .Where(s => s.Gpa > 3.0)
                .OrderByDescending(s => s.Gpa)
                .Select(s => s.FullName)
                .ToList();
            foreach (string n in warmUp) Console.WriteLine($"  {n}");
            Console.WriteLine();

            // =========================================================
            // PART B — Predict-the-Output Drills
            // =========================================================
            Console.WriteLine("===== PART B =====");

            // B1. PREDICTION: Count() -> 0, Any() -> False, Average() -> THROWS EXCEPTION.
            // Reason: Count()/Any() are safe on empty collections and return
            // the "nothing here" value. Average() has no sensible answer for
            // zero elements, so LINQ refuses and throws InvalidOperationException
            // ("Sequence contains no elements") rather than inventing a 0.
            List<Student> b1Empty = new List<Student>();
            Console.WriteLine($"B1 Count: {b1Empty.Count()}");   // 0
            Console.WriteLine($"B1 Any: {b1Empty.Any()}");       // False
            try
            {
                Console.WriteLine($"B1 Average: {b1Empty.Average(s => s.Gpa)}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"B1 Average threw: {ex.GetType().Name} - \"{ex.Message}\"");
            }

            // B2. PREDICTION: prints "2 3 1 4" — NOT sorted.
            // Reason: GroupBy emits buckets in the order each key is FIRST
            // encountered while walking the source list (Yara=2 first, then
            // Omar=3, then Nada=1, then Kareem=4), not in sorted key order.
            Console.Write("B2: ");
            foreach (var g in students.GroupBy(s => s.YearOfStudy))
            {
                Console.Write($"{g.Key} ");
            }
            Console.WriteLine();

            // B3. PREDICTION: prints 3.
            // Reason: deferred execution. `q` is only a description of a
            // query at the moment it's declared — nothing runs yet. By the
            // time Count() forces it to execute, "Test Person" (year 3) has
            // already been added, so the query sees Omar(3), Kareem(4), and
            // Test Person(3) = 3 matches, not the 2 that existed when q was written.
            var b3Students = new List<Student>(students); // new copy so B3 doesn't pollute later parts
            var q = b3Students.Where(s => s.YearOfStudy >= 3);
            b3Students.Add(new Student { FullName = "Test Person", YearOfStudy = 3, Gpa = 3.0 });
            Console.WriteLine($"B3: {q.Count()}"); // 3
            Console.WriteLine();

            // =========================================================
            // PART C — Aggregates and Grouping (Lab ID 31)
            // Threshold = 2.5 + ((31 mod 4) * 0.3) = 2.5 + (3 * 0.3) = 3.4
            // =========================================================
            Console.WriteLine("===== PART C (threshold = 3.4) =====");
            const double myThreshold = 3.4; // 2.5 + (31 mod 4 * 0.3) = 3.4

            // C1 — seven aggregates
            Console.WriteLine($"Total count: {students.Count()}");                          // 4
            Console.WriteLine($"Count above my threshold ({myThreshold}): " +
                               $"{students.Count(s => s.Gpa > myThreshold)}");               // 2 (Yara 3.5, Nada 3.9)
            Console.WriteLine($"Average GPA: {students.Average(s => s.Gpa):F2}");            // 3.35
            Console.WriteLine($"Highest GPA: {students.Max(s => s.Gpa)}");                   // 3.9
            Console.WriteLine($"Lowest GPA: {students.Min(s => s.Gpa)}");                    // 2.8
            Console.WriteLine($"Any below 2.0: {students.Any(s => s.Gpa < 2.0)}");           // False
            Console.WriteLine($"All at/above 2.0: {students.All(s => s.Gpa >= 2.0)}");       // True

            // C2 — trigger the empty-collection exception deliberately (no guard)
            List<Student> c2Empty = new List<Student>();
            Console.WriteLine($"C2 Count: {c2Empty.Count()}"); // 0 — safe
            Console.WriteLine($"C2 Any: {c2Empty.Any()}");     // False — safe
            try
            {
                double crash = c2Empty.Average(s => s.Gpa);
                Console.WriteLine(crash);
            }
            catch (InvalidOperationException ex)
            {
                // Exact exception recorded, as required:
                // Type    : System.InvalidOperationException
                // Message : "Sequence contains no elements"
                Console.WriteLine($"C2 threw: {ex.GetType().FullName} - \"{ex.Message}\"");
            }

            // C3 — fix with a guard
            if (c2Empty.Any())
            {
                Console.WriteLine($"C3 Average: {c2Empty.Average(s => s.Gpa):F2}");
            }
            else
            {
                Console.WriteLine("C3: No students in the collection — average is undefined.");
            }

            // C4 — group by year of study (NOT sorted)
            Console.WriteLine("C4 — GroupBy YearOfStudy:");
            var byYear = students.GroupBy(s => s.YearOfStudy);
            foreach (var group in byYear)
            {
                Console.WriteLine($"  Year {group.Key}: {group.Count()} student(s)");
                foreach (var s in group) Console.WriteLine($"    {s.FullName}");
            }
            // COMMENT: the groups come out in the order 2, 3, 1, 4 — i.e. NOT
            // sorted by key. GroupBy buckets in first-encountered order while
            // walking the source (Yara is first in the list and is year 2,
            // so bucket 2 is created and emitted first), never in key order.

            // C5 — group by a COMPUTED key using MY threshold (3.4), own bucket names
            Console.WriteLine("C5 — GroupBy my own computed threshold band:");
            var byMyBand = students.GroupBy(s => s.Gpa >= myThreshold ? "Saif's Cutoff Met" : "Saif's Cutoff Missed");
            foreach (var group in byMyBand)
            {
                Console.WriteLine($"  {group.Key}: {group.Count()} student(s)");
                foreach (var s in group) Console.WriteLine($"    {s.FullName} ({s.Gpa:F2})");
            }

            // C6 — repeat C4, but sorted by key
            Console.WriteLine("C6 — GroupBy YearOfStudy, sorted:");
            var byYearSorted = students.GroupBy(s => s.YearOfStudy).OrderBy(g => g.Key); // .OrderBy(...) added
            foreach (var group in byYearSorted)
            {
                Console.WriteLine($"  Year {group.Key}: {group.Count()} student(s)");
                foreach (var s in group) Console.WriteLine($"    {s.FullName}");
            }
            Console.WriteLine();

            // =========================================================
            // PART D — Join and Its Silent Failure (Lab ID 31)
            // Experience = (31 mod 5) + 3 = 1 + 3 = 4
            // =========================================================
            Console.WriteLine("===== PART D (experience = 4) =====");
            const int myExperience = 4; // (31 mod 5) + 3 = 4

            // D1 — method syntax
            var teachingMethod = instructors.Join(
                courses,
                i => i.AssignedCourseName,
                c => c.CourseName,
                (i, c) => $"{i.FullName} teaches {c.CourseName} ({c.Credits} credits)"
            );
            Console.WriteLine("D1 — method syntax:");
            foreach (var line in teachingMethod) Console.WriteLine($"  {line}");

            // D2 — identical join, query syntax
            var teachingQuery = from i in instructors
                                join c in courses on i.AssignedCourseName equals c.CourseName
                                select $"{i.FullName} teaches {c.CourseName} ({c.Credits} credits)";
            Console.WriteLine("D2 — query syntax:");
            foreach (var line in teachingQuery) Console.WriteLine($"  {line}");

            // D3 — add an instructor representing myself, with my derived experience value,
            // assigned to a course that does NOT exist ("Machine Learning")
            var me = new Instructor
            {
                FullName = "Saif Elden Khaled Nazmy Lotfy",
                YearsOfExperience = myExperience, // 4
                AssignedCourseName = "Machine Learning"
            };
            instructors.Add(me);

            // D4 — re-run the join, report in vs. out
            var teachingAfter = instructors.Join(
                courses,
                i => i.AssignedCourseName,
                c => c.CourseName,
                (i, c) => $"{i.FullName} teaches {c.CourseName} ({c.Credits} credits)"
            ).ToList();
            Console.WriteLine($"D4 — Instructors in: {instructors.Count}, rows out: {teachingAfter.Count}");
            // Instructors in: 3, rows out: 2.
            // COMMENT: Join keeps only items that match on BOTH sides (inner-join
            // semantics). "Machine Learning" doesn't exist in `courses`, so my row
            // simply produces no output — no error, no blank line, no warning.
            // The row silently disappears rather than crashing, because a join
            // finding no match is completely normal, expected behaviour.

            // D5 — COMMENT: to include my own row with a blank course, I'd need a
            // LEFT join

            instructors.Remove(me); // keep later parts consistent with the guide's numbers
            Console.WriteLine();

            // =========================================================
            // PART E — Deferred Execution and Your Own Operator
            // =========================================================
            Console.WriteLine("===== PART E =====");

            // E1 — PREDICTION:
            // the real answer is 4 — the query only runs when Count() is
            // called, and by then Layla (GPA 3.7) has already been added.
            var deferredQuery = students.Where(s => s.Gpa > 3.0); // nothing runs yet
            students.Add(new Student { FullName = "Layla Mostafa", YearOfStudy = 2, Gpa = 3.7 });
            Console.WriteLine($"E1 count (prediction was 4, actual is): {deferredQuery.Count()}"); // 4
            

            // E2 — remove Layla again so later parts match the guide's numbers
            students.RemoveAll(s => s.FullName == "Layla Mostafa");

            // E3 — reproduce the multiple-enumeration bug deliberately (no ToList)
            var buggyQuery = students.Where(s => s.Gpa > 3.0);
            int buggyCount = buggyQuery.Count();                 // execution #1
            Console.WriteLine($"E3 count: {buggyCount}");
            foreach (var s in buggyQuery) Console.WriteLine($"  {s.FullName}"); // execution #2
            double buggyAvg = buggyQuery.Average(s => s.Gpa);    // execution #3
            Console.WriteLine($"E3 avg: {buggyAvg:F2}");
            // COMMENT: the filtering work actually runs THREE separate times —
            // once per consumption (Count, foreach, Average) — even though the
            // code only "looks" like it filters once.

            // E4 — the fix: materialize once with ToList()
            var fixedList = students.Where(s => s.Gpa > 3.0).ToList(); // runs ONCE, right here
            Console.WriteLine($"E4 count: {fixedList.Count}");     // .Count property
            foreach (var s in fixedList) Console.WriteLine($"  {s.FullName}");
            Console.WriteLine($"E4 avg: {fixedList.Average(s => s.Gpa):F2}");
            // COMMENT: what changed is a single .ToList() at the point the query
            // is composed. This matters because there each
            // enumeration is a full network round-trip to SQL Server, not just
            // wasted in-memory looping — 3 enumerations there means 3 round-trips.

            // E5 — own extension method, used in a chain
            List<string> topNames = students
                .MyTopStudents()                 // mine — Gpa >= 3.4
                .OrderBy(s => s.FullName)         // sort alphabetically
                .Select(s => s.FullName)          // just the names
                .ToList();
            Console.WriteLine("E5 — MyTopStudents() chain:");
            foreach (var name in topNames) Console.WriteLine($"  {name}"); // Nada Samir, Yara Adel

            // E6 — COMMENT: MyTopStudents() is DEFERRED, and not because of
            // its body just returns a Where(...) call,
            // and Where is deferred, so my operator inherits that behaviour
            // automatically.
            Console.WriteLine();

            // =========================================================
            // PART F — Your First Migration
            // =========================================================
            //
            // F3
            //   • Tables created by Up(): Students, Courses, Instructors — one
            //     CreateTable(...) call per DbSet declared on StudentPortalContext.
            //   • Column type EF chose for Gpa (double): float (SQL Server float(53)).
            //     Column type EF chose for FullName (string): nvarchar(max).
            //   • Is FullName nullable? No, and i didnt write anything to make the EF do that
            //     
            //   • Down() would reverse Up() exactly: three DropTable calls,
            //     dropping Instructors, Courses, and Students.
            //
            // F4 
            //   ITI_StudentPortalDB_EF does NOT exist yet.
            //   This proves Add-Migration ONLY writes a migration file to disk —
            //   it never opens a connection to the real database. The database
            //   stays completely untouched until Update-Database is run.
            //
            // F6 
            //   1. Column types: the hand-built table used explicit, deliberately
            //      sized types (e.g. varchar(50)/nvarchar(50) for names), while EF
            //      Code First defaults string columns to nvarchar(max) because
            //      nothing constrained the length in C#.

            //   2. Naming/constraints: EF auto-generates its own primary key
            //      constraint name (e.g. PK_Students) and an identity column
            //      purely from convention (property named Id), whereas the
            //      hand-built table had an explicitly written PRIMARY KEY clause
            //      with a name I chose myself in Session 3.

            // =========================================================
            // PART G — Add Your Own Property and Migrate It
            // =========================================================
            //
            // G4 
            //   The Up() method performs a single AddColumn<int>(name:
            //   "CreditsCompleted", table: "Students", nullable: false,
            //   defaultValue: 0) call — NOT a CreateTable. This matters because
            //   the Students table already has real seeded rows in it by this
            //   point (Part H's seed ran first): CreateTable would try to build
            //   a brand-new empty table and would either fail or wipe existing
            //   data, while AddColumn just appends one new column to the
            //   existing table and backfills the default value (0) into every
            //   row that's already there — the four seeded students survive.
            //
            // G6 — After Update-Database, SSMS confirms the new CreditsCompleted
            //   column exists on Students, and the row count is still 4 —
            //   the original seeded students (Yara, Omar, Nada, Kareem) are intact.

            // =========================================================
            // PART H — Query the Database with LINQ
            // =========================================================
            Console.WriteLine("===== PART H =====");
            using (var context = new StudentPortalContext())
            {
                // H1 — seed only if the table is currently empty
                if (!context.Students.Any())
                {
                    context.AddRange(students.Where(s => s.FullName != "Test Person")); // the 4 original students
                    context.SaveChanges();
                    Console.WriteLine("H1: seeded the database.");
                }
                else
                {
                    Console.WriteLine("H1: database already seeded — skipped.");
                }

                // H2 — the exact same chain as the Warm-Up, against the DATABASE
                var dbTopNames = context.Students
                    .Where(s => s.Gpa > 3.0)
                    .OrderByDescending(s => s.Gpa)
                    .Select(s => s.FullName)
                    .ToList();
                Console.WriteLine("H2 — same chain against context.Students:");
                foreach (var name in dbTopNames) Console.WriteLine($"  {name}");
                // Expect: Nada Samir, Yara Adel, Kareem Fouad — identical to the Warm-Up.

                // H3 — two aggregates against the database
                double dbAvg = context.Students.Average(s => s.Gpa);
                int dbCount = context.Students.Count();
                Console.WriteLine($"H3 — DB average GPA: {dbAvg:F2}, DB student count: {dbCount}");

                // H4 — COMMENT on the difference between these two lines:
                //   context.Students.Where(s => s.Gpa > 3.0).ToList()
                //     Where comes BEFORE ToList, so EF Core translates the filter
                //     into a SQL WHERE clause and SQL Server only sends back the
                //     matching rows. This is the one used above in H2 — filtering
                //     happens on the SERVER.
                //   context.Students.ToList().Where(s => s.Gpa > 3.0)
                //     ToList() comes FIRST, so EF pulls the ENTIRE Students table
                //     into memory as a List, and only then does the Where filter
                //     run — in C#, client-side. Same visible answer on 4 rows, but
                //     on a real table this pulls every row over the network before
                //     discarding most of them.
            }
            Console.WriteLine();

            // =========================================================
            // PART I — Wrap-Up Reflection
            // =========================================================
            //
            // I1 — Lab ID 31, three derived values with arithmetic:
            //   Part C threshold        = 2.5 + ((31 mod 4) * 0.3) = 2.5 + (3 * 0.3) = 3.4
            //   Part D experience value = (31 mod 5) + 3            = 1 + 3          = 4
            //   Part G property         = 31 mod 3 = 1              -> CreditsCompleted
            //
            // I2 — A silently missing join row is more dangerous to inherit than a
            //   crash because a crash announces itself immediately and gets fixed
            //   the same day, while a missing row looks exactly like a correct,
            //   complete answer. It can sit inside a report for months quietly
            //   under-counting something that matters, with nobody aware anything
            //   is wrong, because nothing ever raised an error to investigate.
            //
            // I3 — Add-Migration and Update-Database being two separate commands
            //   is a safety feature, not an inconvenience, because it creates a
            //   review checkpoint: Add-Migration only ever writes a plain C# file
            //   I can open, read, and delete with zero consequences if it's wrong
            //   (via Remove-Migration). Nothing about my actual database is at
            //   risk until I deliberately run Update-Database. If the two were
            //   merged into one command, every schema change would hit a real
            //   database blind, with no chance to catch a mistake first.
            //
            // I4 — One thing that IS different about running the identical LINQ
            //   chain against the database versus a List: against the database,
            //   context.Students is a DbSet<Student>, and enumerating it doesn't
            //   just loop in memory — it sends a generated SQL query across the
            //   network to SQL Server and waits for a response. Deferred execution
            //   matters far more there because every extra enumeration of the same
            //   deferred query is a full extra network round-trip (like Part E's
            //   bug, but each repeat now costs real network latency instead of a
            //   few CPU cycles), and because composing filters before consuming
            //   lets EF fold them all into ONE efficient SQL statement instead of
            //   pulling more data than necessary into memory first.

            Console.WriteLine("Done.");
        }
    }
}