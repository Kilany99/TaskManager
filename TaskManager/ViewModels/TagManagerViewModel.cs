using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskManager.Models;

namespace TaskManager.ViewModels
{
    public class TagManagerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Tag> AvailableTags { get; } = new();
        public ICommand CreateTagCommand => new RelayCommand(CreateTag);

        private void CreateTag(object? obj)
        {
            AvailableTags.Add(new Tag { Name = "New Tag", Color = "#FF808080" });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
