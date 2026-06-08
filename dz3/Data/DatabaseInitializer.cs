using System;
using System.Linq;
using Dz3.Models;

namespace Dz3.Data
{
    /// <summary>
    /// Инициализация базы данных и начальных данных.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Создаёт базу и добавляет начальные данные, если таблицы пустые.
        /// </summary>
        public static void Initialize()
        {
            using var context = new AppDbContext();
            context.Database.EnsureCreated();

            if (context.Stores.Any() || context.Orders.Any())
                return;

            var stores = new[]
            {
                new Store { Name = "Центральный" },
                new Store { Name = "Северный" },
                new Store { Name = "Южный" },
                new Store { Name = "Восточный" }
            };

            context.Stores.AddRange(stores);
            context.SaveChanges();

            var rnd = new Random(1);
            var orders = Enumerable.Range(1, 12).Select(i =>
            {
                var store = stores[(i - 1) % stores.Length];
                return new Order
                {
                    Name = $"Заказ {i}",
                    StoreId = store.Id,
                    Amount = Math.Round((decimal)(rnd.NextDouble() * 10000), 2)
                };
            }).ToArray();

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}
