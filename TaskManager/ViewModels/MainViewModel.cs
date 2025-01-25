// ViewModels/MainViewModel.cs
using System;
using System.Collections.ObjectModel;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.ViewModels;

namespace TaskManager.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITaskManagerService _taskManager;
        private readonly IReminderService _reminderService;
        private readonly IStatisticsService _statisticsService;
        private string _searchText = string.Empty;

        public MainViewModel(
            ITaskManagerService taskManager,
            IReminderService reminderService,
            IStatisticsService statisticsService)
        {
            _taskManager = taskManager;
            _reminderService = reminderService;
            _statisticsService = statisticsService;

            _reminderService.ReminderTriggered += ShowReminder;
            _reminderService.StartMonitoring();
            _taskManager.Tasks.CollectionChanged += (_, _) => UpdateStatistics();
        }

        public ObservableCollection<TaskItem> Tasks => _taskManager.Tasks;

        public TaskItem SelectedTask
        {
            get => _taskManager.SelectedTask;
            set => _taskManager.SelectedTask = value;
        }

        public string SearchText
        {
            get => _searchText;
            set => SetField(ref _searchText, value);
        }

        private void UpdateStatistics() => _statisticsService.UpdateStatistics(Tasks);

        private void ShowReminder(TaskItem task)
        {
            OnReminderTriggered?.Invoke(task.Title);
        }

        public event Action<string>? OnReminderTriggered;

        // Command implementations would go here
    }
}