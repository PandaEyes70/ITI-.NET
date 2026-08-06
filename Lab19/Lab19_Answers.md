# Lab 19 — Models: the Many-to-Many Relationship & Associative Entities
**Name:** Saif Elden Khaled Nazmy Lotfy  
**Lab ID:** 31  
**Course:** ITI Summer Training | Web Development Using .NET  

---

## 🔑 Your Personal Lab ID & Fingerprinted Values
* **Lab ID:** 31
* **MIN_GRADE_LAB:** `1.0 + (31 mod 4) * 0.5` = `1.0 + 3 * 0.5` = **`2.5`**
* **COURSE_COUNT:** `(31 mod 3) + 2` = `1 + 2` = **`3`**
* **First Name (lowercase):** **`saif`**

---

## 🛠️ PART A — Verify and Orient
1. **Verification:** Confirmed that the basic pages work, `/students/{id}` displays the Enrolled Courses section, and `/courses`/`/courses/{id}` function properly.
2. **Comment added at the very top of `Controllers/EnrollmentsController.cs`:**
   ```csharp
   // LAB 19 — Lab ID: 31 | MIN_GRADE_LAB = 2.5 | COURSE_COUNT = 3
   ```
3. **Include vs ThenInclude Answer:**  
   `CoursesController.Index` only needs to count the number of enrollments for each course (which is a property directly on the loaded `Enrollment` records), so it only needs to load the first hop. `CoursesController.Details`, however, must display the name of the actual student associated with each enrollment, which is a second-hop property (`Enrollment.Student.FullName`) that requires `ThenInclude(e => e.Student)` to avoid being null.

---

## 🛠️ PART B — EnrollmentsController: the GET half
* **GET Action Code (`Controllers/EnrollmentsController.cs`):**
  ```csharp
  [HttpGet]
  public async Task<IActionResult> Create()
  {
      var students = await _context.Students.OrderBy(s => s.FullName).ToListAsync();
      var courses = await _context.Courses.OrderBy(c => c.CourseName).ToListAsync();

      ViewData["Students"] = students;
      ViewData["Courses"] = courses;

      return View();
  }
  ```
* **GET Database Queries Answer:**  
  Students have no external dependencies for their creation, so the GET action just serves a blank input form. An Enrollment, however, connects existing records, so the GET action must query the database to fetch the list of available students and courses to populate the dropdown selections for the user.

---

## 🛠️ PART C — EnrollmentsController: the POST half
* **POST Action Code (`Controllers/EnrollmentsController.cs`):**
  ```csharp
  [HttpPost]
  public async Task<IActionResult> Create(Enrollment enrollment)
  {
      ModelState.Remove(nameof(enrollment.Student));
      ModelState.Remove(nameof(enrollment.Course));
      ModelState.Remove(nameof(enrollment.EnrollmentDate));

      if (!ModelState.IsValid)
      {
          ViewData["Students"] = await _context.Students.OrderBy(s => s.FullName).ToListAsync();
          ViewData["Courses"] = await _context.Courses.OrderBy(c => c.CourseName).ToListAsync();
          return View(enrollment);
      }

      enrollment.EnrollmentDate = DateTime.Now;

      _context.Enrollments.Add(enrollment);
      await _context.SaveChangesAsync();

      var student = await _context.Students.FindAsync(enrollment.StudentId);
      var course = await _context.Courses.FindAsync(enrollment.CourseId);
      string studentName = student?.FullName ?? $"Student #{enrollment.StudentId}";
      string courseName = course?.CourseName ?? $"Course #{enrollment.CourseId}";

      TempData["Message"] = $"{studentName} was successfully enrolled in {courseName}.";

      return RedirectToAction("Details", "Students", new { id = enrollment.StudentId });
  }
  ```
* **EnrollmentDate Server-Side Answer:**  
  Setting `EnrollmentDate` server-side in the controller prevents malicious clients from tampering with the request payload (e.g. via browser developer tools or HTTP posting utilities) to backdate or fake the enrollment date.

---

## 🛠️ PART D — Your own validation boundary
* **Tightened attribute in `Models/StudentPortalContext.cs`:**
  ```csharp
  [Range(2.5, 4.0, ErrorMessage = "Grade must be between 2.5 and 4.0.")]
  public double? Grade { get; set; }
  ```
* **Testing Outcomes:**
  * **Grade below 2.5:** Submitting a grade of `2.4` causes model validation to fail. The page displays the custom error message `"Grade must be between 2.5 and 4.0."` and preserves the typed value in the form.
  * **Grade of 2.5:** Submitting `2.5` is successfully accepted, saved, and redirects to the student details view.
  * **Empty Grade:** Leaving the grade blank is accepted because `Grade` is nullable, and the `[Range]` attribute is bypassed for `null`. It correctly saves in the database as `NULL` rather than defaulting to `0`.

---

## 🛠️ PART E — Enroll yourself, and prove the constraint
1. **Creation:** Added a `Student` row with `FullName` set to `"saif"`.
2. **COURSE_COUNT Enrollments:** Enrolled `"saif"` in exactly **3** different courses:
   * *Web Development Using .NET* (Grade = `3.7`, GPA-badge color: green / first class honors)
   * *Database Fundamentals* (Grade = `3.2`, GPA-badge color: blue / second class honors)
   * *Saif Extra Course 1* (Grade = left blank, displayed as "Not yet graded")
3. **Duplicate Attempt Result:**
   * Trying to enroll `"saif"` in *Database Fundamentals* a second time triggers a `DbUpdateException` wrapping a SQL `SqlException` due to a duplicate key violation against the unique index `IX_Enrollments_StudentId_CourseId` (SQL error number 2601).
   * The application handles the unhandled database exception by displaying an HTTP 500 server error page.
   * Confirmed in SQL Server (SSMS) that the database contains exactly one enrollment row for this student-course pair, proving data-level index safety.
4. **Duplicate Behavior Answer:**  
   When the duplicate insert was attempted, the database rejected it by throwing a `DbUpdateException` wrapping a `SqlException` due to the composite unique index constraint violation (error number 2601), causing the application to return an HTTP 500 server error page, which matches the database unique index enforcement demonstrated in Block 5's console demo.

---

## 🛠️ PART F — Wrap-Up Reflection
1. **Derived Calculations:**
   * Lab ID: 31
   * $\text{MIN\_GRADE\_LAB} = 1.0 + (31 \bmod 4) \times 0.5 = 1.0 + 1.5 = 2.5$
   * $\text{COURSE\_COUNT} = (31 \bmod 3) + 2 = 1 + 2 = 3$
2. **Three Boundaries of Data Rejection:**
   * (1) Client-side browser checks (jQuery validation data-attributes), (2) Server-side model validation (`ModelState.IsValid` and validation attributes), and (3) Database engine constraints (composite unique index/foreign keys). Our Part D change lives in server-side model validation.
3. **Many-to-Many Relationship Failure:**
   Placing a foreign key in either the `Student` or `Course` table only supports a one-to-many relationship, which prevents a student from taking multiple courses or a course from having multiple students. A many-to-many relationship requires a separate associative table (e.g., `Enrollments`) with foreign keys referencing both tables to cleanly establish the link between multiple records on both sides.
4. **Cascade Delete Design Decision:**
   For a future relationship where a **StudentFeedback** record belongs to an **Enrollment**:
   * *Selected Delete Behavior:* `Cascade`
   * *Rationale:* Since student feedback is directly tied to a specific enrollment in a course, if that enrollment record is deleted, the feedback is orphaned and loses all meaning. Choosing `Cascade` delete automatically cleans up the database by removing the feedback when its parent enrollment is removed.
