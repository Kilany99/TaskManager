// ViewModels/MainViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Repositories;
using TaskManager.Services;
using TaskManager.Utilities;

namespace TaskManager.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ITaskManagerService _taskManager;
        private readonly IReminderService _reminderService;
        private readonly IStatisticsService _statisticsService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private TaskItem _newTask = new();
        private string _searchText = string.Empty;
        private Category _selectedCategory;
        private Tag _selectedFilterTag;
        private ICollectionView _filteredTasks;
        private ObservableCollection<Tag> _selectedTags = new();


        public MainViewModel(
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            ITaskManagerService taskManager,
            IReminderService reminderService,
            IStatisticsService statisticsService)
        {
            _taskManager = taskManager;
            _reminderService = reminderService;
            _statisticsService = statisticsService;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;

            // Initialize commands
            AddCommand = new RelayCommand(AddTask, CanAddTask);
            DeleteCommand = new RelayCommand(_ => DeleteTask(), _ => CanDeleteTask());
            SaveCommand = new RelayCommand(_ => SaveTask(), _ => CanSaveTask());

            // Initialize data
            Categories = new ObservableCollection<Category>(_categoryRepository.GetAll());
            AvailableTags = new ObservableCollection<Tag>(_tagRepository.GetAll());

            // Setup filtered view
            _filteredTasks = CollectionViewSource.GetDefaultView(_taskManager.Tasks);
            _filteredTasks.Filter = TaskFilter;

            // Event subscriptions
            _reminderService.ReminderTriggered += ShowReminder;
            _reminderService.StartMonitoring();
            _taskManager.Tasks.CollectionChanged += (_, _) => UpdateFilter();
        }

        public ICommand AddCommand;
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }


        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<Tag> AvailableTags { get; }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetField(ref _selectedCategory, value);
                UpdateFilter();
            }
        }
        public ObservableCollection<Tag> SelectedTags
        {
            get => _selectedTags;
            set => SetField(ref _selectedTags, value);
        }

        public TaskItem NewTask
        {
            get => _newTask;
            set => SetField(ref _newTask, value);
        }

        public Tag SelectedFilterTag
        {
            get => _selectedFilterTag;
            set
            {
                SetField(ref _selectedFilterTag, value);
                UpdateFilter();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetField(ref _searchText, value);
                UpdateFilter();
            }
        }

        public ICollectionView FilteredTasks => _filteredTasks;
        public ObservableCollection<TaskItem> Tasks => _taskManager.Tasks;

        public TaskItem SelectedTask
        {
            get => _taskManager.SelectedTask;
            set => _taskManager.SelectedTask = value;
        }

        public event Action<string> OnErrorOccurred;

        private bool TaskFilter(object item)
        {
            if (item is not TaskItem task) return false;

            var textMatch = string.IsNullOrWhiteSpace(SearchText) ||
                          task.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                          task.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

            var categoryMatch = SelectedCategory == null || task.Category == SelectedCategory;
            var tagMatch = SelectedFilterTag == null || task.Tags.Contains(SelectedFilterTag);

            return textMatch && categoryMatch && tagMatch;
        }

        private void UpdateFilter()
        {
            _filteredTasks.Refresh();
            UpdateStatistics();
        }

        private void UpdateStatistics() => _statisticsService.UpdateStatistics(Tasks);

        private void ShowReminder(TaskItem task)
        {
            OnReminderTriggered?.Invoke(task.Title);
        }
        private bool CanAddTask(object parameter)
        {
            // Basic validation check
            return !NewTask.HasErrors &&
                   !string.IsNullOrWhiteSpace(NewTask.Title) &&
                   NewTask.DueDate >= DateTime.Today;
        }

        private void AddTask(object parameter)
        {
            if (NewTask.HasErrors) return;

            var newTask = new TaskItem
            {
                Title = NewTask.Title,
                Description = NewTask.Description,
                DueDate = NewTask.DueDate,
                Priority = NewTask.Priority,
                Category = NewTask.Category,
                Tags = new ObservableCollection<Tag>(SelectedTags.Where(t => t.IsSelected))

                
            };

            _taskManager.AddTask(newTask);

            // Reset form and tags selection
            NewTask = new TaskItem { DueDate = DateTime.Today.AddDays(1) };
            foreach (var tag in AvailableTags)
            {
                tag.IsSelected = false;
            }
        }

        private void DeleteTask()
        {
            _taskManager.DeleteTask();
            UpdateStatistics();
        }

        private void SaveTask()
        {
            _taskManager.SaveTask();
            UpdateStatistics();
        }

        private bool CanDeleteTask() => _taskManager.SelectedTask != null;
        private bool CanSaveTask() => _taskManager.SelectedTask != null &&
                                    !_taskManager.SelectedTask.HasErrors;

        public event Action<string>? OnReminderTriggered;
    }
}