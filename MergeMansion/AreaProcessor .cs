using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using Newtonsoft.Json;
using csdot;
using csdot.Attributes.DataTypes;
using QuickGraph;
using QuickGraph.Graphviz;
using System.Xml;
using System.Diagnostics;
using System.Data.SqlTypes;
using csdot.Attributes.Types;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using MergeMansion.Services;
using MergeMansion.Models;
using static MergeMansion.MergeType;
using Root = MergeMansion.MergeType.Root;

namespace MergeMansion
{
    internal class AreaProcessor
    {
        private Root _root;
        private readonly Services.TaskParser _taskParser;

        public AreaProcessor(Root root)
        {
            _root = root;
            _taskParser = new TaskParser(); // Initialize TaskParser            
        }

        public List<TaskWithArea> ProcessArea(AreaData area)
        {
            string taskDependencies = area.TaskDependencies;
            List<HotspotRef> taskHotspotsRefs = area.HotspotsRefs;

            //if (area.Name.Contains("Library"))
            //{
            //    Debug.WriteLine(area.Name);
            //}
            //else
            //{
            //    return null;
            //}

            // Unescape the text
            string unescapedText = UnescapeText(taskDependencies);

            // Format the text and extract relationships
            List<Models.Task> tasks;
            //tasks = _taskParser.ParseTasks(unescapedText, taskHotspotsRefs);
            tasks = _taskParser.ParseTasks2(unescapedText, taskHotspotsRefs, _root);
            List<string> relationships = _taskParser.ParseRelationship(unescapedText);

            // Create the Graphviz graph and save as SVG
            CreateGraph(tasks, relationships, area.Name);

            // Filter tasks with items that are empty strings and create TaskWithArea objects
            //var emptyTasks = tasks.Where(task => task.Items.Any(item => item == ""))
            //.Select(task => new TaskWithArea(task, area.Name))
            //.ToList();

            //jsonOutput = JsonConvert.SerializeObject(emptyTasks[1].Task, Newtonsoft.Json.Formatting.Indented);

            return null;
        }
        private string UnescapeText(string text)
        {
            // Unescape HTML entities
            string unescapedHtml = System.Web.HttpUtility.HtmlDecode(text);
            // Replace \r\n with actual new lines
            string unescapedText = unescapedHtml.Replace("\\r\\n", "\r\n");
            // Unescape other escape sequences
            return Regex.Unescape(unescapedText);
        }

        public class Node
        {
            public int Id { get; set; }
            public string TaskName { get; set; }
            public string Description { get; set; }
            public List<Requirement> Requirements { get; set; }
        }

        public class Requirement
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
        }
        public (string jsonOutput, List<string> relationships) GenerateJson2(string text)
        {
            var nodes = new List<Node>();
            var relationships = new List<string>();

            // Split the text into lines using semicolons
            var lines = text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();

                // Skip non-node and non-relationship lines
                if (trimmedLine.StartsWith("digraph") || trimmedLine.StartsWith("rankdir") || trimmedLine.StartsWith("node"))
                    continue;

                // Process relationships
                if (trimmedLine.Contains("->"))
                {
                    relationships.Add(trimmedLine);
                    continue;
                }

                // Process node lines
                if (trimmedLine.Contains("[label="))
                {
                    // Extract the node ID and label content using regex
                    var nodeMatch = Regex.Match(trimmedLine, @"(\d+)\[label=(.+?)\]");

                    if (nodeMatch.Success)
                    {
                        int id = int.Parse(nodeMatch.Groups[1].Value);
                        string labelContent = nodeMatch.Groups[2].Value;

                        // Split the label content into parts
                        var labelParts = labelContent.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        // Extract TaskName (first part)
                        string taskName = labelParts[0];

                        // Initialize variables for description and requirements
                        string description = "";
                        var requirements = new List<Requirement>();

                        // Index to keep track of current position in labelParts
                        int index = 1;

                        // Build description until we hit the requirements (indicated by "x" followed by a number)
                        while (index < labelParts.Count && !Regex.IsMatch(labelParts[index], @"^x\d+$"))
                        {
                            description += labelParts[index] + " ";
                            index++;
                        }

                        // Trim the description
                        description = description.Trim();

                        // Parse requirements
                        while (index < labelParts.Count)
                        {
                            // Expecting pairs of requirement name and quantity
                            if (index + 1 < labelParts.Count && Regex.IsMatch(labelParts[index + 1], @"^x\d+$"))
                            {
                                string requirementName = labelParts[index];
                                int quantity = int.Parse(labelParts[index + 1].Substring(1));

                                requirements.Add(new Requirement
                                {
                                    Name = requirementName,
                                    Quantity = quantity
                                });

                                index += 2; // Move past the requirement name and quantity
                            }
                            else
                            {
                                // Handle multi-word requirement names
                                string requirementName = labelParts[index];
                                index++;
                                while (index < labelParts.Count && !Regex.IsMatch(labelParts[index], @"^x\d+$"))
                                {
                                    requirementName += " " + labelParts[index];
                                    index++;
                                }

                                // Get the quantity
                                if (index < labelParts.Count && Regex.IsMatch(labelParts[index], @"^x\d+$"))
                                {
                                    int quantity = int.Parse(labelParts[index].Substring(1));
                                    requirements.Add(new Requirement
                                    {
                                        Name = requirementName,
                                        Quantity = quantity
                                    });
                                    index++;
                                }
                                else
                                {
                                    // No quantity found, exit to prevent infinite loop
                                    break;
                                }
                            }
                        }

                        // Add the node to the list
                        nodes.Add(new Node
                        {
                            Id = id,
                            TaskName = taskName,
                            Description = description,
                            Requirements = requirements
                        });
                    }
                }
            }

            // Serialize the nodes to JSON
            var jsonOutput = System.Text.Json.JsonSerializer.Serialize(nodes, new JsonSerializerOptions { WriteIndented = true });
            return (jsonOutput, relationships);
        }
        private (string jsonOutput, List<string> relationships) GenerateJson(string text)
        {
            // Use a regular expression to match and extract each label
            //string pattern = @"(\d+)\[label=""([^""]+)""]";
            //pattern = @"(\d+)\[label=""([^""]+)"";";            
            Debug.WriteLine($"Text: {text}");
            string pattern = @"(\d+)\[label=([^;]+);";
            var matches = Regex.Matches(text, pattern);

            var tasks = new List<Dictionary<string, object>>();
            var relationships = new List<string>();

            foreach (Match match in matches)
            {
                string id = match.Groups[1].Value;
                string label = match.Groups[2].Value;

                // Split the label into lines
                string[] lines = label.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                string taskID = lines[0].Trim();
                string taskName = lines.Length > 1 && !string.IsNullOrEmpty(lines[1].Trim()) ? lines[1].Trim() : taskID; // Use taskID if taskName is null or empty

                string[] items = new string[0]; // Initialize items array

                // Only process items if taskName is not the same as taskID
                if (taskName != taskID)
                {
                    // Separate the task name from the first item using various patterns
                    var taskAndFirstItem = Regex.Split(taskName, @"(?<=[a-z])(?=[A-Z])|(?<=\.)\s*(?=[A-Z])|(?<=\?)\s*(?=[A-Z])|(?<=!)\s*(?=[A-Z])");
                    taskName = taskAndFirstItem[0].Trim();
                    string itemsText = taskAndFirstItem.Length > 1 ? taskAndFirstItem[1].Trim() : "";

                    // Separate items using the pattern of a number followed by an uppercase letter
                    items = Regex.Split(itemsText, @"(?<=\d)(?=[A-Z])").Select(item => item.Trim()).ToArray();

                    // Extract items and quantities
                    string itemData = string.Join(", ", items.Select(item =>
                    {
                        var parts = Regex.Match(item, @"(\d+)\s*(.*)");
                        if (parts.Success)
                        {
                            return $"{{'Item': '{parts.Groups[2].Value.Trim()}', 'Qty': {parts.Groups[1].Value.Trim()}}}";
                        }
                        return $"{{'Item': '{item}', 'Qty': 1}}"; // Default case if split fails
                    }));
                }

                // Create the task dictionary
                var task = new Dictionary<string, object>
                {
                { "id", int.Parse(id) },
                { "taskID", taskID },
                { "taskName", taskName },
                { "items", items }
                };

                tasks.Add(task);
            }

            // Use a regular expression to match and extract each relationship
            string relationshipPattern = @"(\d+)->(\d+);";
            var relationshipMatches = Regex.Matches(text, relationshipPattern);

            foreach (Match match in relationshipMatches)
            {
                string from = match.Groups[1].Value;
                string to = match.Groups[2].Value;
                relationships.Add($"{from}->{to}");
            }

            // Convert the list of tasks to JSON
            string jsonOutput = JsonConvert.SerializeObject(tasks, Newtonsoft.Json.Formatting.Indented);

            return (jsonOutput, relationships);
        }
        private void CreateGraph(List<Models.Task> tasks, List<string> relationships, string areaName)
        {
            // Step 1: Create the .dot file content
            if (areaName.Contains("Tomb"))
            {
                Debug.WriteLine(areaName);
            }

            var dotContent = new StringBuilder();
            dotContent.AppendLine("digraph G {");
            dotContent.AppendLine("rankdir=\"TB\";");
            dotContent.AppendLine("node[shape=box];");

            string outputFilePath = $"dot\\{areaName}.svg";

            foreach (Models.Task task in tasks)
            {
                // Split the CSV string into individual items
                var itemArray = task.TaskItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var itemsList = string.Join("<br/>", itemArray.Select(item => System.Security.SecurityElement.Escape(item.Trim())));
                var itemsAttribute = string.Join(", ", itemArray.Select(item =>
                {
                    var parts = item.Split('x');
                    var itemName = parts[0].Trim();
                    var itemQty = parts.Length > 1 ? parts[1].Trim() : "1";
                    return $"item: {itemName}, qty: {itemQty}";
                }));

                var taskNameEscaped = System.Security.SecurityElement.Escape(task.TaskTitle);

                // Create the label string
                var labelString = $"{task.TaskID}[label=<" +
                $"{task.TaskID}<br/><i><b><font color=\"blue\">{taskNameEscaped}</font></b></i><br/>" +
                $"{itemsList}>];";

                // Debug the label string
                //Debug.WriteLine(labelString);

                // Append the label string to dotContent
                dotContent.AppendLine(labelString);
            }

            // Append relationships
            foreach (string relationship in relationships)
            {
                dotContent.AppendLine(relationship);
            }

            dotContent.AppendLine("}");

            // Write the .dot content to a file
            string dotFilePath = Path.ChangeExtension(outputFilePath, ".dot");
            File.WriteAllText(dotFilePath, dotContent.ToString());

            // Step 2: Call the dot engine to create the SVG
            string svgFilePath = outputFilePath;
            var dotEngine = new DotEngine();
            dotEngine.Run(dotFilePath, svgFilePath);

            // Step 3: Manipulate the SVG to insert attributes inside the <g> tag
            string svgContent = File.ReadAllText(svgFilePath);
            string manipulatedSvgContent = InsertAttributesInSvg(svgContent, tasks, relationships, areaName);
            File.WriteAllText(svgFilePath, manipulatedSvgContent);
        }
        private string InsertAttributesInSvg(string svgContent, List<Models.Task> tasks, List<string> relationships, string areaName)
        {
            var svgDoc = new XmlDocument();
            svgDoc.LoadXml(svgContent);

            foreach (var task in tasks)
            {
                // Adjust the node ID to match the SVG format
                var nodeId = $"node{task.TaskID + 1}";
                var node = svgDoc.SelectSingleNode($"//*[@id='{nodeId}']");

                if (node != null)
                {
                    var element = node as XmlElement;
                    if (element != null)
                    {
                        element.SetAttribute("ID", task.TaskID.ToString());
                        element.SetAttribute("TaskID", task.TaskName);
                        element.SetAttribute("Status", "off");

                        // Split the CSV string into individual items
                        var itemArray = task.TaskItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        var itemsAttribute = string.Join(", ", itemArray.Select(item =>
                        {
                            var parts = item.Split('x');
                            var itemName = parts[0].Trim();
                            var itemQty = parts.Length > 1 ? parts[1].Trim() : "1";
                            return $"item: {itemName}, qty: {itemQty}";
                        }));

                        element.SetAttribute("Items", itemsAttribute);
                    }
                }
            }

            // Add the relationships as a data attribute
            var svgElement = svgDoc.DocumentElement;
            if (svgElement != null)
            {
                svgElement.SetAttribute("data-relationships", JsonConvert.SerializeObject(relationships));
            }

            //// Add the combo box and script reference
            //var foreignObject = svgDoc.CreateElement("foreignObject", svgDoc.DocumentElement.NamespaceURI);
            //foreignObject.SetAttribute("x", "10");
            //foreignObject.SetAttribute("y", "10");
            //foreignObject.SetAttribute("width", "200");
            //foreignObject.SetAttribute("height", "50");

            //var body = svgDoc.CreateElement("body", "http://www.w3.org/1999/xhtml");

            //// Create a div to hold the select and button elements side by side
            //var div = svgDoc.CreateElement("div", "http://www.w3.org/1999/xhtml");
            //div.SetAttribute("style", "display: flex; align-items: center;");

            //var select = svgDoc.CreateElement("select", "http://www.w3.org/1999/xhtml");
            //select.SetAttribute("id", "depthSelect");
            //select.SetAttribute("style", "margin-right: 10px;"); // Add some space between the select and button

            //var options = new[] { "1", "3", "5", "10" };
            //foreach (var optionValue in options)
            //{
            //    var option = svgDoc.CreateElement("option", "http://www.w3.org/1999/xhtml");
            //    option.SetAttribute("value", optionValue);
            //    option.InnerText = optionValue;
            //    select.AppendChild(option);
            //}

            //var button = svgDoc.CreateElement("button", "http://www.w3.org/1999/xhtml");
            //button.SetAttribute("onclick", "performDepthSearch()");
            //button.InnerText = "Search";

            //// Append the select and button to the div
            //div.AppendChild(select);
            //div.AppendChild(button);

            //// Append the div to the body
            //body.AppendChild(div);

            //// Append the body to the foreignObject
            //foreignObject.AppendChild(body);

            //// Append the foreignObject to the SVG document
            //svgDoc.DocumentElement.AppendChild(foreignObject);

            // Create the style element with the correct namespace
            XmlElement style = svgDoc.CreateElement("style", "http://www.w3.org/1999/xhtml");
            style.SetAttribute("type", "text/css");

            // Add the CSS content as a text node
            string cssContent = @"
            .status-on polygon {
            fill: lightgreen;
            }
            ";
            XmlText styleText = svgDoc.CreateTextNode(cssContent);
            style.AppendChild(styleText);

            // Append the style element to the SVG document
            svgDoc.DocumentElement.AppendChild(style);

            // Read the content of the JavaScript file
            string scriptFilePath = "functionality.js";
            string scriptContent = File.ReadAllText(scriptFilePath);

            // Replace the string "data-area-name" with the value of areaName
            scriptContent = scriptContent.Replace("data-area-name", areaName);

            // Create the script element with the correct namespace
            XmlElement script = svgDoc.CreateElement("script", "http://www.w3.org/1999/xhtml");
            script.SetAttribute("type", "text/javascript");

            // Add the JavaScript content as a text node
            XmlText scriptText = svgDoc.CreateTextNode(scriptContent);
            script.AppendChild(scriptText);

            // Append the script element to the SVG document
            svgDoc.DocumentElement.AppendChild(script);

            // Format the SVG content for better readability
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineOnAttributes = false // Ensure attributes are on a single line
            };

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter, settings))
            {
                svgDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }

}

