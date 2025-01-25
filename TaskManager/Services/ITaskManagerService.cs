using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Repositories;

namespace TaskManager.Services
{
    public interface ITaskManagerService
    {
        ObservableCollection<TaskItem> Tasks { get; }
        TaskItem SelectedTask { get; set; }
        void AddTask();
        void DeleteTask();
        void SaveTask();
        void UpdateFilter();
    }

    public class TaskManagerService : ITaskManagerService
    {
        private readonly ITaskRepository _taskRepository;
        private TaskItem _selectedTask = new();

        public TaskManagerService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
            Tasks = _taskRepository.GetAll();
        }

        public ObservableCollection<TaskItem> Tasks { get; private set; }

        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set => _selectedTask = value;
        }

        public void AddTask()
        {
            Tasks.Add(new TaskItem { DueDate = DateTime.Now.AddDays(1) });
            SelectedTask = Tasks.Last();
            SaveAll();
        }

        public void DeleteTask()
        {
            if (SelectedTask == null) return;

            Tasks.Remove(SelectedTask);
            SaveAll();
        }

        public void SaveTask()
        {
            if (SelectedTask == null || SelectedTask.HasErrors) return;

            _taskRepository.Save(SelectedTask);
            UpdateFilter();
        }

        public void UpdateFilter() => Tasks = new ObservableCollection<TaskItem>(Tasks);

        public void SaveAll() => _taskRepository.SaveAll(Tasks);
    }
}

