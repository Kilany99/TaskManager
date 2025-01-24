using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Enums;
using TaskManager.Models;
using TaskManager.Utilities.Validators;

namespace TaskManager
{
    public class TaskItem : INotifyPropertyChanged, IDataErrorInfo
    {
        private string? _title;
        private string? _description;
        private DateTime _dueDate;
        private bool _isCompleted;
        private Priority _priority;
        private Status _status;
        private Category? _category;
        private DateTime _createdDate;
        private readonly TaskValidator _validator;
        private ObservableCollection<Tag> _tags;
        public TaskItem()
        {
            _validator = new TaskValidator();
            _validator.ErrorsChanged += (s, e) => ErrorsChanged?.Invoke(this, e);
        }
        public string this[string columnName]
        {
            get
            {
                var validationResults = new List<ValidationResult>();
                var property = GetType().GetProperty(columnName);
                if (property != null)
                {
                    Validator.TryValidateProperty(
                        property.GetValue(this),
                        new ValidationContext(this) { MemberName = columnName },
                        validationResults
                    );
                }

                return validationResults.FirstOrDefault()?.ErrorMessage ?? string.Empty;
            }
        }

        public Priority Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(); }
        }

        [Required(ErrorMessage = "Title is required")]
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _title??string.Empty;
            set
            {
                _title = value; OnPropertyChanged();
            }
        }

        [CustomValidation(typeof(TaskItem), nameof(ValidateDueDate))]
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged();
            }
        }
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged();
            }
        }

        public Status Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public Category Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set { _createdDate = value; OnPropertyChanged(); }
        }
        public bool HasErrors => _validator.HasErrors;

        public IEnumerable GetErrors(string? propertyName) => _validator.GetErrors(propertyName);


        public ObservableCollection<Tag> Tags
        {
            get => _tags;
            set { _tags = value; OnPropertyChanged(); }
        }

        public string Error => throw new NotImplementedException();

        public static ValidationResult? ValidateDueDate(DateTime dueDate, ValidationContext context)
        {
            return dueDate < DateTime.Now.Date
                ? new ValidationResult("Due date must be in the future")
                : ValidationResult.Success;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
