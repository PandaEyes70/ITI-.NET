# Lab 15 — From Console to Web: MVC, Dependency Injection, and the Pipeline
**Name:** Saif Elden Khaled Nazmy Lotfy  
**Lab ID:** 31  
**Course:** ITI Summer Training | Web Development Using .NET  

---

## 🛠️ PART A — Scaffold the Web Project
* **Default URL Opened:** `http://localhost:5114` (non-HTTPS) and `https://localhost:7037` (HTTPS)
* **File Containing Port Numbers:** `Properties/launchSettings.json` (specifically under the `applicationUrl` properties of the profiles).

---

## 🛠️ PART B — Predict Before You Run

### B.1 Prediction vs. Actual
* **Prediction:** The app will compile successfully because the syntax is correct. However, it will fail at startup when `AddControllersWithViews()` is called after `Build()`. This is because the service collection is marked as read-only once the container has been built.
* **Actual Result:** The app compiled successfully, but crashed on startup with the following exception:
  ```
  Unhandled exception. System.InvalidOperationException: The service collection cannot be modified because it is read-only.
     at Microsoft.Extensions.DependencyInjection.ServiceCollection.ThrowReadOnlyException()
     ...
     at Lab15_StudentPortalWeb.Program.Main(String[] args)
  ```
  The prediction was **correct**.

### B.2 Prediction vs. Actual
* **Prediction:** The failure will occur at request-time (when the page is visited) rather than at app startup. This is because ASP.NET Core resolves controllers on-demand per request.
* **Actual Result:** The application started successfully and listened on its ports. The crash only occurred when a request was sent to the home page, yielding a 500 error in the browser and the following console exception:
  ```
  System.InvalidOperationException: Unable to resolve service for type 'Lab15_StudentPortalWeb.Models.StudentPortalContext' while attempting to activate 'Lab15_StudentPortalWeb.Controllers.HomeController'.
  ```
  The prediction was **correct**.

### B.3 Prediction vs. Actual
* **Prediction:** The custom middleware will run **1 time** for the home page load. Although the browser requests both `/` (HTML) and static resources (like `/css/site.css`), the static files will be served and short-circuited by the static files middleware, never reaching the custom middleware placed after it.
* **Actual Result:** The custom middleware printed logs only for `/`. The assets were short-circuited and did not trigger the log. The prediction was **correct**.

---

## 🛠️ PART C — Wire the Real Context Through DI
* **Build Number:** `(31 × 7) + 100` = **317**
* **Heading Rendered:** `Student Portal — Build #317`
* ** Roster Count:** 4 students are shown on the page (Yara Adel, Omar Hesham, Nada Samir, Kareem Fouad).
* **Database Verification:** Running `SELECT COUNT(*) FROM Students` in SSMS returns **4**. The count matches the web page roster exactly.

---

## 🛠️ PART D — Break It On Purpose

### D.2 Commented-Out Registration Error
* **Verbatim Exception Type:** `System.InvalidOperationException`
* **Verbatim Message (First Sentence):** `Unable to resolve service for type 'Lab15_StudentPortalWeb.Models.StudentPortalContext' while attempting to activate 'Lab15_StudentPortalWeb.Controllers.HomeController'.`
* **Timing of Failure:** The failure occurs at request time when the user visits the home page, not at startup. This confirms the B.2 prediction.

### D.5 Singleton Registration Behavior
* **Verbatim Startup/Runtime Behavior:** The app started up perfectly and served the page normally. No exceptions were thrown on startup or on the page request.

### D.6 Dangers of Singleton DbContext
* **Is this good or bad news?** This is **bad news**.
* **Explanation:** A `DbContext` is not thread-safe and is designed to have a scoped lifetime. Under multi-user concurrent loads in production, concurrent requests will share the same Singleton context instance, leading to database concurrency exceptions (e.g., `ConcurrentAccessException` / `InvalidOperationException: A second operation was started on this context...`) and potential data corruption. This bug remains invisible during single-user testing but will crash the app under real load.

---

## 🛠️ PART E — The Lifetime Experiment
* **Assigned Lifetime (Lab ID 31):** **Scoped** (`31 mod 3` = `1`)
* **DI Registration Line:** 
  `builder.Services.AddScoped<ISaifStampService, SaifStampService>();`

### Observation Table
| Load / Refresh | Stamp A | Stamp B |
|---|---|---|
| **First Load** | `3e353823` | `3e353823` |
| **Second Load** | `a9ef2808` | `a9ef2808` |

### Lifetime Analysis
1. **Did Stamp A and Stamp B match within a single load? Why?**  
   Yes. Under the **Scoped** lifetime, the DI container instantiates a single instance of the service per HTTP request. Since both parameter injections occurred within the scope of the same request, they resolved to the same object instance, sharing the same `Stamp` value.
2. **Did the stamps change between loads? Why?**  
   Yes. When the page is refreshed, a new HTTP request scope is created. The DI container disposes the old scoped instances and instantiates a new `SaifStampService` instance, resulting in a fresh GUID substring.
3. **Neighbour Comparison:**  
   * **(Transient):** Within a single load, Stamp A and Stamp B did NOT match because Transient creates a new instance for every injection request. Refreshing the page generated two entirely new, non-matching stamps.
   * **(Singleton):** Within a single load, Stamp A and Stamp B matched. Additionally, the stamps did NOT change across refreshes; they remained identical across all page loads because a single instance is shared globally for the app's lifetime.

---

## 🛠️ PART F — The Pipeline, Observed
* **Audit Path:** `/audit-31`

### F.3 Console Output Log (First Run - Middleware at the Front)
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5114
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Users\PandaEyes\Desktop\Lab15\Lab15\Lab15_StudentPortalWeb
[START] Request path: /
warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]
      Failed to determine the https port for redirect.
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (46ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT [s].[Id], [s].[FullName], [s].[Gpa], [s].[YearOfStudy]
      FROM [Students] AS [s]
      ORDER BY [s].[FullName]
[END] Request path: /
[START] Request path: /lib/bootstrap/dist/css/bootstrap.min.css
[START] Request path: /css/site.css
[END] Request path: /css/site.css
[START] Request path: /lib/jquery/dist/jquery.min.js
[END] Request path: /lib/bootstrap/dist/css/bootstrap.min.css
[END] Request path: /lib/jquery/dist/jquery.min.js
[START] Request path: /lib/bootstrap/dist/js/bootstrap.bundle.min.js
[END] Request path: /lib/bootstrap/dist/js/bootstrap.bundle.min.js
[START] Request path: /js/site.js
[END] Request path: /js/site.js
[START] Request path: /favicon.ico
[END] Request path: /favicon.ico
```

### F.4 Analysis
* **Number of `[START]` lines:** 7 lines appeared.
* **Paths Listed:**
  1. `/`
  2. `/lib/bootstrap/dist/css/bootstrap.min.css`
  3. `/css/site.css`
  4. `/lib/jquery/dist/jquery.min.js`
  5. `/lib/bootstrap/dist/js/bootstrap.bundle.min.js`
  6. `/js/site.js`
  7. `/favicon.ico`
* **Explanation:** When the browser loads the main page (`/`), it parses the returned HTML document and automatically sends separate, subsequent HTTP requests to fetch all referenced stylesheets, scripts, and media resources.

### F.5 Audit Path Request Logs
* **Console Output:**
  ```
  [START] Request path: /audit-31
  [AUDIT] Saif Elden Khaled Nazmy Lotfy saw a request for /audit-31
  [END] Request path: /audit-31
  ```
* **Explanation:** The middleware pipeline executes for all incoming HTTP requests before routing takes place. Even though `/audit-31` does not match any route/controller and returns a 404, it still flows through the pipeline and triggers the middleware.

### F.7 Console Output Log (Second Run - Middleware after UseStaticFiles)
```
[START] Request path: /
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (2ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT [s].[Id], [s].[FullName], [s].[Gpa], [s].[YearOfStudy]
      FROM [Students] AS [s]
      ORDER BY [s].[FullName]
[END] Request path: /
```
* **Disappeared Paths:** `/lib/bootstrap/dist/css/bootstrap.min.css`, `/css/site.css`, `/lib/jquery/dist/jquery.min.js`, `/lib/bootstrap/dist/js/bootstrap.bundle.min.js`, `/js/site.js`, and `/favicon.ico`.
* **Explanation:** By placing the custom middleware *after* `app.UseStaticFiles()`, requests for static files are intercepted and short-circuited by the static files middleware. They return early and never propagate to subsequent middleware in the pipeline, which matches the Part B.3 prediction.

---

## 🛠️ PART G — Reflection

1. **Explain why making the class *less* capable made the application *more* flexible:**  
   By removing the database connection configuration from `StudentPortalContext` (making it "less capable" of self-configuration), we decoupled the context from a hardcoded environment. The connection settings are now passed in from outside via DI options. This allows the application to dynamically direct the exact same context class to different databases (e.g., local database for development, an in-memory database for testing, or a remote production server) without altering the C# code of the context class.

2. **Why is failing at startup better than failing on the first user request?**  
   Startup failures are fail-fast mechanisms. They surface configuration and wiring issues immediately when the application is launched, alerting the developer or deployment pipeline before any user hits the site. In contrast, request-time failures are silent landmines that only explode when a specific path is requested, allowing broken code to pass CI/CD tests and crash in production.

3. **Name the two earlier bugs in this course that had the same shape:**  
   * **Session 13:** Multiple Enumeration Bug (queries are re-evaluated silently, causing performance issues without throwing exceptions).
   * **Session 14:** Silent Save Failure (using `AsNoTracking()` prevents EF from tracking changes, leading to `SaveChangesAsync()` quietly completing with zero modifications).

4. **What would go wrong if both projects (Console and Web) tried to manage migrations?**  
   They would generate separate, diverging migration histories that conflict with each other. When trying to apply migrations, both projects would attempt to create or modify the same database schema blocks independently, resulting in database lockouts, column collision errors, or database state corruption.
