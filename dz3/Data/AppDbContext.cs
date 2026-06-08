using Microsoft.EntityFrameworkCore;
using Dz3.Models;

namespace Dz3.Data
{
    /// <summary>
    /// Контекст базы данных приложения.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Магазины.
        /// </summary>
        public DbSet<Store> Stores { get; set; }

        /// <summary>
        /// Заказы.
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Конфигурирование подключения.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=app.db");
        }
    }
}
