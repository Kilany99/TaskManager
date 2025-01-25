using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Enums;
using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Services
{
    public interface ITaskManagerService
    {
        ObservableCollection<TaskItem> Tasks { get; }
        TaskItem SelectedTask { get; set; }
        void AddTask(TaskItem task);
        void DeleteTask(TaskItem task);
        void SaveTask(TaskItem task);
        
        void UpdateTask(TaskItem task);

        void UpdateFilter();
        void CompleteTask(TaskItem task);

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

        public void AddTask(TaskItem task)
        {
            // Validate before adding
            if (task.HasErrors)
                throw new InvalidOperationException("Cannot add invalid task");

            task.CreatedDate = DateTime.Now;
            Tasks.Add(task);
            SaveAll();
        }

        public void DeleteTask(TaskItem task)
        {
            if (Tasks.Contains(task))
            {
                Tasks.Remove(task);
                SaveAll();
            }
        }

        public void UpdateTask(TaskItem task)
        {
            var existing = Tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing != null)
            {
                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.DueDate = task.DueDate;
                existing.Priority = task.Priority;
                existing.Category = task.Category;
                existing.Tags = new ObservableCollection<Tag>(task.Tags);
                
            }
        }

        public void UpdateFilter() => Tasks = new ObservableCollection<TaskItem>(Tasks);
        public void SaveAll()
        {
            _taskRepository.SaveAll(Tasks);

        }
        public void SaveTask(TaskItem task)
        {

            _taskRepository.Save(task);
        }

        public void CompleteTask(TaskItem task)
        {
            if (task.IsRecurring)
            {
                var newTask = new TaskItem
                {
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.NextDueDate ?? DateTime.Now.AddDays(1),
                    Recurrence = task.Recurrence,
                    RecurrenceInterval = task.RecurrenceInterval
                };
                newTask.Reschedule();
                Tasks.Add(newTask);
            }
            Tasks.Remove(task);
            SaveAll();
        }
    }
}

