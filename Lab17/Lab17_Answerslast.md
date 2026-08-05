# Lab 17 — Controllers & Actions
**Name:** Saif Elden Khaled Nazmy Lotfy  
**Lab ID:** 31  
**Course:** ITI Summer Training | Web Development Using .NET  

---

## 🔑 Your Personal Lab ID & Fingerprinted Values
* **Lab ID:** 31
* **MIN_GPA_EDIT:** `2.0 + (31 mod 5) * 0.3` = `2.0 + 1 * 0.3` = **`2.3`**
* **MAX_YEAR_EDIT:** `(31 mod 3) + 2` = `1 + 2` = **`3`**

---

## 🛠️ PART A — Verify and Orient
1. **Verification:** Confirmed that `/students`, `Details` page, and the `Create` form load and execute successfully.
2. **Comment added to `StudentsController.cs`:**
   ```csharp
   // LAB 17 — Lab ID: 31 | MIN_GPA_EDIT = 2.3 | MAX_YEAR_EDIT = 3
   ```
3. **Overload Async Answer:**  
   The `Create()` GET overload is not marked `async` because it does not make any I/O calls or access the database (it simply returns an empty form `View()`), so there is no task to await. In contrast, the POST overload is marked `async` because it saves the new student to the database using Entity Framework's asynchronous methods `AddAsync()` and `SaveChangesAsync()`, which must be awaited.

---

## 🛠️ PART B — The GET half: show the real row
* **GET Action Code (`StudentsController.cs`):**
  ```csharp
  // Why does this action load the student from the database at all, instead of just rendering an empty form the way Create()'s GET half does?
  // Answer: It loads the existing student from the database to pre-populate the form inputs with their current values, allowing the user to see what they are editing, whereas Create is for a new record and starts with an empty form.
  [HttpGet]
  public async Task<IActionResult> Edit(int id)
  {
      var student = await _context.Students
          .FirstOrDefaultAsync(s => s.Id == id);

      if (student is null)
      {
          return NotFound();
      }

      return View(student);
  }
  ```
* **Edit View (`Views/Students/Edit.cshtml`):**
  Created form file [Edit.cshtml](file:///c:/Users/PandaEyes/Desktop/Lab15/Lab17/Views/Students/Edit.cshtml) featuring `FullName`, `YearOfStudy`, and `Gpa` inputs, as well as a hidden input carrying the student's `Id`.
* **Testing:** Visiting `/students/edit/3` successfully returns the student details pre-filled. Visiting `/students/edit/9999` returns a 404 response.

---

## 🛠️ PART C — The POST half: save the change, properly
* **POST Action Code (`StudentsController.cs`):**
  ```csharp
  // What would happen to this action's signature if you renamed its id parameter to studentId and left the route pattern saying {id}?
  // Answer: The model binder matches parameters by name. If the parameter is renamed to studentId but the route specifies {id}, the binder will search route values for "studentId", find nothing, and silently default studentId to 0, which would fail the guard clause (studentId != student.Id).
  [HttpPost]
  public async Task<IActionResult> Edit(int id, Student student)
  {
      // Guard clause: validate id match and ModelState
      if (id != student.Id || !ModelState.IsValid)
      {
          return View(student);
      }

      var existingStudent = await _context.Students
          .FirstOrDefaultAsync(s => s.Id == id);

      if (existingStudent is null)
      {
          return NotFound();
      }

      // Update properties
      existingStudent.FullName = student.FullName;
      existingStudent.YearOfStudy = student.YearOfStudy;
      existingStudent.Gpa = student.Gpa;

      _context.Students.Update(existingStudent);
      await _context.SaveChangesAsync();

      TempData["Message"] = $"{student.FullName} was updated successfully.";

      // Part E: Redirect to SaifConfirmed confirmation action
      return RedirectToAction("SaifConfirmed", new { id = student.Id });
  }
  ```
* **F5 Behavior Check:** When saving a change, the application redirects the browser to the confirmation action `/Students/SaifConfirmed/3`. Pressing F5 on that page simply requests the confirmation GET action again and does **not** insert or update database rows redundantly.

---

## 🛠️ PART D — Your own validation boundaries
* **Model Class Validation Changes (`Models/StudentPortalContext.cs`):**
  ```csharp
  // Answer: [Range] is a validation-only attribute. You can prove this to someone by showing that changing the range limits or error message does not prompt Entity Framework to detect any changes (running Add-Migration would result in an empty migration with no table or column alterations), whereas modifying [Required] or [MaxLength] changes table metadata and generates structural schema changes.
  public class Student
  {
      public int Id { get; set; }

      [Required]
      [MaxLength(100)]
      public string FullName { get; set; } = "";

      [Range(1, 3, ErrorMessage = "Year of study must be between 1 and 3.")]
      public int YearOfStudy { get; set; }
      
      [Range(2.3, 4.0, ErrorMessage = "GPA must be between 2.30 and 4.00.")]
      public double Gpa { get; set; }
  }
  ```
* **Validation Verification:**
  * Submitting GPA `2.2` (below `2.3` MIN_GPA_EDIT) rejects the form with validation error message: *"GPA must be between 2.30 and 4.00."*
  * Submitting GPA `2.3` is accepted and successfully saves the student.
  * Submitting Year `4` (above `3` MAX_YEAR_EDIT) rejects the form with validation error message: *"Year of study must be between 1 and 3."*
* **Security & Performance Logs:** In console, when the validation fails, the logging middleware prints `[START]` and `[END]` with **nothing** between them (no SQL INSERT or UPDATE), demonstrating that invalid data is rejected in-memory before it reaches SQL Server.

---

## 🛠️ PART E — One action that carries your own name
* **SaifConfirmed Action Code (`StudentsController.cs`):**
  ```csharp
  // Question: why must this action reload the student from the database rather than simply reusing the student object the POST action already had in memory?
  // Answer: Reloading from the database guarantees that we display the actual committed state of the data in the database, verifying that the transaction was successfully persisted.
  [HttpGet]
  public async Task<IActionResult> SaifConfirmed(int id)
  {
      var student = await _context.Students
          .FirstOrDefaultAsync(s => s.Id == id);

      if (student is null)
      {
          return NotFound();
      }

      return View("Details", student);
  }
  ```
* **Wired Redirect:** On a successful POST in `Edit`, the controller returns `RedirectToAction("SaifConfirmed", new { id = student.Id });` which redirects the browser to `http://localhost:5084/Students/SaifConfirmed/3`.

---

## 🛠️ PART F — Wrap-Up Reflection
1. **Lab ID & Derived Values:**
   * Lab ID: 31
   * MIN_GPA_EDIT = `2.3`
   * MAX_YEAR_EDIT = `3`
2. **Input Rejection Checkpoints in Order:**
   1. **Route Constraints** (e.g. `{id:int}` matching the URL parameter types).
   2. **Model Validation / ModelState** (e.g. properties checked against `[Range]` before action execution).
   3. **Database Constraints** (e.g. database schema checks, executed at `SaveChangesAsync`).
   * Our Part D validation attributes live in checkpoint 2 (**Model Validation / ModelState**).
3. **Browser PRG Flow:**
   * Redirection (Post/Redirect/Get) returns a response with status 302 and a redirect location. The browser reads this and automatically sends an idempotent `GET` request to the new URL, updating the address bar.
   * Pressing F5 re-sends the last request, which is a safe, read-only `GET` request, avoiding the resubmission of POST form payloads and preventing duplicate database rows.
4. **Schema vs. Validation Attributes:**
   * **Same:** Both are declared as metadata attributes directly above property fields in C# model classes.
   * **Different:** Schema attributes (`[Required]`, `[MaxLength]`) define database column structures (nullable/not-null, length) and generate SQL changes in migrations, whereas validation attributes (`[Range]`) are validation-only rules evaluated by the MVC model validation system in C# code, generating no database schema adjustments or migrations.
