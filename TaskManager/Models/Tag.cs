using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{

    public class Tag : INotifyPropertyChanged
    {
        private string _name;
        private string _color;
        private bool _isSelected;

        public int Id   { get; set; }
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
