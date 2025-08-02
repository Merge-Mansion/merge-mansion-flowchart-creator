using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeMansion
{
    public class DotEngine
    {
        private readonly string dotExecutablePath;

        public DotEngine(string dotExecutablePath = @"C:\Program Files\Graphviz\bin\dot.exe")
        {
            this.dotExecutablePath = dotExecutablePath;
        }

        public void Run(string dotFilePath, string outputFilePath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = dotExecutablePath,
                Arguments = $"-Tsvg \"{dotFilePath}\" -o \"{outputFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
