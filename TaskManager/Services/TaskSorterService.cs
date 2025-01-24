using System;
using System.Collections.Generic;
using TaskManager.Models.Enums;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManager.Services
{
    public class TaskSorterService
    {
        

        private int _totalTasks;
        private int _completedTasks;
        private int _overdueTasks;
        private Dictionary<Category, int> _tasksByCategory = new();
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

        public void UpdateStatistics(IEnumerable<TaskItem> tasks)
        {
            TotalTasks = tasks.Count();
            CompletedTasks = tasks.Count(t => t.IsCompleted);
            OverdueTasks = tasks.Count(t => t.DueDate < DateTime.Now && !t.IsCompleted);
            TasksByCategory = tasks
                .GroupBy(t => t.Category)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        
        public IEnumerable<TaskItem> SortTasks(IEnumerable<TaskItem> tasks, SortOption sortOption, bool ascending = true)
        {
            var sortedTasks = sortOption switch
            {
                SortOption.DueDate => ascending
                    ? tasks.OrderBy(t => t.DueDate)
                    : tasks.OrderByDescending(t => t.DueDate),

                SortOption.Priority => ascending
                    ? tasks.OrderBy(t => t.Priority)
                    : tasks.OrderByDescending(t => t.Priority),

                SortOption.Title => ascending
                    ? tasks.OrderBy(t => t.Title)
                    : tasks.OrderByDescending(t => t.Title),

                SortOption.Status => ascending
                    ? tasks.OrderBy(t => t.Status)
                    : tasks.OrderByDescending(t => t.Status),

                SortOption.Category => ascending
                    ? tasks.OrderBy(t => t.Category?.Name)
                    : tasks.OrderByDescending(t => t.Category?.Name),

                _ => tasks
            };

            return sortedTasks;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
