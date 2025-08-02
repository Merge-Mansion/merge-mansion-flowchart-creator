using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace MergeMansion
{
    public class TextSearcher
    {
        public static List<string> FindOccurrences(string text, string searchPattern)
        {
            List<string> results = new List<string>();
            int index = text.IndexOf(searchPattern);

            while (index != -1)
            {
                int start = Math.Max(0, index - 20);
                int end = Math.Min(text.Length, index + searchPattern.Length + 80);
                string context = text.Substring(start, end - start);
                results.Add($"Position: {index}, Context: \"{context}\"");

                index = text.IndexOf(searchPattern, index + searchPattern.Length);
            }

            return results;
        }
    }

    public class Correction
    {
        public string LabelPattern { get; set; }
        public string ErrorPattern { get; set; }
        public string CorrectionText { get; set; }

        public Correction(string labelPattern, string errorPattern, string correctionText)
        {
            LabelPattern = labelPattern;
            ErrorPattern = errorPattern;
            CorrectionText = correctionText;
        }
    }

    public class TextCorrector
    {
        private List<Correction> corrections = new List<Correction>();

        public TextCorrector()
        {
            string labelPattern = @"162[label=\""MusicianRoomCharacterTask11";
            Debug.WriteLine("Original Text Length: " + labelPattern);

            string errorPattern = "you!' Polishing Wax";
            string correctionText = "you!'.Polishing Wax";
                        
            Correction correction = new Correction(labelPattern, errorPattern, correctionText);
            corrections.Add(correction);            
            corrections.Add(new Correction(@"24[label=\""LayeredDecoration", "\r\n", "\r\nNone x1"));
            corrections.Add(new Correction(@"4[label =\""PoolHouseEntryRemoveBrokenTv", "TVLeather", "TV.Leather"));
            corrections.Add(new Correction(@"16[label=\""PoolHouseBarPlaceTvandGlasses", "glasses\r\nCoins", "glasses.Coins"));
            corrections.Add(new Correction(@"37[label=\""PoolHouseUpperLevelPlaceTelephoneAndItemsOnTable", "tems\r\nCoins", "tems.Coins"));
            corrections.Add(new Correction(@"26[label=\""BBQAreaLoungeRemoveWoodenFenceWeeds", "weeds Knife", "weeds.Knife"));
            corrections.Add(new Correction(@"29[label=\""BBQAreaLoungePlaceNewSofa", "sofa\r\nCoins", "sofa.Coins"));
            corrections.Add(new Correction(@"74[label=\""BBQAreaDiningFixSmallWall", "wall Brush", "wall.Brush"));
            
            corrections.Add(new Correction(@" ", "TVLeather", "Leather"));

            corrections.Add(new Correction(@" ", "TVLeather", "Leather"));

            corrections.Add(new Correction(@" ", "TVLeather", "Leather"));

            corrections.Add(new Correction(@" ", "TVLeather", "Leather"));

            corrections.Add(new Correction(@" ", "TVLeather", "Leather"));

            corrections.Add(new Correction(@" ", "TVLeather", "Leather"));

            corrections.Add(new Correction(@" ", "TVLeather", "Leather"));




        }

        public string CorrectText(string text)
        {
            Debug.WriteLine("Original Text Length: " + text.Length);

            foreach (var correction in corrections)
            {
                string labelPattern = correction.LabelPattern;
                string errorPattern = correction.ErrorPattern;
                string replacement = correction.CorrectionText;

                // Example usage                
                TextSearcher textCorrector = new TextSearcher();
                List<string> occurrences = TextSearcher.FindOccurrences(text, "MusicianRoomCharacterTask11");

                foreach (var occurrence in occurrences)
                {
                    Debug.WriteLine(occurrence);
                }

                // Find the label containing the pattern and apply the correction
                Debug.WriteLine("labelPattern Text: " + labelPattern);

                int labelIndex = text.IndexOf(labelPattern);
                if (labelIndex != -1)
                {
                    int errorIndex = text.IndexOf(errorPattern, labelIndex);
                    if (errorIndex != -1)
                    {
                        text = text.Substring(0, errorIndex) + replacement + text.Substring(errorIndex + errorPattern.Length);
                    }
                }
            }

            Debug.WriteLine("Corrected Text Length: " + text.Length);
            return text;
        }
    }
}

