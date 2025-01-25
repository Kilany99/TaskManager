// Services/Implementations/ReminderService.cs
using System;
using System.Collections.ObjectModel;
using TaskManager.Models;

namespace TaskManager.Services
{
    public interface IReminderService
    {
        event Action<TaskItem>? ReminderTriggered;
        void StartMonitoring();

    }
    public class ReminderService : IReminderService
    {
        private readonly Func<ObservableCollection<TaskItem>> _getTasks;
        private readonly System.Windows.Threading.DispatcherTimer _timer;

        public ReminderService(Func<ObservableCollection<TaskItem>> getTasks)
        {
            _getTasks = getTasks;
            _timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _timer.Tick += CheckReminders;
        }

        public event Action<TaskItem> ReminderTriggered;

        public void StartMonitoring() => _timer.Start();

        private void CheckReminders(object sender, EventArgs e)
        {
            var tasks = _getTasks();
            foreach (var task in tasks.Where(t =>
                !t.IsCompleted &&
                t.DueDate <= DateTime.Now.AddMinutes(15) &&
                t.DueDate > DateTime.Now))
            {
                ReminderTriggered?.Invoke(task);
            }
        }
    }
}