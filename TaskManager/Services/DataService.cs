using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskManager.Services
{
    public interface IDataService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
    }

    public class DataService : IDataService
    {
        private const string CategoriesFilePath = "categories.json";

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            if (File.Exists(CategoriesFilePath))
            {
                var json = await File.ReadAllTextAsync(CategoriesFilePath);
                return JsonSerializer.Deserialize<List<Category>>(json) ?? new List<Category>();
            }
            return new List<Category>();
        }

        public async Task AddCategoryAsync(Category category)
        {
            var categories = (await GetCategoriesAsync()).ToList();
            categories.Add(category);
            await SaveCategoriesAsync(categories);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var categories = (await GetCategoriesAsync()).ToList();
            var index = categories.FindIndex(c => c.Id == category.Id);
            if (index != -1)
            {
                categories[index] = category;
                await SaveCategoriesAsync(categories);
            }
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            var categories = (await GetCategoriesAsync()).ToList();
            categories.RemoveAll(c => c.Id == category.Id);
            await SaveCategoriesAsync(categories);
        }

        private async Task SaveCategoriesAsync(IEnumerable<Category> categories)
        {
            var json = JsonSerializer.Serialize(categories);
            await File.WriteAllTextAsync(CategoriesFilePath, json);
        }
    }
}
