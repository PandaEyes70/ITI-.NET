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
