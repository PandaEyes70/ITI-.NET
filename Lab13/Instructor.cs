// =====================================================================
// StudentPortalConsole — SESSION PROJECT (Style Guide Rule 20/35/39/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 13 — LINQ Part 2 + EF Core (Code First)
//
// THIS PROJECT IS DAY-READY (Rule 39). The model classes and all seed
// data are already present as REAL, WORKING, RUNNABLE code, and the EF
// Core packages are already referenced. Open it, press run, and it
// builds and prints the Session 12 warm-up chain immediately — before a
// single TODO is done.
//
// Only TODAY'S NEW content is left as TODOs. Each TODO sits exactly
// where its code will be written (Rule 40), and the numbers run
// strictly top-to-bottom through this file in the same order the
// lecture teaches them:
//
//   TODO 1      Block 3 — your own LINQ extension method (top-level)
//   TODO 2-3    Blocks 4-5 — the DbContext
//   TODO 4-6    Block 1 (aggregates), inside Main
//   TODO 7-11   Block 2 (GroupBy, Join), inside Main
//   TODO 12-16  Block 3 (deferred execution), inside Main
//   TODO 17-19  Blocks 4-5 (EF Core queries), inside Main
//
// ⚠️ WHY TODO 1 IS AT THE TOP RATHER THAN WITH BLOCK 3'S OTHER TODOs:
//   C# requires extension methods in a top-level static class (CS1109),
//   so it cannot sit inside Program next to the code that uses it. It is
//   numbered 1 because it is physically first (Rule 40); teach it when
//   you reach Block 3, and TODO 16 is where you actually call it.
//
// ⚠️ BEFORE THE EF TODOs WILL WORK:
//   1. Run Application/Session13_PreInit.sql in SSMS.
//   2. Check TODO 3's connection string matches YOUR server.
//   3. Package Manager Console:  Add-Migration InitialCreate
//                                Update-Database
//
// For the full, correct, runnable version (do NOT peek until you've
// tried it yourself, or you're checking your own work), see:
// ../StudentPortalConsole_Complete/Instructor.cs
// =====================================================================

namespace StudentPortalConsole
{
    public class Instructor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int YearsOfExperience { get; set; }
        public string AssignedCourseName { get; set; }
    }

    #region 📋 Full TODO Checklist (collapse this region for a quick overview)
    // --- Namespace level, above Program ---
    // 1.  The public static top-level class + HonorRoll extension method   [Block 3]
    // 2.  The StudentPortalContext class with three DbSet properties       [Block 4]
    // 3.  Its OnConfiguring override + the four-part connection string     [Block 4]
    // --- Inside Main (seed data and the warm-up chain are already provided) ---
    // 4.  The seven aggregate values                                        [Block 1]
    // 5.  Trigger the empty-collection exception; record it                 [Block 1]
    // 6.  Guard it with Any() and print a message instead                   [Block 1]
    // 7.  GroupBy year; note the groups are NOT sorted                      [Block 2]
    // 8.  GroupBy a COMPUTED key (Honors/Standard)                          [Block 2]
    // 9.  GroupBy year again, chained with OrderBy on the key               [Block 2]
    // 10. Join instructors to courses, in BOTH syntaxes                     [Block 2]
    // 11. Add Sara Nabil; prove 3 in / 2 out; remove her again              [Block 2]
    // 12. Prove deferred execution with the late-added fifth student        [Block 3]
    // 13. Remove that fifth student again                                   [Block 3]
    // 14. Write the multiple-enumeration anti-pattern deliberately          [Block 3]
    // 15. Write the ToList() fix directly beneath it                        [Block 3]
    // 16. Use HonorRoll in a chain (defined in TODO 1)                      [Block 3]
    // 17. Open a StudentPortalContext in a using block                      [Block 4]
    // 18. Seed the database if empty, then SaveChanges                      [Block 5]
    // 19. Re-run the Warm-Up's exact chain against the DATABASE             [Block 5]
    #endregion
}
