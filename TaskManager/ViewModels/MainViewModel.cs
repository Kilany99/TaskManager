// ViewModels/MainViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Enums;
using TaskManager.Models;
using TaskManager.Repositories;
using TaskManager.Services;
using TaskManager.Utilities;
using TaskManager.Utilities.Validators;

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
        private readonly TaskValidator _validator = new();
        private ObservableCollection<Tag> _selectedFilterTags = new();
        private Priority? _selectedPriority;
        private bool _isDarkTheme;

        public MainViewModel(
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            ITaskManagerService taskManager,
            IReminderService reminderService,
            IStatisticsService statisticsService,
            INotificationService notificationService)
        {
            _taskManager = taskManager;
            _reminderService = reminderService;
            _statisticsService = statisticsService;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;

            // Initialize commands
            AddCommand = new RelayCommand(AddTask, CanAddTask);
            DeleteCommand = new RelayCommand(DeleteTask, CanDeleteTask);
            SaveCommand = new RelayCommand(SaveTask, CanSaveTask);
            EditCommand = new RelayCommand(EditTask, CanEditTask);
            ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
            notificationService.StartMonitoring();


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

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand ToggleThemeCommand { get; }


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

        public ObservableCollection<Tag> SelectedFilterTags
        {
            get => _selectedFilterTags;
            set => SetField(ref _selectedFilterTags, value);
        }

        public Priority? SelectedPriority
        {
            get => _selectedPriority;
            set
            {
                SetField(ref _selectedPriority, value);
                UpdateFilter();
            }
        }
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetField(ref _isDarkTheme, value);
        }

        public event Action<string> OnErrorOccurred;

        private bool TaskFilter(object item)
        {
            if (item is not TaskItem task) return false;

            // Text search
            var textMatch = string.IsNullOrWhiteSpace(SearchText) ||
                          (task.Title?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                          (task.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false);

            // Category filter
            var categoryMatch = SelectedCategory == null || task.Category?.Id == SelectedCategory.Id;

            // Tag filter (match any selected tag)
            var tagMatch = !SelectedFilterTags.Any() ||
                         SelectedFilterTags.Any(selectedTag =>
                             task.Tags.Any(taskTag => taskTag.Id == selectedTag.Id));

            // Priority filter
            var priorityMatch = !SelectedPriority.HasValue ||
                              task.Priority == SelectedPriority.Value;

            return textMatch && categoryMatch && tagMatch && priorityMatch;
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
            if (NewTask.HasErrors)
                throw new Exception("Has Errors");

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

        private bool CanEditTask(object parameter)
         => SelectedTask != null && !SelectedTask.HasErrors;

        private void EditTask(object parameter)
        {
            _taskManager.UpdateTask(SelectedTask);
            UpdateStatistics();
        }

        private bool CanDeleteTask(object parameter)
            => SelectedTask != null;

        private void DeleteTask(object parameter)
        {
            _taskManager.DeleteTask(SelectedTask);
            SelectedTask = null;
            UpdateStatistics();
        }
        private void SaveTask(object parameter)
        {
            var results = _validator.Validate(SelectedTask);
            if (!results.IsValid)
            {
                var errors = string.Join("\n", results.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }
            _taskManager.SaveTask(SelectedTask);
        }

        private bool CanSaveTask(object parameter)
        {
            return SelectedTask != null &&
                 _validator.Validate(SelectedTask).IsValid;
        }
        private void ToggleTheme()
        {
            _isDarkTheme = !_isDarkTheme;
            ApplyTheme(_isDarkTheme ? "Dark" : "Light");
        }

        private void ApplyTheme(string themeName)
        {
            var themeUri = new Uri($"/Themes/{themeName}Theme.xaml", UriKind.Relative);

            // Find and remove only the theme dictionary
            var existingTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme.xaml") == true);

            if (existingTheme != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(existingTheme);
            }

            // Add new theme
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary { Source = themeUri }
            );
        }

        public event Action<string>? OnReminderTriggered;
    }
}