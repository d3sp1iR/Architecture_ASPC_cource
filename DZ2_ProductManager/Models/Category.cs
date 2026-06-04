using System;
using System.Collections.Generic;
using System.Text;

namespace DZ2_ProductManager.Models
{
    /// <summary>
    /// Категория товаров (справочная таблица, сторона "один")
    /// </summary>
    class Category
    {
        /// <summary>
        /// Идентификатор категории
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="name">Название категории</param>
        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Category() : this(0, "") { }

        /// <summary>
        /// Строковое представление категории
        /// </summary>
        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }
    }
}
