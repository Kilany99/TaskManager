using System;
using System.Windows.Threading;

namespace TaskManager.Services
{
    public class ReminderService
    {
        private readonly DispatcherTimer _timer;
        private readonly Func<IEnumerable<TaskItem>> _getTasks;

        public event Action<TaskItem>? ReminderTriggered;

        public ReminderService(Func<IEnumerable<TaskItem>> getTasks)
        {
            _getTasks = getTasks;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _timer.Tick += CheckReminders;
            _timer.Start();
        }

        private void CheckReminders(object? sender, EventArgs e)
        {
            foreach (var task in _getTasks().Where(t =>
                !t.IsCompleted &&
                t.DueDate <= DateTime.Now.AddMinutes(15) &&
                t.DueDate > DateTime.Now))
            {
                ReminderTriggered?.Invoke(task);
            }
        }
    }
}
