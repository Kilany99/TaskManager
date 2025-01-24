using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Enums;


namespace TaskManager.Utilities.Validators
{
    public class TaskValidator : INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            return propertyName != null && _errors.ContainsKey(propertyName)
                ? _errors[propertyName]
                : Enumerable.Empty<string>();
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public void ValidateProperty(string propertyName, object? value)
        {
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(TaskItem.Title):
                    ValidateTitle(value as string);
                    break;
                case nameof(TaskItem.DueDate):
                    ValidateDueDate((DateTime)value);
                    break;
                case nameof(TaskItem.Priority):
                    ValidatePriority((Priority)value);
                    break;
            }
        }

        private void ValidateTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                AddError(nameof(TaskItem.Title), "Title is required");
            }
            else if (title.Length < 3)
            {
                AddError(nameof(TaskItem.Title), "Title must be at least 3 characters long");
            }
            else if (title.Length > 100)
            {
                AddError(nameof(TaskItem.Title), "Title cannot exceed 100 characters");
            }
        }

        private void ValidateDueDate(DateTime dueDate)
        {
            if (dueDate.Date < DateTime.Now.Date)
            {
                AddError(nameof(TaskItem.DueDate), "Due date cannot be in the past");
            }
        }

        private void ValidatePriority(Priority priority)
        {
            if (!Enum.IsDefined(typeof(Priority), priority))
            {
                AddError(nameof(TaskItem.Priority), "Invalid priority value");
            }
        }
    }
}

