using Microsoft.EntityFrameworkCore;
using GoodsManagerWinForms.Models;
using System;
using System.Linq;

namespace GoodsManagerWinForms.Data
{
    /// <summary>
    /// Контекст базы данных для работы с категориями и товарами
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Категории товаров
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Товары
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Настройка подключения к базе данных SQLite
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=goods.db");
        }

        /// <summary>
        /// Создание и заполнение базы данных начальными данными
        /// </summary>
        public void InitializeDatabase()
        {
            Database.EnsureCreated();

            // Проверяем, есть ли уже данные
            if (Categories.Any() || Products.Any())
                return;

            // Добавляем категории (не менее 4)
            var categories = new[]
            {
                new Category { Name = "Электроника" },
                new Category { Name = "Бытовая техника" },
                new Category { Name = "Одежда и обувь" },
                new Category { Name = "Книги" },
                new Category { Name = "Товары для дома" }
            };
            Categories.AddRange(categories);
            SaveChanges();

            // Добавляем товары (не менее 12)
            var products = new[]
            {
                new Product { CategoryId = 1, Name = "Смартфон Samsung Galaxy", Price = 49990 },
                new Product { CategoryId = 1, Name = "Ноутбук Lenovo", Price = 65900 },
                new Product { CategoryId = 1, Name = "Наушники Sony", Price = 8990 },
                new Product { CategoryId = 1, Name = "Планшет Huawei", Price = 25990 },
                new Product { CategoryId = 2, Name = "Пылесос Bosch", Price = 18990 },
                new Product { CategoryId = 2, Name = "Микроволновая печь LG", Price = 12990 },
                new Product { CategoryId = 2, Name = "Стиральная машина Indesit", Price = 34990 },
                new Product { CategoryId = 2, Name = "Холодильник Samsung", Price = 52990 },
                new Product { CategoryId = 3, Name = "Куртка зимняя", Price = 5990 },
                new Product { CategoryId = 3, Name = "Кроссовки Nike", Price = 7990 },
                new Product { CategoryId = 3, Name = "Джинсы Levi's", Price = 4990 },
                new Product { CategoryId = 4, Name = "Война и мир", Price = 1250 },
                new Product { CategoryId = 4, Name = "Мастер и Маргарита", Price = 890 },
                new Product { CategoryId = 4, Name = "1984", Price = 650 },
                new Product { CategoryId = 5, Name = "Набор кастрюль", Price = 3990 },
                new Product { CategoryId = 5, Name = "Постельное бельё", Price = 2990 },
                new Product { CategoryId = 5, Name = "Стол письменный", Price = 14990 },
                new Product { CategoryId = 1, Name = "Смарт-часы Xiaomi", Price = 12990 }
            };
            Products.AddRange(products);
            SaveChanges();
        }
    }
}