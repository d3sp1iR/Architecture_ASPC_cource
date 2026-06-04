using System;

namespace DZ2_ProductManager.Models
{
    /// <summary>
    /// Товар (основная таблица, сторона "много")
    /// </summary>
    class Product
    {
        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор категории (внешний ключ)
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string Name { get; set; }

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
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <param name="name">Название товара</param>
        /// <param name="price">Цена в рублях</param>
        public Product(int id, int categoryId, string name, decimal price)
        {
            Id = id;
            CategoryId = categoryId;
            Name = name;
            Price = price;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Product() : this(0, 0, "", 0) { }

        /// <summary>
        /// Строковое представление товара
        /// </summary>
        public override string ToString()
        {
            return $"[{Id}] {Name}, категория #{CategoryId}, цена: {Price:F2} руб.";
        }
    }
}