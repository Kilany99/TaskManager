using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManager.Utilities;
using TaskManager.ViewModels;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(
                ServiceLocator.CategoryRepository,
                ServiceLocator.TagRepository,
                ServiceLocator.TaskManager,
                ServiceLocator.ReminderService,
                ServiceLocator.StatisticsService
            );

            ((MainViewModel)DataContext).OnReminderTriggered += ShowReminderNotification;
        }

        private void ShowReminderNotification(string message)
        {
            Dispatcher.Invoke(() =>
                MessageBox.Show(message, "Task Reminder", MessageBoxButton.OK));
        }

        
    }
}