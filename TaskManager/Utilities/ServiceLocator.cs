using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Repositories;
using TaskManager.Services;

namespace TaskManager.Utilities
{
    public static class ServiceLocator
    {
        public static ITaskRepository TaskRepository => new JsonTaskRepository();
        public static ITaskManagerService TaskManager => new TaskManagerService(TaskRepository);
        public static IReminderService ReminderService => new ReminderService( () => TaskManager.Tasks );
        public static IStatisticsService StatisticsService => new StatisticsService();
        public static ICategoryRepository CategoryRepository => new JsonCategoryRepository();
        public static ITagRepository TagRepository => new JsonTagRepository();
        public static INotificationService notificationService = new NotificationService(TaskManager);
    }
}
