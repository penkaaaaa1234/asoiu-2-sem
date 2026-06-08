namespace Dz3.Models
{
    /// <summary>
    /// Заказ (Order).
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Идентификатор заказа.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ — идентификатор магазина.
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Навигационное свойство — магазин.
        /// </summary>
        public Store Store { get; set; }

        /// <summary>
        /// Название/описание заказа.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сумма заказа в рублях.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
