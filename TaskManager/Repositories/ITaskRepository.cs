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
    public interface ITaskRepository
    {
        ObservableCollection<TaskItem> GetAll();
        void Save(TaskItem task);
        void Delete(TaskItem task);
        void SaveAll(ObservableCollection<TaskItem> tasks);
    }

    public class JsonTaskRepository : ITaskRepository
    {
        private const string FilePath = "tasks.json";

        public ObservableCollection<TaskItem> GetAll()
        {
            if (!File.Exists(FilePath)) return new ObservableCollection<TaskItem>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(json)
                ?? new ObservableCollection<TaskItem>();
        }

        public void SaveAll(ObservableCollection<TaskItem> tasks)
        {
            try
            {
                var json = JsonSerializer.Serialize(tasks);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Save(TaskItem task) => SaveAll(GetAll());
        public void Delete(TaskItem task) => SaveAll(GetAll());
    }
}
