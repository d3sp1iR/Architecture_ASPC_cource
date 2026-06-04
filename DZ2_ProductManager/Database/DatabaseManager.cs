using DZ2_ProductManager.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace DZ2_ProductManager.Database
{
    /// <summary>
    /// Управление базой данных SQLite. Инкапсулирует все операции с БД
    /// </summary>
    class DatabaseManager
    {
        private string _connectionString;

        /// <summary>
        /// Конструктор. Принимает путь к файлу БД
        /// </summary>
        /// <param name="dbPath">Путь к файлу базы данных</param>
        public DatabaseManager(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
        }

        /// <summary>
        /// Инициализирует базу данных: создаёт таблицы и загружает CSV
        /// </summary>
        public void InitializeDatabase(string categoriesCsvPath, string productsCsvPath)
        {
            CreateTables();

            if (GetAllCategories().Count == 0 && File.Exists(categoriesCsvPath))
            {
                ImportCategoriesFromCsv(categoriesCsvPath);
                Console.WriteLine($"[OK] Загружены категории из {categoriesCsvPath}");
            }

            if (GetAllProducts().Count == 0 && File.Exists(productsCsvPath))
            {
                ImportProductsFromCsv(productsCsvPath);
                Console.WriteLine($"[OK] Загружены товары из {productsCsvPath}");
            }
        }

        /// <summary>
        /// Создание таблиц в базе данных
        /// </summary>
        private void CreateTables()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS categories (
                    category_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    category_name TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS products (
                    product_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    category_id INTEGER NOT NULL,
                    product_name TEXT NOT NULL,
                    price DECIMAL(10,2) NOT NULL,
                    FOREIGN KEY (category_id) REFERENCES categories(category_id)
                );
            ";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Импорт категорий из CSV
        /// </summary>
        private void ImportCategoriesFromCsv(string path)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string[] lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(';');
                if (parts.Length < 2) continue;

                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO categories (category_id, category_name) VALUES (@id, @name)";
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@name", parts[1]);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Импорт товаров из CSV
        /// </summary>
        private void ImportProductsFromCsv(string path)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string[] lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(';');
                if (parts.Length < 4) continue;

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO products (product_id, category_id, product_name, price)
                    VALUES (@id, @categoryId, @name, @price)
                ";
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@categoryId", int.Parse(parts[1]));
                cmd.Parameters.AddWithValue("@name", parts[2]);
                cmd.Parameters.AddWithValue("@price", decimal.Parse(parts[3]));
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Получить все категории
        /// </summary>
        public List<Category> GetAllCategories()
        {
            var result = new List<Category>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT category_id, category_name FROM categories ORDER BY category_id";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Category(reader.GetInt32(0), reader.GetString(1)));
            }
            return result;
        }

        /// <summary>
        /// Получить все товары
        /// </summary>
        public List<Product> GetAllProducts()
        {
            var result = new List<Product>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT product_id, category_id, product_name, price FROM products ORDER BY product_id";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Product(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetDecimal(3)
                ));
            }
            return result;
        }

        /// <summary>
        /// Получить товар по ID
        /// </summary>
        public Product GetProductById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT product_id, category_id, product_name, price FROM products WHERE product_id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Product(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetDecimal(3)
                );
            }
            return null;
        }

        /// <summary>
        /// Добавить товар (Id генерируется автоматически)
        /// </summary>
        public void AddProduct(Product product)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO products (category_id, product_name, price)
                VALUES (@categoryId, @name, @price)
            ";
            cmd.Parameters.AddWithValue("@categoryId", product.CategoryId);
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновить данные товара
        /// </summary>
        public void UpdateProduct(Product product)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE products
                SET category_id = @categoryId, product_name = @name, price = @price
                WHERE product_id = @id
            ";
            cmd.Parameters.AddWithValue("@id", product.Id);
            cmd.Parameters.AddWithValue("@categoryId", product.CategoryId);
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удалить товар по ID
        /// </summary>
        public void DeleteProduct(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM products WHERE product_id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Выполнить произвольный SQL-запрос для отчётов
        /// </summary>
        public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            using var reader = cmd.ExecuteReader();

            string[] columns = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                columns[i] = reader.GetName(i);

            var rows = new List<string[]>();
            while (reader.Read())
            {
                string[] row = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                    row[i] = reader.GetValue(i)?.ToString() ?? "";
                rows.Add(row);
            }

            return (columns, rows);
        }

        /// <summary>
        /// Получить товары конкретной категории (для группы Г)
        /// </summary>
        public List<Product> GetProductsByCategory(int categoryId)
        {
            var result = new List<Product>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT product_id, category_id, product_name, price
                FROM products WHERE category_id = @categoryId ORDER BY product_name
            ";
            cmd.Parameters.AddWithValue("@categoryId", categoryId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Product(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetDecimal(3)
                ));
            }
            return result;
        }
    }
}