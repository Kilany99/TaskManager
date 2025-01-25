using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
using TaskManager.Models.Enums;
using TaskManager.Utilities.Validators;

namespace TaskManager
{
    public class TaskItem : INotifyPropertyChanged
    {
        private string? _title;
        private string? _description;
        private DateTime _dueDate;
        private bool _isCompleted;
        private Priority _priority;
        private Status _status;
        private Category? _category;
        private DateTime _createdDate;
        private readonly TaskValidator _validator = new();
        private ObservableCollection<Tag> _tags;
        private readonly Dictionary<string, List<string>> _errors = new();

        public TaskItem()
        {
            _validator = new TaskValidator();
            Tags = new ObservableCollection<Tag>();

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
        public Guid Id { get; } = Guid.NewGuid();
        public Priority Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
                _validator.Validate(this);
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged();
                _validator.Validate(this);
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
        public bool HasErrors => _errors.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            return _validator.Validate(this).Errors
                .Where(e => e.PropertyName == propertyName)
                .Select(e => e.ErrorMessage);
        }
        public ObservableCollection<Tag> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged();
                _validator.Validate(this);
                Tags.CollectionChanged += (s, e) => _validator.Validate(this);
            }
        }


        public RecurrencePattern Recurrence { get; set; } = RecurrencePattern.None;
        public int RecurrenceInterval { get; set; } = 1;
        public DateTime? NextDueDate { get; set; }
        public bool IsRecurring => Recurrence != RecurrencePattern.None;

        public void Reschedule()
        {
            if (!IsRecurring) return;

            NextDueDate = DueDate.Add(Recurrence switch
            {
                RecurrencePattern.Daily => TimeSpan.FromDays(RecurrenceInterval),
                RecurrencePattern.Weekly => TimeSpan.FromDays(7 * RecurrenceInterval),
                RecurrencePattern.Monthly => TimeSpan.FromDays(30 * RecurrenceInterval), // Approximation
                _ => TimeSpan.Zero
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
