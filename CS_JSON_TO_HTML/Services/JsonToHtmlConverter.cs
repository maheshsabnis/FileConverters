using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CS_JSON_TO_HTML.Services
{
    internal class JsonToHtmlConverter
    {
        public string ConvertJsonToHtml(JsonElement element)
        {
            StringBuilder sb = new StringBuilder();



            sb.Append("<html><head><style>");
            sb.Append("body { font-family: Arial; font-size: 14px; }");
            sb.Append("table { border-collapse: collapse; margin: 10px 0; }");
            sb.Append("td, th { border: 2px solid red; padding: 6px; }");
            sb.Append("th { background: grey; color: white;    }");
            sb.Append("</style></head><body>");



            sb.Append(ProcessJsonPropertyElement(element));



            sb.Append("</body></html>");
            return sb.ToString();
        }



        private string ProcessJsonPropertyElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    // Render as table
                    StringBuilder sbObj = new StringBuilder("<table>");
                    foreach (var property in element.EnumerateObject())
                    {
                        sbObj.Append("<tr>");
                        // Convert property name to PascalCase
                        string propertyName = $"{property.Name[0].ToString().ToUpper()}{property.Name[1..]}";
                        sbObj.Append($"<th>{propertyName}</th>");
                        sbObj.Append("<td>");
                        sbObj.Append(ProcessJsonPropertyElement(property.Value));
                        sbObj.Append("</td></tr>");
                    }
                    sbObj.Append("</table>");
                    return sbObj.ToString();

                case JsonValueKind.Array:
                    // Render arrays as list
                    StringBuilder sbArr = new StringBuilder("<ol>");
                    foreach (var item in element.EnumerateArray())
                    {
                        sbArr.Append("<li>");
                        sbArr.Append(ProcessJsonPropertyElement(item));
                        sbArr.Append("</li>");
                    }
                    sbArr.Append("</ol>");
                    return sbArr.ToString();

                case JsonValueKind.Null:
                    return "<i>null</i>";

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Number:
                    return element.GetRawText();

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetRawText();

                default:
                    return string.Empty;
            }
        }
    }
}
