// =====================================================================
// GpaBadgeTagHelper — CARRIED FORWARD FROM SESSION 18 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
//
// Finished, unchanged. Today (Block 4) it gets used a THIRD context it
// was never written for — a Grade column on an Enrollment, not a
// Student's Gpa — which is the whole point of a tag helper being a
// dumb, reusable component: it does not know or care where its number
// came from.
// =====================================================================

using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;

namespace StudentPortalWeb.TagHelpers
{
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
