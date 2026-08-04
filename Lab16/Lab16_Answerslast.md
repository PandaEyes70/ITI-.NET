# Lab 16 — Designing Your Own URLs (Routing & Constraints)
**Name:** Saif Elden Khaled Nazmy Lotfy  
**Lab ID:** 31  
**Course:** ITI Summer Training | Web Development Using .NET  

---

## 🔑 Your Personal Lab ID & Fingerprinted Values
* **Lab ID:** 31
* **MAX_YEAR:** `(31 mod 4) + 1` = `3 + 1` = **`4`**
* **MIN_GPA:** `2.5 + (31 mod 3) * 0.5` = `2.5 + 1 * 0.5` = **`3.0`**
* **INTAKE_CODE:** `iti` + `B` (since `31 mod 3` is `1`) = **`itiB`**

---

## 🛠️ PART A — Verify and Orient
1. **Verification:** Confirmed that `/`, `/students`, `/students/3`, `/students/year/2` and `/students/honours/first` all run successfully on the server.
2. **Comment added to `Program.cs`:**
   ```csharp
   // LAB 16 — Lab ID: 31 | MAX_YEAR = 4 | MIN_GPA = 3.0 | INTAKE_CODE = itiB
   ```
3. **Route Order Answer:** The `default` route sits at the bottom of the table because it is a very general catch-all that matches almost any URL pattern with 0 to 3 segments (due to defaults and the optional id). Since route matching is evaluated top-to-bottom and stops at the first matching pattern, placing the default route at the top would cause it to capture and misroute requests intended for more specific custom routes (e.g. `/students/3` or `/students/top/3`).

---

## 🛠️ PART B — A Second Address for a Page that Already Exists
* **Route Pattern:** `roster` -> mapped to `StudentsController.Index` (Index action of the Students controller).
* **Route Registration in `Program.cs`:**
  ```csharp
  app.MapControllerRoute(
      name: "rosterAlias",
      pattern: "roster",
      defaults: new { controller = "Students", action = "Index" });
  ```
* **Correct Placement Check:** Registered above the `default` route. Both `/roster` and `/students` render the student roster successfully. `/Home/Privacy` still works.
* **Is it acceptable for two different URLs to reach the same action?**  
  Yes, it is acceptable if they represent semantic synonyms or aliases for user convenience, marketing (e.g., promotional URLs), or backward compatibility, provided that a canonical link is defined in the HTML head to prevent duplicate content search engine indexing penalties.

---

## 🛠️ PART C — A Route with a Personalised Constraint
* **Action Signature:**
  ```csharp
  public async Task<IActionResult> Top(int count)
  ```
* **Top Action Code:**
  ```csharp
  public async Task<IActionResult> Top(int count)
  {
      // Guard clause: count must be between 1 and 4 inclusive (MAX_YEAR = 4)
      if (count < 1 || count > 4)
      {
          return NotFound();
      }

      var students = await _context.Students
          .OrderByDescending(s => s.Gpa)
          .Take(count)
          .ToListAsync();

      return View("Index", students);
  }
  ```
* **Route Registration in `Program.cs`:**
  ```csharp
  app.MapControllerRoute(
      name: "studentsTop",
      pattern: "students/top/{count:int:range(1,4)}",
      defaults: new { controller = "Students", action = "Top" });
  ```
* **Is your MAX_YEAR itself accepted, or rejected? Say why.**  
  Accepted. The `range(1,4)` constraint is inclusive of both endpoints (lower and upper bounds), meaning the value `4` (MAX_YEAR) is accepted, while `5` is rejected.
* **Console Logs for `/students/top/5` (404 check):**
  ```
  [START] Request path : /students/top/5
  [END] Request path : /students/top/5
  ```
  No EF query was executed. The request was immediately short-circuited by the action's guard clause when it fell through to the default route, conserving database connections.

---

## 🛠️ PART D — A Constraint You Write Yourself
* **Custom Constraint Class:** [IntakeCodeConstraint.cs](file:///c:/Users/PandaEyes/Desktop/Lab15/Lab16/Constraints/IntakeCodeConstraint.cs)
* **Match Method Code:**
  ```csharp
  public bool Match(
      HttpContext? httpContext,
      IRouter? route,
      string routeKey,
      RouteValueDictionary values,
      RouteDirection routeDirection)
  {
      if (!values.TryGetValue(routeKey, out var value) || value == null)
      {
          return false;
      }
      var code = Convert.ToString(value, CultureInfo.InvariantCulture);
      return string.Equals(code, "itiB", StringComparison.OrdinalIgnoreCase);
  }
  ```
* **Why must the constraint NOT touch the database?**  
  A constraint is evaluated on every single incoming request during route matching (including static file requests, icons, and bad URLs). Performing database queries inside `Match()` would cause massive performance bottlenecks and redundant connection overhead. Constraints should only validate the shape or value of route parameters, never check database state.
* **Constraint Map Registration in `Program.cs`:**
  ```csharp
  builder.Services.AddRouting(options =>
  {
      options.ConstraintMap.Add("honourBand", typeof(HonourBandConstraint));
      options.ConstraintMap.Add("intakecode", typeof(IntakeCodeConstraint));
  });
  ```
* **StudentsController Intake Action Code:**
  ```csharp
  public async Task<IActionResult> Intake(string code)
  {
      // Guard clause: code must match "itiB" case-insensitively
      if (string.IsNullOrWhiteSpace(code) || !string.Equals(code, "itiB", StringComparison.OrdinalIgnoreCase))
      {
          return NotFound();
      }

      var students = await _context.Students
          .OrderBy(s => s.FullName)
          .ToListAsync();

      return View("Index", students);
  }
  ```
* **Route Registration in `Program.cs`:**
  ```csharp
  app.MapControllerRoute(
      name: "studentsIntake",
      pattern: "students/intake/{code:intakecode}",
      defaults: new { controller = "Students", action = "Intake" });
  ```
* **Behavior Verification:**
  * `/students/intake/itiB` returns `200` (renders roster).
  * `/students/intake/ITIB` returns `200` (renders roster).
  * `/students/intake/itiA` throws `404` (no EF query executed).
  * `/students/intake/banana` throws `404` (no EF query executed).

---

## 🛠️ PART E — Your Own Address
* **StudentsController About Action Code:**
  ```csharp
  [Route("about/saif")]
  public async Task<IActionResult> About([FromQuery] double? minGpa)
  {
      double actualMinGpa = minGpa ?? 3.0; // Default to MIN_GPA = 3.0 for Lab ID 31

      var students = await _context.Students
          .Where(s => s.Gpa >= actualMinGpa)
          .OrderBy(s => s.FullName)
          .ToListAsync();

      return View("Index", students);
  }
  ```
* **Verification:**
  * `/about/saif` returns `200` (shows students with GPA ≥ 3.0).
  * `/about/saif?minGpa=3.9` returns `200` (shows students with GPA ≥ 3.9).
  * `/Students/About` returns `404`.
* **Why does `/Students/About` return 404?**  
  Because once an action is decorated with an attribute route (like `[Route("about/saif")]`), it is completely decoupled from conventional routing and will no longer respond to conventional patterns like `{controller}/{action}`.
* **Why does `minGpa` belong in the query string rather than in the path?**  
  Because `minGpa` is a filtering parameter (a view of the resources) rather than an identity of a resource. Changing `minGpa` filters the students, but we are still looking at the same page (about students), not a different entity. Therefore, it is a query concern, not a path concern.

---

## 🛠️ PART F — Wrap-Up Reflection
1. **Lab ID & Derived Values:**
   * Lab ID: 31
   * MAX_YEAR = `(31 mod 4) + 1` = `3 + 1` = `4`
   * MIN_GPA = `2.5 + (31 mod 3) * 0.5` = `2.5 + 1 * 0.5` = `3.0`
   * INTAKE_CODE = `iti` + `B` = `itiB`
2. **Step-by-step framework behavior for `/students/top/5`:**
   * The request is received and custom middleware prints `[START] Request path : /students/top/5`.
   * Routing checks the specific route `students/top/{count:int:range(1,4)}`. The segment `5` is checked against the range constraint, failing it.
   * Routing falls through to the `"default"` catch-all route, matching it as `controller = students`, `action = top`, `id = 5`.
   * MVC locates `StudentsController.Top` but binds `count` to `0` (default) because the parameter name is `count` and the route segment key is `id`.
   * The action begins executing, and immediately hits the guard clause `if (count < 1 || count > 4) { return NotFound(); }`. Since `0 < 1`, it returns a 404.
   * The response propagates back, printing `[END] Request path : /students/top/5`.
   * **What does NOT happen:** No EF database query is executed, as the action short-circuits at the guard clause.
3. **Built-in `int` vs. Custom `intakecode` Constraints:**
   * **Same:** Both are mapped to types implementing `IRouteConstraint` in the system's `ConstraintMap` and called inside `Match()` during speculative route checks.
   * **Different:** The built-in `int` constraint is pre-compiled by Microsoft, whereas our `intakecode` constraint was written by us in our local codebase and registered in `Program.cs`.
4. **Attribute Route Guarantee:**
   * It is a **guarantee** because it ensures strict URL contract enforcement, protecting the action from being accidentally exposed under conventional paths.
