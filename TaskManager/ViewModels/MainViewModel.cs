using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Enums;
using TaskManager.Models;
using TaskManager.Models.Enums;
using TaskManager.Services;
using TaskManager.ViewModels;

namespace TaskManager
{
   
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string DataFilePath = "tasks.json";
        private string _searchText;
        private Tag _selectedFilterTag;
        private bool _showCompleted;
        private ObservableCollection<TaskItem> _tasks = new();
        private TaskItem _selectedTask = new();
        private readonly ReminderService _reminderService;
        private SortOption _selectedSortOption;
        private readonly TaskSorterService _taskSorter;
        private readonly TaskStatisticsService _statisticsService;
        private readonly CategoryManagerViewModel _categoryManager;
        private Category _selectedCategory;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Tag> _availableTags;
        

        public MainViewModel()
        {
            // Initialize services
            _taskSorter = new TaskSorterService();
            _statisticsService = new TaskStatisticsService();
            _categoryManager = new CategoryManagerViewModel(new DataService()); 
            _reminderService = new ReminderService(() => Tasks);

            // Initialize collections
            Tasks = new ObservableCollection<TaskItem>();
            FilteredTasks = CollectionViewSource.GetDefaultView(Tasks);
            FilteredTasks.Filter = o => TaskFilter(o as TaskItem);
            _categories = new ObservableCollection<Category>();
            _availableTags = new ObservableCollection<Tag>();

            // Setup event handlers
            _reminderService.ReminderTriggered += ShowReminder;

            // Load initial data
            LoadTasks();
            UpdateStatistics();
            LoadCategories();
            LoadTags();
        }
        public CategoryManagerViewModel CategoryManager => _categoryManager;
        public TaskStatisticsService Statistics => _statisticsService;
        public TaskSorterService TaskSorter { get; } = new();
        public ObservableCollection<SortOption> SortOptions { get; }
               = new(Enum.GetValues<SortOption>());
        public IEnumerable<Priority> PriorityValues => Enum.GetValues<Priority>();

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; UpdateFilter(); OnPropertyChanged(); }
        }
        public bool ShowCompleted
        {
            get => _showCompleted;
            set { _showCompleted = value; UpdateFilter(); OnPropertyChanged(); }
        }
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                UpdateFilter();
                OnPropertyChanged();
            }
        }
        public ICollectionView FilteredTasks { get; }
        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }
        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }
        public ICommand AddCommand => new RelayCommand(AddTask);
        public ICommand DeleteCommand => new RelayCommand(DeleteTask);
        public ICommand SaveCommand => new RelayCommand(SaveTask);
        public Tag SelectedFilterTag
        {
            get => _selectedFilterTag;
            set { _selectedFilterTag = value; UpdateFilter(); OnPropertyChanged(); }
        }
        public SortOption SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                UpdateSorting();
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Tag> AvailableTags
        {
            get => _availableTags;
            set { _availableTags = value; OnPropertyChanged(); }
        }

        private void LoadCategories()
        {
            // Add some default categories
            Categories.Add(new Category { Id = 1, Name = "Work" });
            Categories.Add(new Category { Id = 2, Name = "Personal" });
            Categories.Add(new Category { Id = 3, Name = "Shopping" });
            Categories.Add(new Category { Id = 4, Name = "Health" });
        }

        private void LoadTags()
        {
            // Add some default tags
            AvailableTags.Add(new Tag { Name = "Important", Color = "#FF0000" });
            AvailableTags.Add(new Tag { Name = "Urgent", Color = "#FF6B00" });
            AvailableTags.Add(new Tag { Name = "Later", Color = "#00FF00" });
            AvailableTags.Add(new Tag { Name = "Ideas", Color = "#0000FF" });
        }

        private void UpdateSorting()
        {
            var sortedTasks = _taskSorter.SortTasks(Tasks, SelectedSortOption);
            Tasks = new ObservableCollection<TaskItem>(sortedTasks);
        }

        private void LoadTasks()
        {
            try
            {
                if (File.Exists(DataFilePath))
                {
                    var json = File.ReadAllText(DataFilePath);
                    var tasks = JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(json);
                    Tasks = tasks ?? new ObservableCollection<TaskItem>();
                }
                else
                {
                    Tasks = new ObservableCollection<TaskItem>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SaveTasks()
        {
            try
            {
                var json = JsonSerializer.Serialize(Tasks);
                File.WriteAllText(DataFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Update existing commands to call SaveTasks()
        private void DeleteTask(object? obj)
        {
            if (SelectedTask != null)
            {
                Tasks.Remove(SelectedTask);
                SaveTasks();
                UpdateStatistics();
            }
        }

        private void AddTask(object? obj)
        {
            Tasks.Add(new TaskItem { DueDate = DateTime.Now.AddDays(1) });
            SelectedTask = Tasks.Last();
            SaveTasks(); // Auto-save on change
            UpdateStatistics();

        }

        private void SaveTask(object? obj)
        {
            if (SelectedTask != null && !SelectedTask.HasErrors)
            {
                var taskToUpdate = Tasks.FirstOrDefault(t => t == SelectedTask);
                if (taskToUpdate != null)
                {
                    taskToUpdate.Title = SelectedTask.Title;
                    taskToUpdate.Description = SelectedTask.Description;
                    taskToUpdate.DueDate = SelectedTask.DueDate;
                    taskToUpdate.IsCompleted = SelectedTask.IsCompleted;
                    taskToUpdate.Priority = SelectedTask.Priority;
                    taskToUpdate.Category = SelectedTask.Category;
                    SaveTasks();
                    UpdateStatistics();
                    OnPropertyChanged(nameof(Tasks));
                }
            }
            else
            {
                MessageBox.Show("Please fix validation errors before saving.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
            }
        }
        private bool TaskFilter(TaskItem task)
        {
            if (task == null) return false;

            var textMatch = string.IsNullOrWhiteSpace(SearchText) ||
                           task.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                           task.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

            var categoryMatch = SelectedCategory == null || task.Category == SelectedCategory;
            var tagMatch = SelectedFilterTag == null || task.Tags.Contains(SelectedFilterTag);
            var statusMatch = ShowCompleted || !task.IsCompleted;

            return textMatch && categoryMatch && tagMatch && statusMatch;
        }
        private void UpdateFilter()=> FilteredTasks.Refresh();

        public void AddCategory(string name)
        {
            var newCategory = new Category
            {
                Id = Categories.Count + 1,
                Name = name
            };
            Categories.Add(newCategory);
            SaveCategories();
        }

        public void RemoveCategory(Category category)
        {
            if (category != null)
            {
                Categories.Remove(category);
                SaveCategories();
            }
        }
        public void AddTag(string name, string color)
        {
            var newTag = new Tag
            {
                Name = name,
                Color = color
            };
            AvailableTags.Add(newTag);
            SaveTags();
        }

        public void RemoveTag(Tag tag)
        {
            if (tag != null)
            {
                AvailableTags.Remove(tag);
                SaveTags();
            }
        }
        // Save and load methods for categories and tags
        private void SaveCategories()
        {
            try
            {
                var json = JsonSerializer.Serialize(Categories);
                File.WriteAllText("categories.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving categories: {ex.Message}");
            }
        }

        private void SaveTags()
        {
            try
            {
                var json = JsonSerializer.Serialize(AvailableTags);
                File.WriteAllText("tags.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving tags: {ex.Message}");
            }
        }

        private void LoadSavedCategories()
        {
            try
            {
                if (File.Exists("categories.json"))
                {
                    var json = File.ReadAllText("categories.json");
                    var savedCategories = JsonSerializer.Deserialize<List<Category>>(json);
                    Categories = new ObservableCollection<Category>(savedCategories);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}");
            }
        }

        private void LoadSavedTags()
        {
            try
            {
                if (File.Exists("tags.json"))
                {
                    var json = File.ReadAllText("tags.json");
                    var savedTags = JsonSerializer.Deserialize<List<Tag>>(json);
                    AvailableTags = new ObservableCollection<Tag>(savedTags);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tags: {ex.Message}");
            }
        }

        private void UpdateStatistics()
        {
            _statisticsService.UpdateStatistics(Tasks);
        }
        private void ShowReminder(TaskItem task)
        {
            string reminderMessage = $"Reminder: {task.Title} is due soon!";
            var reminderWindow = new ReminderWindow(reminderMessage);
            reminderWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
