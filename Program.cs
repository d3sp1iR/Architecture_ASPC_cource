using System;
using System.Windows.Forms;
using GoodsManagerWinForms.Data;
using GoodsManagerWinForms.Forms;

namespace GoodsManagerWinForms
{
    /// <summary>
    /// Точка входа в приложение
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Главный метод приложения
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Инициализация базы данных
            using (var context = new AppDbContext())
            {
                context.InitializeDatabase();
            }

            Application.Run(new MainForm());
        }
    }
}