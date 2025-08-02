using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MergeMansion.Models;
using Root = MergeMansion.MergeType.Root;

namespace MergeMansion.Services
{
    public class TaskParser
    {
        public List<Task> ParseTasks(string text, List<HotspotRef> taskHotspotsRefs)
        {
            List<Task> tasks = new List<Task>();

            // Split the text by semicolons ;
            string pattern = @"(?<=\])(;)";

            // Split the text by semicolons with preceding ']'
            string[] parts = Regex.Split(text, pattern);

            foreach (string part in parts)
            {
                if (part.Contains("[label="))
                {
                    string cleanedText = RemovePunctuation(part);
                    // Extract task ID
                    int idEndIndex = cleanedText.IndexOf('[');
                    string taskId = cleanedText.Substring(0, idEndIndex).Trim();

                    // Extract task name and detail
                    int labelStartIndex = part.IndexOf("label=") + "label=".Length;
                    int labelEndIndex = part.IndexOf(']', labelStartIndex);
                    string taskFullName = part.Substring(labelStartIndex, labelEndIndex - labelStartIndex).Trim();

                    // Split task name and detail
                    string[] taskNameParts = taskFullName.Split(new[] { ' ' }, 2, StringSplitOptions.None);
                    string taskName = taskNameParts[0];
                    HotspotRef hotspot = GetHotspotById(taskHotspotsRefs, taskName);

                    string taskDetail = taskNameParts.Length > 1 ? taskNameParts[1] : "";

                    // Split taskDetail based on the defined pattern
                    int splitPosition = GetSplitPosition(taskDetail);
                    string taskTitle = taskDetail.Substring(0, splitPosition).Trim();
                    string taskItems = taskDetail.Substring(splitPosition).Trim();

                    taskTitle = hotspot.Description;

                    // If taskTitle is empty, use #TaskName
                    if (string.IsNullOrWhiteSpace(taskTitle))
                    {
                        taskTitle = $"#{taskName}";
                    }

                    // Manually parse and separate task items
                    taskItems = ManuallyParseItems(taskItems);
                    //taskItems = GetIems(hotspot.RequirementsList);
                    // If there isn't any item text and taskTitle doesn't start with '#', use #######
                    if (string.IsNullOrWhiteSpace(taskItems) && !taskTitle.StartsWith("#"))
                    {
                        taskItems = "#######";
                    }

                    // Create a new task object and add it to the list
                    Task task = new Task
                    {
                        TaskID = taskId,
                        TaskName = taskName,
                        TaskTitle = taskTitle,
                        TaskItems = taskItems
                    };

                    tasks.Add(task);
                }
            }

            return tasks;
        }

        public List<Task> ParseTasks2(string text, List<HotspotRef> taskHotspotsRefs, Root root)
        {
            List<Task> tasks = new List<Task>();

            // Split the text by semicolons ;
            string pattern = @"(?<=\])(;)";

            // Split the text by semicolons with preceding ']'
            string[] parts = Regex.Split(text, pattern);

            foreach (string part in parts)
            {
                if (part.Contains("[label="))
                {
                    string cleanedText = RemovePunctuation(part);
                    // Extract task ID
                    int idEndIndex = cleanedText.IndexOf('[');
                    string taskId = cleanedText.Substring(0, idEndIndex).Trim();

                    // Extract task name and detail
                    int labelStartIndex = part.IndexOf("label=") + "label=".Length;
                    int labelEndIndex = part.IndexOf(']', labelStartIndex);
                    string taskFullName = part.Substring(labelStartIndex, labelEndIndex - labelStartIndex).Trim();

                    // Split task name and detail
                    string[] taskNameParts = taskFullName.Split(new[] { ' ' }, 2, StringSplitOptions.None);
                    string taskName = taskNameParts[0];
                    HotspotRef hotspot = GetHotspotById(taskHotspotsRefs, taskName);

                    string taskDetail = taskNameParts.Length > 1 ? taskNameParts[1] : "";

                    // Split taskDetail based on the defined pattern
                    int splitPosition = GetSplitPosition(taskDetail);
                    string taskTitle = taskDetail.Substring(0, splitPosition).Trim();
                    string taskItems = taskDetail.Substring(splitPosition).Trim();

                    //taskTitle = hotspot.Description;

                    // If taskTitle is empty, use #TaskName
                    if (string.IsNullOrWhiteSpace(taskTitle))
                    {
                        taskTitle = $"#{taskName}";
                    }

                    // Manually parse and separate task items
                    taskItems = ManuallyParseItems(taskItems);
                    //taskItems = GetIems(hotspot.RequirementsList);
                    // If there isn't any item text and taskTitle doesn't start with '#', use #######
                    if (string.IsNullOrWhiteSpace(taskItems) && !taskTitle.StartsWith("#"))
                    {
                        taskItems = "#######";
                    }

                    // Create a new task object and add it to the list
                    Task task = new Task
                    {
                        TaskID = taskId,
                        TaskName = taskName,
                        TaskTitle = taskTitle,
                        TaskItems = taskItems
                    };

                    tasks.Add(task);
                }
            }

            return tasks;
        }

        private int GetSplitPosition(string taskDetail)
        {
            // Loop through each character in the string
            for (int i = 0; i < taskDetail.Length; i++)
            {
                // Check if the current character is an uppercase letter
                // and the previous character is a lowercase letter, punctuation mark, or symbol
                if (char.IsUpper(taskDetail[i]) && i > 1 &&
                    (
                    char.IsLower(taskDetail[i - 1]) || char.IsPunctuation(taskDetail[i - 1]) || char.IsSymbol(taskDetail[i - 1])
                    )
                    )
                {
                    return i; // Return the current position as the split position
                }
            }

            // Second loop: Check for spaceUpper
            for (int i = 0; i < taskDetail.Length; i++)
            {
                if (char.IsUpper(taskDetail[i]) && i > 0 && char.IsWhiteSpace(taskDetail[i - 1]))
                {
                    // Check ahead for 'xN' pattern
                    for (int j = i + 1; j < taskDetail.Length - 1; j++)
                    {
                        if (taskDetail[j] == 'x' && char.IsDigit(taskDetail[j + 1]))
                        {
                            return i; // Return the current position as the split position
                        }
                    }
                }
            }

            // Second loop: Check for spaceUpper
            for (int i = 0; i < taskDetail.Length; i++)
            {
                if (char.IsUpper(taskDetail[i]) && i > 0 && char.IsDigit(taskDetail[i - 1]))
                {

                    return i; // Return the current position as the split position

                }
            }

            // If no conditions are met, return the length of the string
            return taskDetail.Length;
        }

        private string ManuallyParseItems(string taskItems)
        {
            List<string> items = new List<string>();
            string pattern = @" x\d+";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(taskItems);

            int lastIndex = 0;

            foreach (Match match in matches)
            {
                int splitPosition = match.Index + match.Length;
                if (splitPosition > lastIndex)
                {
                    string item = taskItems.Substring(lastIndex, splitPosition - lastIndex).Trim();
                    items.Add(item);
                    lastIndex = splitPosition;
                }
            }

            if (lastIndex < taskItems.Length)
            {
                items.Add(taskItems.Substring(lastIndex).Trim());
            }

            return string.Join(", ", items);
        }

        public List<string> ParseRelationship(string text)
        {
            List<string> relationships = new List<string>();

            string relationshipPattern = @"(\d+)->(\d+);";
            var relationshipMatches = Regex.Matches(text, relationshipPattern);

            foreach (Match match in relationshipMatches)
            {
                string from = match.Groups[1].Value;
                string to = match.Groups[2].Value;
                relationships.Add($"{from}->{to}");
            }

            return relationships;
        }

        private string RemovePunctuation(string input)
        {
            // Replace dot, comma, and apostrophe with an empty string
            return input.Replace(".", "")
                        .Replace("!", "")
                        .Replace("?", "")
                        .Replace(",", "")
                        .Replace("'", "");
        }

        public static HotspotRef GetHotspotById(List<HotspotRef> hotspots, string id)
        {
            // Use LINQ to find the HotspotRef with the given Id
            return hotspots.FirstOrDefault(h => h.Id == id);
        }
    }
}
