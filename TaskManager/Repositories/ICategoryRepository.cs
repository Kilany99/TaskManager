using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskManager.Repositories
{
    public interface ICategoryRepository
    {
        ObservableCollection<Category> GetAll();
        void SaveAll(ObservableCollection<Category> categories);
    }

    public class JsonCategoryRepository : ICategoryRepository
    {
        private const string FilePath = "categories.json";

        public ObservableCollection<Category> GetAll()
        {
            if (!File.Exists(FilePath)) return CreateDefaultCategories();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<ObservableCollection<Category>>(json)
                ?? CreateDefaultCategories();
        }

        public void SaveAll(ObservableCollection<Category> categories)
        {
            var json = JsonSerializer.Serialize(categories);
            File.WriteAllText(FilePath, json);
        }

        private ObservableCollection<Category> CreateDefaultCategories() => new()
        {
            new Category { Id = 1, Name = "Work" },
            new Category { Id = 2, Name = "Personal" },
            new Category { Id = 3, Name = "Shopping" },
            new Category { Id = 4, Name = "Health" }
        };
    }

}
