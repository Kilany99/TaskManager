using System.Configuration;
using System.Data;
using System.Windows;
using TaskManager.Utilities;
using TaskManager.ViewModels;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(
                    ServiceLocator.TaskManager,
                    ServiceLocator.ReminderService,
                    ServiceLocator.StatisticsService
                )
            };
            mainWindow.Show();
        }
    }
}
