using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Services
{
    public interface IStatisticsService
    {
        void UpdateStatistics(ObservableCollection<TaskItem> tasks);
        int TotalTasks { get; }
        int CompletedTasks { get; }
        int OverdueTasks { get; }
    }
    public class StatisticsService : IStatisticsService
    {
        public int TotalTasks { get; private set; }
        public int CompletedTasks { get; private set; }
        public int OverdueTasks { get; private set; }

        public void UpdateStatistics(ObservableCollection<TaskItem> tasks)
        {
            TotalTasks = tasks.Count;
            CompletedTasks = tasks.Count(t => t.IsCompleted);
            OverdueTasks = tasks.Count(t => !t.IsCompleted && t.DueDate < DateTime.Now);
        }
    }
}
