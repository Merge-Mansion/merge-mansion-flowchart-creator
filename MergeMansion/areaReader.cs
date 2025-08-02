using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

using static System.Net.Mime.MediaTypeNames;

namespace MergeMansion
{
    public class JsonReader
    {
        public static string UnescapeString(string text)
        {
            // Unescape HTML entities
            string unescapedHtml = System.Web.HttpUtility.HtmlDecode(text);
            // Replace \r\n with actual new lines
            string unescapedText = unescapedHtml.Replace("\\r\\n", "\r\n");
            // Unescape other escape sequences
            return Regex.Unescape(unescapedText);
        }

        public static AreaCollection ReadJson(string filePath)
        {
            string jsonData = File.ReadAllText(filePath);
            jsonData = Regex.Replace(jsonData, @"\\\""", "");
            jsonData = jsonData.Replace(@"\r\n", " ");
            jsonData = jsonData.Replace(@"\\", ""); // Targets specific sequences like {\"}; // Adjust this based on the specific issues you face
            string unescapedText = UnescapeString(jsonData);
            
            //var corrector = new TextCorrector();
            //string correctedText = corrector.CorrectText(jsonData);
            Debug.WriteLine(unescapedText.Substring(0, Math.Min(200, unescapedText.Length))); // Print first 100 characters

            return JsonConvert.DeserializeObject<AreaCollection>(unescapedText);
        }

    }
}
