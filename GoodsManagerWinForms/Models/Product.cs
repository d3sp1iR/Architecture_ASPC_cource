using System;

namespace GoodsManagerWinForms.Models
{
    /// <summary>
    /// Товар (основная таблица, сторона "много")
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Идентификатор товара (первичный ключ)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор категории (внешний ключ)
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Навигационное свойство: категория товара
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string Name { get; set; } = "";

        private decimal _price;

        /// <summary>
        /// Цена товара в рублях (не может быть отрицательной)
        /// </summary>
        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Цена не может быть отрицательной");
                _price = value;
            }
        }

        /// <summary>
        /// Строковое представление товара
        /// </summary>
        public override string ToString() => $"[{Id}] {Name}, цена: {Price:F2} руб.";
    }
}