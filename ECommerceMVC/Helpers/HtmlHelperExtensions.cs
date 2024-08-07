using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Text.Encodings.Web;

namespace ECommerceMVC.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string DisabledOption(this IHtmlHelper htmlHelper, int value, string text, bool isDisabled)
        {
            var option = new TagBuilder("option");
            option.Attributes.Add("value", value.ToString());
            if (isDisabled)
            {
                option.Attributes.Add("disabled", "disabled");
            }
            option.InnerHtml.Append(text);

            using (var writer = new StringWriter())
            {
                option.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }
    }
}
