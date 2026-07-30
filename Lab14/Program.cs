// =====================================================================
// StudentPortalConsole
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 14 — EF Core: CRUD, Relationships, and Loading
//
// Name : Saif Elden Khaled Nazmy Lotfy
// Lab ID: 31
// Fingerprinted values for Lab ID 31
// -----------------------------------------------------------------
//   Part C — GPA value      : 3.0 + ((31 mod 7) * 0.1) = 3.0 + (3*0.1) = 3.3
//   Part E — Delete behavior: 31 mod 2 = 1  -> SetNull (FK must be nullable)
//   Part F — Extra courses  : (31 mod 3) + 2 = 1 + 2  = 3
// =====================================================================

using Microsoft.EntityFrameworkCore;
using System.Data;

namespace StudentPortalConsole
{




    internal class Program
    {
        // ================================================================
        // PART B — Predict-the-Output Drills
        // ================================================================
        // B1.

        //     var s = await context.Students.FirstAsync(x => x.Id == 1);
        //     s.Gpa = 3.99;
        //     Console.WriteLine(s.Gpa);

        //   PREDICTION: prints 3.99, but the DATABASE ROW IS UNCHANGED.
        //   REASON: assigning to s.Gpa only changes the in-memory object.
        //   EF's change tracker notices the object now differs from its
        //   snapshot, but generates and sends no SQL until
        //   SaveChangesAsync() is called — which never happens here.

        // B2.

        //     var instructors = await context.Instructors.ToListAsync();
        //     foreach (var i in instructors)
        //         Console.WriteLine($"{i.FullName}: {i.Courses.Count}");

        //   PREDICTION: every line prints ": 0", even for instructors who
        //   really do teach courses in the database.
        //   REASON: Instructors.ToListAsync() with no Include() only
        //   fetches Instructor rows. Courses is initialized to `new()`,
        //   so it exists and is safe to read,
        //   but nothing has ever populated it

        // B3.

        //     var s = await context.Students.AsNoTracking().FirstAsync();
        //     s.Gpa = 2.0;
        //     await context.SaveChangesAsync();

        //   PREDICTION: nothing happens to the database — no error, no
        //   change, no exception. It is dangerous BECAUSE it is silent.
        //   REASON: AsNoTracking() tells EF not to snapshot this entity.
        //   With no snapshot, SaveChangesAsync() has nothing to diff `s`
        //   against, detects zero changed entities, and sends no SQL at
        //   all. Code that "looks" like a normal update quietly does
        //   nothing
        // ================================================================

        static async Task Main(string[] args)
        {
            using (var context = new StudentPortalContext())
            {
                // =============================================================
                // PART A — Setup
                // =============================================================

                Console.WriteLine("Students currently in the database:");
                foreach (var s in await context.Students.ToListAsync())
                {
                    Console.WriteLine($"  {s.FullName} — Year {s.YearOfStudy}, GPA {s.Gpa:F2}");
                }
                Console.WriteLine();

                // =============================================================
                // PART C — Full CRUD, Async, Verified   (Lab ID 31 -> GPA = 3.3)
                // Formula: 3.0 + ((31 mod 7) * 0.1) = 3.0 + (3 * 0.1) = 3.3
                // =============================================================

                const double myGpa = 3.3; // Lab ID 31: 3.0 + ((31 mod 7) * 0.1)

                // C1 (TODO 8) Read Nada Samir with an async single-entity method.

                var nada = await context.Students.FirstAsync(s => s.FullName == "Nada Samir");
                Console.WriteLine($"[C1] Read: {nada.FullName} — current GPA {nada.Gpa:F2}");

                // C2 (TODO 9) Change her GPA, do NOT save yet.

                nada.Gpa = myGpa;
                Console.WriteLine($"[C2] In C# after assignment (NOT saved yet): {nada.Gpa:F2}");


                // in SSMS right now. It will still show the OLD value.
                // WHY THEY DIFFER: `nada.Gpa = myGpa;` only changes a plain C#
                // property on an in-memory object. EF's change tracker records
                // that this entity is now different from its loaded
                // snapshot, but no SQL is generated or sent until
                // SaveChangesAsync() actually runs. No save yet, no write yet.

                // C3 (TODO 10) Save, then re-check.

                await context.SaveChangesAsync();
                Console.WriteLine($"[C3] Saved. GPA should now read {myGpa:F2} in SSMS.");


                // HOW EF KNEW TO UPDATE ONLY Gpa: when `nada` was loaded, EF
                // stored a snapshot of every scalar property (FullName,
                // YearOfStudy, Gpa). At SaveChangesAsync(), EF compares the
                // CURRENT values against that snapshot property-by-property.
                // Only Gpa differs, so the generated UPDATE statement's SET
                // clause touches only Gpa — FullName and YearOfStudy are
                // re-sent nowhere, even though they're on the same row.

                // C4 (TODO 11) Create a new student using my own real name.

                var me = new Student { FullName = "Saif Elden Khaled Nazmy Lotfy", YearOfStudy = 2, Gpa = myGpa };
                Console.WriteLine($"[C4] Before SaveChangesAsync(), Id = {me.Id}"); // 0: CLR int default, nothing assigned yet
                await context.Students.AddAsync(me);
                await context.SaveChangesAsync();
                Console.WriteLine($"[C4] Database-assigned Id = {me.Id}");

                // Before the save, Id was 0 (the default for an un-set int).
                // After SaveChangesAsync(), EF reads back the identity value
                // SQL Server generated for the new row and writes it into
                // me.Id automatically

                // C5  Update YearOfStudy to 3. Panda

                var saif = await context.Students.FirstAsync(s => s.FullName == "Saif Elden Khaled Nazmy Lotfy");
                saif.YearOfStudy = 3;
                await context.SaveChangesAsync();
                Console.WriteLine($"[C5] YearOfStudy updated to {saif.YearOfStudy}.");


                // C6 (TODO 12) — Delete my own student, save, verify.

                //var saif = await context.Students.FirstAsync(s => s.FullName == "Saif Elden Khaled Nazmy Lotfy");
                context.Students.Remove(saif);
                await context.SaveChangesAsync();
                Console.WriteLine("[C6] Deleted my own student row.");


                // WHY Remove() HAS NO ASYNC VERSION: Remove() only flags the
                // entity as Deleted in the local change tracker — a purely
                // in-memory bookkeeping step, exactly like assigning a
                // property or calling Add(). There is no I/O to await at that
                // point. The actual DELETE statement is only built and sent
                // inside SaveChangesAsync(), which is already the async call.

                // =============================================================
                // PART D — Constraints, Both Ways, Migrated onto Real Data
                // =============================================================

                // 
                // migration 
                //
                //   - Operation Up() performs on FullName: AlterColumn<string>(...)
                //     — narrowing nvarchar(max) down to
                //     nvarchar(100)
                //   - nullable: false, oldNullable: false
                //   - Two kinds of existing row that could break this migration:
                //       1) FullName IS NULL
                //       2) LEN(FullName) > 100
                //   - SSMS check query to run first:
                //       SELECT * FROM Students WHERE FullName IS NULL OR LEN(FullName) > 100;


                // TODO 13 — Prove the constraint with a caught exception.
                try
                {
                    var broken = new Student { FullName = null!, YearOfStudy = 2, Gpa = 3.0 };
                    await context.Students.AddAsync(broken);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"[D] Database refused a NULL FullName. Exception type: {ex.GetType().Name}");
                    // >>> RECORD: Database refused a NULL FullName. Exception type: DbUpdateException

                }
                finally
                {
                    // Detach any pending Added-but-never-saved Student entities
                    foreach (var entry in context.ChangeTracker.Entries<Student>()
                                 .Where(e => e.State == EntityState.Added).ToList())
                    {
                        entry.State = EntityState.Detached;
                    }
                }

                // =============================================================
                // PART E — The Real Relationship   (Lab ID 31 mod 2 = 1 -> SetNull)
                // =============================================================

                // Read-before-apply questions for the
                // AddInstructorCourseRelationship migration:
                //
                //   - Operations Up() performs: DropColumn("AssignedCourseName",
                //     "Instructors"); AddColumn<int>("InstructorId", "Courses",
                //     nullable: true); AddForeignKey(...); CreateIndex on
                //     Courses.InstructorId.
                //   - The data-destroying step: DropColumn("AssignedCourseName",
                //     "Instructors") — every instructor's existing
                //     AssignedCourseName text is permanently gone the moment
                //     this runs; there is no getting it back from the Down()
                //     of a later migration.
                //   - What a real project would do first: capture that data
                //     before dropping it — either back the column up, or (better)
                //     run a one-off script/migration step that uses the existing
                //     AssignedCourseName values to populate the new InstructorId
                //     column on the matching courses, so the information moves
                //     across instead of being deleted outright.

                var hamdy = await context.Instructors.FirstOrDefaultAsync(i => i.FullName == "Hamdy");
                var webCourse = await context.Courses
                    .FirstOrDefaultAsync(c => c.CourseName == "Web Development Using .NET");

                // TODO 14 — Link them via the FK property only (no navigation touch).
                if (hamdy != null && webCourse != null)
                {
                    webCourse.InstructorId = hamdy.Id;
                    await context.SaveChangesAsync();
                    Console.WriteLine($"[E] Linked '{webCourse.CourseName}' to {hamdy.FullName} " +
                                       $"by setting InstructorId = {hamdy.Id} (never loaded/assigned the navigation property).");
                }

                // TODO 15 — Prove the FK constraint is real, not decorative.
                try
                {
                    var orphan = new Course { CourseName = "AI and Machine Learning", Credits = 3, InstructorId = 9999 };
                    await context.Courses.AddAsync(orphan);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"[E] Invalid InstructorId (9999) rejected: {ex.InnerException?.Message}");

                    // >>> RECORD: [E] Invalid InstructorId (9999) rejected:
                    // The INSERT statement conflicted with the FOREIGN KEY constraint "FK_Courses_Instructors_InstructorId".
                    // The conflict occurred in database "ITI_StudentPortal_EF",
                    // table "dbo.Instructors", column 'Id'.


                    // COMPARE TO AssignedCourseName: the old string column would
                    // have SILENTLY ACCEPTED "9999" as a course's instructor —
                    // it was just text, nothing checked whether it referred to
                    // anyone real. That's precisely how Sara Nabil vanished from
                    // the Session 13 join with no error at all. A real foreign
                    // key refuses the bad row at save time instead of hiding it.
                }
                finally
                {
                    foreach (var entry in context.ChangeTracker.Entries<Course>()
                                 .Where(e => e.State == EntityState.Added).ToList())
                    {
                        entry.State = EntityState.Detached;
                    }
                    }

                    // =============================================================
                    // PART F — Loading Strategies and the N+1 Problem
                    // (Lab ID 31 -> (31 mod 3) + 2 = 1 + 2 = 3 extra courses)
                    // =============================================================
                    //var hamdy = await context.Instructors.FirstOrDefaultAsync(i => i.FullName == "Hamdy");
                if (hamdy != null)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        await context.Courses.AddAsync(new Course
                        {
                            CourseName = $"Saif Extra Course {i}",
                            Credits = 3,
                            InstructorId = hamdy.Id
                        });
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("[F] Added 3 extra courses (Lab ID 31), all assigned to Hamdy.");
                }

                // TODO 16 — Load instructors WITHOUT Include.
                Console.WriteLine("[F] Loading instructors WITHOUT Include:");
                var instructorsNoInclude = await context.Instructors.ToListAsync();
                foreach (var instructor in instructorsNoInclude)
                {
                    Console.WriteLine($"  {instructor.FullName}: {instructor.Courses.Count} courses");
                }

                // >>> RECORD: every count prints 0 and the log shows
                // exactly 1 query (a plain SELECT against Instructors)

                // TODO 17 — Load again, this time WITH Include.

                Console.WriteLine("[F] Loading instructors WITH Include:");
                var instructorsWithInclude = await context.Instructors
                    .Include(i => i.Courses)
                    .ToListAsync();
                foreach (var instructor in instructorsWithInclude)
                {
                    Console.WriteLine($"  {instructor.FullName}:");
                    foreach (var course in instructor.Courses)
                    {
                        Console.WriteLine($"     {course.CourseName} ({course.Credits} credits)");
                    }
                }

                // >>> RECORD: 4 courses for hamdy and 0 for mona and exactly 1 query — a single SELECT with a
                // LEFT JOIN between Instructors and Courses.
                //
                // WHY SQL RETURNED MORE ROWS THAN THERE ARE INSTRUCTORS: a JOIN
                // produces one result row per MATCHING PAIR, not per instructor.
                // An instructor with 4 courses comes back as 4 rows, each
                // repeating that instructor's own columns. EF's relational
                // fixup recognizes the repeated instructor key across those
                // rows and folds them back into a single Instructor object
                // with a fully populated Courses list — so the C# output looks
                // right even though the raw SQL result set had duplicates.

                // TODO 18 — Explicit loading on a single instructor.

                var oneInstructor = await context.Instructors.FirstOrDefaultAsync();
                if (oneInstructor != null)
                {
                    Console.WriteLine($"[F] {oneInstructor.FullName}: {oneInstructor.Courses.Count} courses (before explicit load)");
                    await context.Entry(oneInstructor).Collection(i => i.Courses).LoadAsync();
                    Console.WriteLine($"[F] {oneInstructor.FullName}: {oneInstructor.Courses.Count} courses (after explicit load)");
                }

                // >>> RECORD: Before explicit load -> count = 0,
                // After explicit load -> count = 4 and the query count from the log.
                // 2 queries total here (1 for the instructor, 1
                // deliberate extra for LoadAsync's courses)

                // TODO 19 — AsNoTracking() for read-only work.
                var readOnlyStudents = await context.Students.AsNoTracking().ToListAsync();
                Console.WriteLine($"[F] Loaded {readOnlyStudents.Count} students with AsNoTracking().");
                if (readOnlyStudents.Count > 0)
                {
                    readOnlyStudents[0].Gpa = 2.0;
                    await context.SaveChangesAsync();
                }

                // it will be UNCHANGED, with no exception and no warning of any
                // kind. AsNoTracking() means EF never stored a snapshot for
                // these entities; with no snapshot, SaveChangesAsync() has
                // nothing to diff against, detects zero changed entities, and
                // sends no SQL.
                //}

                Console.WriteLine();
                Console.WriteLine("Done.");
            }
        }

        // =====================================================================
        // PART G — Wrap-Up Reflection
        // =====================================================================
        // 1. Lab ID 31, three derived values, with arithmetic:
        //      Part C GPA        : 3.0 + ((31 mod 7) * 0.1) = 3.0 + (3 * 0.1) = 3.3
        //      Part E delete rule: 31 mod 2 = 1 -> SetNull (InstructorId made nullable)
        //      Part F extra count: (31 mod 3) + 2 = 1 + 2 = 3
        //
        // 2. My OnDelete question (Part E row for 31 mod 2 = 1, i.e. SetNull):
        //    why SetNull requires the foreign key to be nullable, and what
        //    changed in my entity —
        //    SetNull's whole job is to keep the child row (the course) alive
        //    after its parent (the instructor) is deleted, by clearing the
        //    link instead of deleting the course or refusing the delete. But
        //    "clearing the link" means writing NULL into InstructorId — and a
        //    column declared as plain `int` can never legally hold NULL in SQL
        //    Server. So the FK has to be declared `int?` (and the navigation
        //    property `Instructor?`) before SetNull is even a legal choice;
        //    with a non-nullable FK, EF would refuse this configuration
        //    outright. That's exactly why Course.InstructorId is `int?`
        //    not `int`.
        //
        // 3. Did any migration fail today?
        //    >>> no rollback needed
        //        Update-Database <PreviousMigrationName>
        //        Remove-Migration
        //
        // 4. What Session 13's multiple enumeration and today's N+1 problem
        //    actually have in common, using my own Part F query counts as
        //    evidence:
        //    Both bugs share the same root cause — code that LOOKS like it
        //    touches the database once but actually doesn't. Multiple
        //    enumeration re-ran an unmaterialized LINQ query once per
        //    consumption (Count(), then foreach, then Average() — 3 separate
        //    round-trips from what read like one query). N+1 does the same
        //    thing shaped differently: TODO 16's loop over instructor.Courses
        //    with no Include() would, under lazy loading, fire one extra
        //    query PER INSTRUCTOR rather than per re-read of the same query.
        //    >>> RECORD: TODO 16/17 both
        //    showed 1 query in the log because lazy loading is off, while
        //    TODO 18's explicit load pushed the count to 2 for a single
        //    instructor, which is the shape N+1 takes once you have more than
        //    one. The fix in both cases is the same shape too: make the
        //    round-trips visible (ToList() a query once; Include() a
        //    navigation up front) and collapse many round-trips into one.
        // =====================================================================
    }
}
