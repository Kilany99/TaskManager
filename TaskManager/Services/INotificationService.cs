using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TaskManager.Views;

namespace TaskManager.Services
{
    public interface INotificationService
    {
        void ShowToastNotification(TaskItem task);
        void StartMonitoring();

    }

    public class NotificationService : INotificationService
    {
        private readonly DispatcherTimer _timer;
        private readonly List<TaskItem> _monitoredTasks = new();

        public NotificationService(ITaskManagerService taskManager)
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _timer.Tick += CheckDueTasks;

            taskManager.Tasks.CollectionChanged += (_, e) =>
            {
                if (e.NewItems != null) _monitoredTasks.AddRange(e.NewItems.Cast<TaskItem>());
            };
        }

        private void CheckDueTasks(object sender, EventArgs e)
        {
            foreach (var task in _monitoredTasks.Where(t =>
                t.DueDate - DateTime.Now <= TimeSpan.FromMinutes(15) &&
                !t.IsCompleted))
            {
                ShowToastNotification(task);
            }
        }
        public void StartMonitoring() => _timer.Start();


        public void ShowToastNotification(TaskItem task)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var toast = new NotificationWindow(task.Title, task.Description)
                {
                    Owner = Application.Current.MainWindow
                };
                toast.Show();
            });
        }

    }
}
