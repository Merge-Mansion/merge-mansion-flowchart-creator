using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeMansion
{
    public class Task2
    {
        public int Id { get; set; }
        public string TaskID { get; set; }
        public string TaskName { get; set; }
        public string[] Items { get; set; }

        public Task2(int id, string taskID, string taskName, string[] items)
        {
            Id = id;
            TaskID = taskID;
            TaskName = taskName;
            Items = items;
        }
    }

    public class TaskWithArea
    {
        public Task Task { get; set; }
        public string AreaName { get; set; }

        public TaskWithArea(Task task, string areaName)
        {
            Task = task;
            AreaName = areaName;
        }
    }
}
