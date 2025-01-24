

using Microsoft.EntityFrameworkCore;

namespace TaskManager
{
	public class TaskManagerContext : DbContext
	{
		public DbSet<Task> Tasks { get; set; }
		public DbSet<Category> Categories { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite("Data Source=taskmanager.db");
	}

}