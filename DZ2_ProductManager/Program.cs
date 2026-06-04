using DZ2_ProductManager.Database;
using DZ2_ProductManager.Models;
using DZ2_ProductManager.Reports;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GoodsManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            string dbPath = "goods.db";
            string categoriesCsv = Path.Combine(AppContext.BaseDirectory, "Data", "categories.csv");
            string productsCsv = Path.Combine(AppContext.BaseDirectory, "Data", "products.csv");

            var db = new DatabaseManager(dbPath);
            db.InitializeDatabase(categoriesCsv, productsCsv);

            Console.WriteLine("\n=== Управление каталогом товаров ===\n");

            string choice;
            do
            {
                Console.WriteLine("┌─────────────────────────────────────┐");
                Console.WriteLine("│          ГЛАВНОЕ МЕНЮ              │");
                Console.WriteLine("├─────────────────────────────────────┤");
                Console.WriteLine("│ 1 — Показать все категории         │");
                Console.WriteLine("│ 2 — Показать все товары            │");
                Console.WriteLine("│ 3 — Добавить товар                 │");
                Console.WriteLine("│ 4 — Редактировать товар            │");
                Console.WriteLine("│ 5 — Удалить товар                  │");
                Console.WriteLine("│ 6 — Отчёты                         │");
                Console.WriteLine("│ 7 — Фильтр по категории (группа Г) │");
                Console.WriteLine("│ 0 — Выход                          │");
                Console.WriteLine("└─────────────────────────────────────┘");
                Console.Write("Ваш выбор: ");

                choice = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowCategories(db); break;
                    case "2": ShowProducts(db); break;
                    case "3": AddProduct(db); break;
                    case "4": EditProduct(db); break;
                    case "5": DeleteProduct(db); break;
                    case "6": ReportsMenu(db); break;
                    case "7": FilterByCategory(db); break;
                    case "0": Console.WriteLine("До свидания!"); break;
                    default: Console.WriteLine("Неверный пункт меню."); break;
                }
                Console.WriteLine();
            } while (choice != "0");
        }

        static void Pause()
        {
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
            Console.Clear();
        }

        static void ShowCategories(DatabaseManager db)
        {
            Console.WriteLine("--- Все категории товаров ---");
            var categories = db.GetAllCategories();
            foreach (var category in categories)
                Console.WriteLine($"  {category}");
            Console.WriteLine($"Итого: {categories.Count} категорий");
            Pause();
        }

        static void ShowProducts(DatabaseManager db)
        {
            Console.WriteLine("--- Все товары ---");
            var products = db.GetAllProducts();
            foreach (var product in products)
                Console.WriteLine($"  {product}");
            Console.WriteLine($"Итого: {products.Count} товаров");
            Pause();
        }

        static void AddProduct(DatabaseManager db)
        {
            Console.WriteLine("--- Добавление товара ---");

            Console.WriteLine("Доступные категории:");
            var categories = db.GetAllCategories();
            foreach (var cat in categories)
                Console.WriteLine($"  {cat}");

            Console.Write("ID категории: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                Pause();
                return;
            }

            if (!categories.Any(c => c.Id == categoryId))
            {
                Console.WriteLine("Ошибка: категория с таким ID не существует.");
                Pause();
                return;
            }

            Console.Write("Название товара: ");
            string name = Console.ReadLine()?.Trim() ?? "";
            if (name.Length == 0)
            {
                Console.WriteLine("Ошибка: название не может быть пустым.");
                Pause();
                return;
            }

            Console.Write("Цена (руб.): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Ошибка: введите число.");
                Pause();
                return;
            }

            try
            {
                var product = new Product(0, categoryId, name, price);
                db.AddProduct(product);
                Console.WriteLine("Товар успешно добавлен.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            Pause();
        }

        static void EditProduct(DatabaseManager db)
        {
            Console.WriteLine("--- Редактирование товара ---");
            Console.Write("Введите ID товара: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                Pause();
                return;
            }

            var product = db.GetProductById(id);
            if (product == null)
            {
                Console.WriteLine($"Товар с ID={id} не найден.");
                Pause();
                return;
            }

            Console.WriteLine($"Текущие данные: {product}");
            Console.WriteLine("(Нажмите Enter, чтобы оставить значение без изменений)");

            Console.Write($"Название [{product.Name}]: ");
            string input = Console.ReadLine()?.Trim() ?? "";
            if (input.Length > 0)
                product.Name = input;

            Console.Write($"ID категории [{product.CategoryId}]: ");
            input = Console.ReadLine()?.Trim() ?? "";
            if (input.Length > 0 && int.TryParse(input, out int newCategoryId))
                product.CategoryId = newCategoryId;

            Console.Write($"Цена [{product.Price:F2}]: ");
            input = Console.ReadLine()?.Trim() ?? "";
            if (input.Length > 0 && decimal.TryParse(input, out decimal newPrice))
            {
                try
                {
                    product.Price = newPrice;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Pause();
                    return;
                }
            }

            db.UpdateProduct(product);
            Console.WriteLine("Данные обновлены.");
            Pause();
        }

        static void DeleteProduct(DatabaseManager db)
        {
            Console.WriteLine("--- Удаление товара ---");
            Console.Write("Введите ID товара: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                Pause();
                return;
            }

            var product = db.GetProductById(id);
            if (product == null)
            {
                Console.WriteLine($"Товар с ID={id} не найден.");
                Pause();
                return;
            }

            Console.Write($"Удалить «{product.Name}»? (да/нет): ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (confirm == "да" || confirm == "lf")
            {
                db.DeleteProduct(id);
                Console.WriteLine("Товар удалён.");
            }
            else
            {
                Console.WriteLine("Удаление отменено.");
            }
            Pause();
        }

        static void ReportsMenu(DatabaseManager db)
        {
            string choice;
            do
            {
                Console.WriteLine("--- Отчёты ---");
                Console.WriteLine(" 1 - Список товаров с категориями");
                Console.WriteLine(" 2 - Количество товаров по категориям");
                Console.WriteLine(" 3 - Средняя цена товаров по категориям");
                Console.WriteLine(" 0 - Назад");
                Console.Write("Ваш выбор: ");

                choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1": Report1_ProductsWithCategories(db); break;
                    case "2": Report2_CountByCategory(db); break;
                    case "3": Report3_AvgPriceByCategory(db); break;
                    case "0": break;
                    default: Console.WriteLine("Неверный пункт."); Pause(); break;
                }
                Console.WriteLine();
            } while (choice != "0");
        }

        static void Report1_ProductsWithCategories(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"
                    SELECT p.product_name, c.category_name, p.price
                    FROM products p
                    JOIN categories c ON p.category_id = c.category_id
                    ORDER BY p.product_name
                ")
                .Title("Каталог товаров по категориям")
                .Header("Товар", "Категория", "Цена (руб.)")
                .ColumnWidths(30, 20, 15)
                .Numbered()
                .Footer("Всего товаров")
                .Print();
            Pause();
        }

        static void Report2_CountByCategory(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"
                    SELECT c.category_name, COUNT(*) AS product_count
                    FROM products p
                    JOIN categories c ON p.category_id = c.category_id
                    GROUP BY c.category_name
                    ORDER BY product_count DESC
                ")
                .Title("Количество товаров по категориям")
                .Header("Категория", "Кол-во товаров")
                .ColumnWidths(25, 15)
                .Print();
            Pause();
        }

        static void Report3_AvgPriceByCategory(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"
                    SELECT c.category_name, ROUND(AVG(p.price), 2) AS avg_price
                    FROM products p
                    JOIN categories c ON p.category_id = c.category_id
                    GROUP BY c.category_name
                    ORDER BY avg_price DESC
                ")
                .Title("Средняя цена товаров по категориям")
                .Header("Категория", "Средняя цена (руб.)")
                .ColumnWidths(25, 20)
                .Print();
            Pause();
        }

        static void FilterByCategory(DatabaseManager db)
        {
            Console.WriteLine("--- Фильтр товаров по категории ---");
            Console.WriteLine("Доступные категории:");
            var categories = db.GetAllCategories();
            foreach (var cat in categories)
                Console.WriteLine($"  {cat}");

            Console.Write("Введите ID категории: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                Pause();
                return;
            }

            var category = db.GetAllCategories().Find(c => c.Id == categoryId);
            if (category == null)
            {
                Console.WriteLine("Категория не найдена.");
                Pause();
                return;
            }

            var products = db.GetProductsByCategory(categoryId);
            if (products.Count == 0)
            {
                Console.WriteLine($"В категории «{category.Name}» нет товаров.");
                Pause();
                return;
            }

            Console.WriteLine($"\nТовары в категории «{category.Name}»:");
            decimal totalPrice = 0;
            foreach (var product in products)
            {
                Console.WriteLine($"  {product}");
                totalPrice += product.Price;
            }
            Console.WriteLine($"Итого: {products.Count} товаров");
            Console.WriteLine($"Общая стоимость: {totalPrice:F2} руб.");
            Console.WriteLine($"Средняя цена: {totalPrice / products.Count:F2} руб.");
            Pause();
        }
    }
}