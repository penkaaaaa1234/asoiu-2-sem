using System.Collections.Generic;

namespace Dz3.Models
{
    /// <summary>
    /// Магазин (Store).
    /// </summary>
    public class Store
    {
        /// <summary>
        /// Идентификатор магазина.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название магазина.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Навигационное свойство — заказы магазина.
        /// </summary>
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
