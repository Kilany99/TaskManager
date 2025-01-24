using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Enums;

namespace TaskManager.Services
{
    public class TaskStatisticsService : INotifyPropertyChanged
    {
        private int _totalTasks;
        private int _completedTasks;
        private int _overdueTasks;
        private Dictionary<Category, int> _tasksByCategory = new();
        private Dictionary<Priority, int> _tasksByPriority = new();

        public int TotalTasks
        {
            get => _totalTasks;
            private set { _totalTasks = value; OnPropertyChanged(); }
        }

        public int CompletedTasks
        {
            get => _completedTasks;
            private set { _completedTasks = value; OnPropertyChanged(); }
        }

        public int OverdueTasks
        {
            get => _overdueTasks;
            private set { _overdueTasks = value; OnPropertyChanged(); }
        }

        public Dictionary<Category, int> TasksByCategory
        {
            get => _tasksByCategory;
            private set { _tasksByCategory = value; OnPropertyChanged(); }
        }

        public Dictionary<Priority, int> TasksByPriority
        {
            get => _tasksByPriority;
            private set { _tasksByPriority = value; OnPropertyChanged(); }
        }

        public void UpdateStatistics(IEnumerable<TaskItem> tasks)
        {
            TotalTasks = tasks.Count();
            CompletedTasks = tasks.Count(t => t.Status == Status.Completed);
            OverdueTasks = tasks.Count(t => t.DueDate < DateTime.Now && t.Status != Status.Completed);

            TasksByCategory = tasks
                .GroupBy(t => t.Category)
                .ToDictionary(g => g.Key, g => g.Count());

            TasksByPriority = tasks
                .GroupBy(t => t.Priority)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
