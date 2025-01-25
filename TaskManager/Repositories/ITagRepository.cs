using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Repositories
{
    public interface ITagRepository
    {
        ObservableCollection<Tag> GetAll();
        void SaveAll(ObservableCollection<Tag> tags);
    }

    public class JsonTagRepository : ITagRepository
    {
        private const string FilePath = "tags.json";

        public ObservableCollection<Tag> GetAll()
        {
            if (!File.Exists(FilePath)) return CreateDefaultTags();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<ObservableCollection<Tag>>(json)
                ?? CreateDefaultTags();
        }

        public void SaveAll(ObservableCollection<Tag> tags)
        {
            var json = JsonSerializer.Serialize(tags);
            File.WriteAllText(FilePath, json);
        }

        private ObservableCollection<Tag> CreateDefaultTags() => new()
        {
            new Tag { Name = "Important", Color = "#FF0000" },
            new Tag { Name = "Urgent", Color = "#FF6B00" },
            new Tag { Name = "Later", Color = "#00FF00" },
            new Tag { Name = "Ideas", Color = "#0000FF" }
        };
    }

}
