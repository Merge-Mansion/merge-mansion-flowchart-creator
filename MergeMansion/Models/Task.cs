using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeMansion.Models
{
    public class Task
    {
        public string TaskID { get; set; }
        public string TaskName { get; set; }
        public string TaskTitle { get; set; }
        public string TaskItems { get; set; }

        public override string ToString()
        {
            return $"Task ID: {TaskID}\nTask Name: {TaskName}\nTask Title: {TaskTitle}\nTask Items: {TaskItems}\n";
        }
    }
}
