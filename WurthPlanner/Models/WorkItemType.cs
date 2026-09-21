using System;
using System.Collections.Generic;
using System.Text;

namespace WurthPlanner.Models
{
    public enum WorkItemType
    {
        Unknown = 0,
        Task = 1,
        Phase = 2,
        Event = 3,
        Assignment = 4,
        Note = 5,
    }

    public static class WorkItemTypeExtensions
    {
        public static string ToString(this WorkItemType workItemType)
        {
            return workItemType switch
            {
                WorkItemType.Task => "Task",
                WorkItemType.Phase => "Phase",
                WorkItemType.Event => "Event",
                WorkItemType.Assignment => "Assignment",
                WorkItemType.Note => "Note",
                _ => "Unknown"
            };
        }

        public static WorkItem ValidateWorkItem(this WorkItemType workItemType, WorkItem workItem)
        {
            return workItemType switch
            {
                WorkItemType.Task => ValidateTask(workItem),
                WorkItemType.Phase => ValidatePhase(workItem),
                WorkItemType.Event => ValidateEvent(workItem),
                WorkItemType.Assignment => ValidateAssignment(workItem),
                WorkItemType.Note => ValidateNote(workItem),
                _ => workItem
            };
        }

        private static WorkItem ValidateTask(WorkItem workItem)
        {
            return workItem;
        }

        private static WorkItem ValidatePhase(WorkItem workItem)
        {
            return workItem;
        }

        private static WorkItem ValidateEvent(WorkItem workItem)
        {
            return workItem;
        }

        private static WorkItem ValidateAssignment(WorkItem workItem)
        {
            return workItem;
        }

        private static WorkItem ValidateNote(WorkItem workItem)
        {
            return workItem;
        }
    }
}
