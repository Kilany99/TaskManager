using FluentValidation;
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
    public class TaskValidator : AbstractValidator<TaskItem>
    {
        public TaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now).WithMessage("Due date must be in the future");

            RuleFor(x => x.Category)
                .NotNull().WithMessage("Category is required");

            RuleFor(x => x.Tags)
                .Must(t => t.Any()).WithMessage("At least one tag must be selected");
        }
        
    }
}

