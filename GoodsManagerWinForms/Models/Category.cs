using System;
using System.Collections.Generic;

namespace GoodsManagerWinForms.Models
{
    /// <summary>
    /// Категория товаров (справочная таблица, сторона "один")
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Идентификатор категории (первичный ключ)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Навигационное свойство: товары этой категории
        /// </summary>
        public ICollection<Product> Products { get; set; } = new List<Product>();

        /// <summary>
        /// Строковое представление категории
        /// </summary>
        public override string ToString() => $"[{Id}] {Name}";
    }
}