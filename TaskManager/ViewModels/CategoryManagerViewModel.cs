using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskManager.Services;

namespace TaskManager.ViewModels
{
    public class CategoryManagerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Category> _categories = new();
        private Category _selectedCategory = new();
        private readonly IDataService _dataService;

        public CategoryManagerViewModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadCategories();
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        public ICommand AddCategoryCommand => new RelayCommand(AddCategory);
        public ICommand DeleteCategoryCommand => new RelayCommand(DeleteCategory, CanDeleteCategory);
        public ICommand EditCategoryCommand => new RelayCommand(EditCategory, CanEditCategory);

        private async void LoadCategories()
        {
            try
            {
                var categories = await _dataService.GetCategoriesAsync();
                Categories = new ObservableCollection<Category>(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }
        }

        private async void AddCategory(object? parameter)
        {
            try
            {
                var newCategory = new Category { Name = "New Category" };
                await _dataService.AddCategoryAsync(newCategory);
                Categories.Add(newCategory);
                SelectedCategory = newCategory;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding category: {ex.Message}");
            }
        }

        private async void DeleteCategory(object? parameter)
        {
            if (SelectedCategory == null) return;

            try
            {
                await _dataService.DeleteCategoryAsync(SelectedCategory);
                Categories.Remove(SelectedCategory);
                SelectedCategory = new Category();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
            }
        }

        private async void EditCategory(object? parameter)
        {
            if (SelectedCategory == null) return;

            try
            {
                await _dataService.UpdateCategoryAsync(SelectedCategory);
                var updatedCategories = await _dataService.GetCategoriesAsync();
                Categories = new ObservableCollection<Category>(updatedCategories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing category: {ex.Message}");
            }
        }


        private bool CanDeleteCategory(object? parameter)
            => SelectedCategory != null;

        private bool CanEditCategory(object? parameter)
            => SelectedCategory != null;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
