// =====================================================================
// GpaBadgeTagHelper — SESSION PROJECT (Rule 20/35/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 18 — Block 5
//
// This is the file where the framework calls YOUR code while it is
// building HTML. You have now met that shape three times:
//
//   Session 16   IRouteConstraint   — your code decides whether a URL
//                                     matches a route.
//   Session 17   ValidationAttribute — your code decides whether data
//                                     is acceptable.
//   Session 18   TagHelper           — your code decides what HTML an
//                                     element turns into.
//
// Every one of them is the same deal: a base class or interface
// Microsoft wrote, one method you override, and a registration step.
// Once you have seen it three times you should start EXPECTING it.
//
// ⚠️ AS SHIPPED, THIS TAG HELPER DOES NOTHING. Process() is empty, so
//    <gpa-badge /> renders as an empty, invisible element. That is
//    deliberate: the project has to build and run before any TODO is
//    done (Rule 39), and an empty override is the smallest thing that
//    compiles. TODO 7 gives it a body.
// =====================================================================

using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;

namespace StudentPortalWeb.TagHelpers
{
    // TODO 7: Turn this class into a real tag helper, in three steps.
    //
    //         Step one — claim the element name. Put an attribute above
    //         the class whose name means "this tag helper targets an
    //         HTML element", and pass it the element name you want to
    //         invent: the two words gpa and badge, joined by a hyphen,
    //         in quotes. Add a second, named argument to that same
    //         attribute stating the tag structure has no end tag, so
    //         the element can be written self-closing.
    //         ⚠️ The hyphen is not decoration. Razor only treats an
    //         element as a tag helper if the name cannot be confused
    //         with a real HTML element, and a hyphen guarantees that —
    //         the HTML standard reserves hyphenated names for custom
    //         elements precisely so this is safe.
    //
    //         Step two — declare what the element's attribute carries.
    //         Add one public property of type double called For, with
    //         both a getter and a setter. Razor maps an attribute
    //         written in lowercase on the element to a property written
    //         in PascalCase on the class, so an attribute called for
    //         fills a property called For. You write no parsing code.
    //
    //         Step three — fill in the Process method below. In order:
    //           1. Work out a Bootstrap colour class and a band name
    //              from the value in For, using the SAME three bands
    //              Session 16's route constraint used: three point five
    //              and above is first, three point zero up to but not
    //              including three point five is second, and anything
    //              lower is pass. Use the success, primary and secondary
    //              background classes respectively.
    //           2. Set the output's tag name to span, so the invented
    //              element is replaced by a real one the browser knows.
    //           3. Set the output's tag mode so it renders with both a
    //              start and an end tag — a span with no content and no
    //              closing tag would show nothing.
    //           4. Set a class attribute on the output combining the
    //              word badge with the colour class you chose.
    //           5. Set the output's content to the GPA formatted to two
    //              decimal places, then an em dash, then the band name.
    //              Format with the invariant culture, for the same
    //              reason Session 16's constraint did: this is markup,
    //              not something written in the user's local language.
    //
    //         ⚠️ Notice what you are NOT doing: touching the database,
    //         reading the request, or knowing which page you are on. A
    //         tag helper is handed a value and returns markup. Keeping
    //         it that dumb is what makes it reusable on every page.
    [HtmlTargetElement("gpa-badge" , TagStructure = TagStructure.WithoutEndTag)]
    public class GpaBadgeTagHelper : TagHelper
    {
        public double For { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string cssClass;
            string label;
            if(For >= 3.5) { cssClass = "bg-success"; label = "First"; }
            else if (For >= 3.0) { cssClass = "bg-primary"; label = "Second"; }
            else { cssClass = "bg-secondary"; label = "Pass"; }

            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute("class", $"badge {cssClass}");
            output.Content.SetContent($"{For.ToString("F2", CultureInfo.InvariantCulture)} - {label}");
        }
    }
}

#region 📋 Full TODO Checklist
// ---------------------------------------------------------------------
// Views/Students/Index.cshtml
//   TODO 1: A Razor code block, a printed summary, and an empty guard   [Block 1]
//   TODO 4: Replace the loop body with a call to the partial            [Block 3]
//
// Views/Shared/_Layout.cshtml
//   TODO 2: Add an optional named section to the layout                 [Block 2]
//
// Views/Students/Details.cshtml
//   TODO 3: Fill that section from one page only                        [Block 2]
//
// Views/Shared/_StudentRow.cshtml   (new file)
//   TODO 5: Build the partial: one strongly-typed table row             [Block 3]
//   TODO 6: Swap the typed URL for real tag helpers                     [Block 4]
//   TODO 9: Use your own tag helper in the GPA cell                     [Block 5]
//
// TagHelpers/GpaBadgeTagHelper.cs  (this file)
//   TODO 7: Write a tag helper the framework will call                  [Block 5]
//
// Views/_ViewImports.cshtml
//   TODO 8: Register your tag helper so Razor can see it                [Block 5]
// ---------------------------------------------------------------------
#endregion
