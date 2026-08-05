# Lab 18 — Say It Once, and Invent Something (Views & Tag Helpers)
**Name:** Saif Elden Khaled Nazmy Lotfy  
**Lab ID:** 31  
**Course:** ITI Summer Training | Web Development Using .NET  

---

## 🔑 Your Personal Lab ID & Fingerprinted Values
* **Lab ID:** 31
* **CHIP_YEAR:** `(31 mod 4) + 1` = `3 + 1` = **`4`**
* **CHIP_LABEL:** **`Final`** (since CHIP_YEAR is 4)
* **First Name (lowercase):** **`saif`**

---

## 🛠️ PART A — Verify and Orient
1. **Verification:** Confirmed that the summary line on `/students`, the blue alert on `/students/3` only, `_StudentRow.cshtml` in use, and coloured GPA badges all run successfully.
2. **Comment added to `Views/Shared/_StudentRow.cshtml`:**
   ```cshtml
   @* LAB 18 — Lab ID: 31 | CHIP_YEAR = 4 | CHIP_LABEL = Final *@
   ```
3. **Model Scoping Answer:**  
   The type of the `Model` property in any view or partial view is determined by the `@model` directive declared at the top of the file. `Index.cshtml` has `@model List<Student>`, making its `Model` a collection (list) of students. In contrast, `_StudentRow.cshtml` has `@model Student`, which configures its `Model` to be a single student instance passed by the parent view.

---

## 🛠️ PART B — Say it once, everywhere
* **Deduplication Check:**
  * Cleaned up `ByYear.cshtml`, `Honours.cshtml`, and `Searching.cshtml` by removing the commented out duplicate `<tr>` row markup blocks, replacing them with calls to the partial: `<partial name="_StudentRow" model="student" />`.
  * Verified that `/students/year/2`, `/students/honours/first`, and `/students/search?name=a` render perfectly.
* **ByYear.cshtml Razor Answer Comment:**
  ```cshtml
  @*
    Part B Question: What would have happened if you had written <partial name="_StudentRow" /> with no model attribute?
    Answer: The partial view would inherit the parent view's model automatically. Here, the parent model is a List<Student>, but _StudentRow.cshtml expects a single Student object as its model, resulting in an InvalidOperationException at runtime due to model type mismatch.
  *@
  ```
* **Verify Search Results:** The phrase `@Model.YearOfStudy` or `@student.YearOfStudy` has been removed from all student views except [_StudentRow.cshtml](file:///c:/Users/PandaEyes/Desktop/Lab15/Lab18/Views/Shared/_StudentRow.cshtml), establishing a single source of truth for the row markup.

---

## 🛠️ PART C — A second partial, a different shape
* **Partial Card Code (`Views/Shared/_StudentCard.cshtml`):**
  Created form file [_StudentCard.cshtml](file:///c:/Users/PandaEyes/Desktop/Lab15/Lab18/Views/Shared/_StudentCard.cshtml):
  ```html
  @model Student
  @*
      LAB 18 — Part C: A second partial view (_StudentCard.cshtml)
      Question: this partial and _StudentRow.cshtml are both handed one Student. Could one page use both at once? Why or why not?
      Answer: Yes, a single page can use both partial views at once. They both accept the same model type (Student) but produce completely distinct HTML markup representation shapes (a table row vs. a Bootstrap card), allowing them to co-exist on the same page (e.g. rendering a detailed card of a featured student above a list table).
  *@
  <div class="card" style="width: 18rem; margin-bottom: 1.5rem; box-shadow: 0 4px 6px rgba(0,0,0,0.1);">
      <div class="card-body">
          <h5 class="card-title">@Model.FullName</h5>
          <h6 class="card-subtitle mb-2 text-muted">Student ID: @Model.Id</h6>
          <p class="card-text">
              <strong>Year:</strong> <year-chip for="@Model.YearOfStudy" />
              <br />
              <strong>GPA:</strong> <gpa-badge for="@Model.Gpa" />
          </p>
      </div>
  </div>
  ```
* **Details.cshtml updates:** Replaced the plain tables in [Details.cshtml](file:///c:/Users/PandaEyes/Desktop/Lab15/Lab18/Views/Students/Details.cshtml) with `<partial name="_StudentCard" model="Model" />`.
* **Testing:** Visiting `/students/3` renders a Bootstrap card with the coloured GPA badge inside, replacing the old key-value details table.

---

## 🛠️ PART D — Your own tag helper
* **Tag Helper Code (`TagHelpers/YearChipTagHelper.cs`):**
  Created [YearChipTagHelper.cs](file:///c:/Users/PandaEyes/Desktop/Lab15/Lab18/TagHelpers/YearChipTagHelper.cs):
  ```csharp
  using Microsoft.AspNetCore.Razor.TagHelpers;

  namespace StudentPortalWeb.TagHelpers
  {
      [HtmlTargetElement("year-chip", TagStructure = TagStructure.WithoutEndTag)]
      public class YearChipTagHelper : TagHelper
      {
          public int For { get; set; }

          public override void Process(TagHelperContext context, TagHelperOutput output)
          {
              string cssClass;
              string label;

              // Lab ID 31 -> CHIP_YEAR = 4, CHIP_LABEL = "Final"
              if (For == 4)
              {
                  cssClass = "bg-warning text-dark";
                  label = "Final";
              }
              else
              {
                  cssClass = "bg-light text-dark";
                  label = $"Year {For}";
              }

              output.TagName = "span";
              output.TagMode = TagMode.StartTagAndEndTag;
              output.Attributes.SetAttribute("class", $"badge {cssClass}");
              output.Attributes.SetAttribute("title", "rendered by saif");
              output.Content.SetContent(label);
          }
      }
  }
  ```
* **Verification in Views:** Placed `<year-chip for="@Model.YearOfStudy" />` in the Year column of `_StudentRow.cshtml`.
* **Roster Page Visuals:** Year-4 students render a yellow **`Final`** badge, and all other students render a light-grey **`Year n`** badge.

---

## 🛠️ PART E — Prove it, and break it
1. **WHAT THE BROWSER RECEIVED (HTML Source):**
   ```html
   <span class="badge bg-warning text-dark" title="rendered by saif">Final</span>
   ```
2. **Broken State Symptoms:**
   * **(a) Removing `@addTagHelper` from `_ViewImports.cshtml`:**
     * *Symptom:* The browser receives the raw custom XML tag `<year-chip for="4"></year-chip>` in the page source and displays nothing on screen, because standard browsers do not recognize this custom tag.
   * **(b) Removing `output.TagMode` from `YearChipTagHelper.cs`:**
     * *Symptom:* The browser receives a self-closing HTML tag `<span class="badge bg-warning text-dark" title="rendered by saif" />` without a closing `</span>` block, rendering an empty, invisible badge containing no content.
   * **(c) Renaming element to `yearchip` (no hyphen):**
     * *Symptom:* The browser receives the custom tag `<yearchip for="4"></yearchip>` in the source page and displays nothing, because Razor fails to map custom elements to Tag Helpers unless the element name contains at least one hyphen (to prevent conflicts with standard HTML elements).
3. **What all three failures have in common:**
   All three failures fail silently and invisibly without throwing any exceptions or generating compilation/run-time errors, passing unrecognized tags to the browser.

---

## 🛠️ PART F — Wrap-Up Reflection
1. **Derived Values:** Lab ID: 31 | CHIP_YEAR: 4 | CHIP_LABEL: Final | First Name: saif.
2. **Duplication Parallel:** In Session 15, the database connection string and options configuration were duplicated between the `StudentPortalContext` class and `Program.cs`. This was solved by configuring options inside `Program.cs` and injecting `DbContextOptions` into the context constructor via ASP.NET Core Dependency Injection.
3. **Shared Extensibility Pattern:** All three extensibility points (route constraints, validation attributes, and tag helpers) require us to inherit from a framework base class (`TagHelper` or `ValidationAttribute`) or implement a framework interface (`IRouteConstraint`) and override a single core method (`Match()`, `IsValid()`, or `Process()`). We then register this custom class (in `Program.cs`, the Model class, or `_ViewImports.cshtml`), allowing the ASP.NET Core engine to call our custom implementation dynamically.
4. **Rule vs. Label Duplication:**
   * `<gpa-badge>` contains a **rule** (GPA ranges for grades).
   * `<year-chip>` contains a **label** (string representation of years).
   * I would be much more nervous about duplicating the **rule** in views because GPA boundary calculations represent business logic. Duplicating rules makes future modifications (e.g. raising the GPA required for a "First") highly error-prone and likely to result in silent display inconsistencies across the application.
