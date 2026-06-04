using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using GoodsManagerWinForms.Data;

namespace GoodsManagerWinForms.Forms
{
    /// <summary>
    /// Форма для отображения отчётов (LINQ)
    /// </summary>
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
            LoadReports();
        }

        private void InitializeComponent()
        {
            this.tabControl = new TabControl();
            this.tabPage1 = new TabPage();
            this.dgvReport1 = new DataGridView();
            this.tabPage2 = new TabPage();
            this.dgvReport2 = new DataGridView();
            this.tabPage3 = new TabPage();
            this.dgvReport3 = new DataGridView();

            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport3)).BeginInit();
            this.SuspendLayout();

            // tabControl
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Size = new System.Drawing.Size(860, 500);
            this.tabControl.Dock = DockStyle.Fill;

            // tabPage1
            this.tabPage1.Text = "1. Список товаров по категориям";
            this.dgvReport1.Dock = DockStyle.Fill;
            this.dgvReport1.ReadOnly = true;
            this.dgvReport1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.tabPage1.Controls.Add(this.dgvReport1);

            // tabPage2
            this.tabPage2.Text = "2. Количество товаров по категориям";
            this.dgvReport2.Dock = DockStyle.Fill;
            this.dgvReport2.ReadOnly = true;
            this.dgvReport2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.tabPage2.Controls.Add(this.dgvReport2);

            // tabPage3
            this.tabPage3.Text = "3. Средняя цена по категориям";
            this.dgvReport3.Dock = DockStyle.Fill;
            this.dgvReport3.ReadOnly = true;
            this.dgvReport3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.tabPage3.Controls.Add(this.dgvReport3);

            this.tabControl.Controls.AddRange(new TabPage[] { tabPage1, tabPage2, tabPage3 });

            // ReportForm
            this.Text = "Отчёты";
            this.Size = new System.Drawing.Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Controls.Add(this.tabControl);

            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport3)).EndInit();
            this.ResumeLayout(false);
        }

        private TabControl tabControl;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private DataGridView dgvReport1;
        private DataGridView dgvReport2;
        private DataGridView dgvReport3;

        private void LoadReports()
        {
            using (var context = new AppDbContext())
            {
                // Отчёт 1: Полный список товаров с категориями (JOIN)
                // Загружаем данные с Include, а потом преобразуем в памяти
                var products = context.Products
                    .Include(p => p.Category)
                    .OrderBy(p => p.Name)
                    .ToList();

                var report1 = products.Select(p => new
                {
                    Название = p.Name,
                    Категория = p.Category != null ? p.Category.Name : "Без категории",
                    Цена = p.Price
                }).ToList();

                dgvReport1.DataSource = report1;

                // Отчёт 2: Количество товаров по категориям (GROUP BY COUNT)
                var report2 = context.Products
                    .GroupBy(p => p.CategoryId)
                    .Select(g => new
                    {
                        CategoryId = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                // Получаем названия категорий отдельно
                var categories = context.Categories.ToDictionary(c => c.Id, c => c.Name);

                var report2Formatted = report2.Select(r => new
                {
                    Категория = categories.ContainsKey(r.CategoryId) ? categories[r.CategoryId] : "Без категории",
                    Количество = r.Count
                }).OrderBy(r => r.Категория).ToList();

                dgvReport2.DataSource = report2Formatted;

                // Отчёт 3: Средняя цена по категориям (GROUP BY AVG)
                var report3 = context.Products
                    .GroupBy(p => p.CategoryId)
                    .Select(g => new
                    {
                        CategoryId = g.Key,
                        AvgPrice = g.Average(p => p.Price)
                    })
                    .ToList();

                var report3Formatted = report3.Select(r => new
                {
                    Категория = categories.ContainsKey(r.CategoryId) ? categories[r.CategoryId] : "Без категории",
                    СредняяЦена = Math.Round(r.AvgPrice, 2)
                }).OrderByDescending(r => r.СредняяЦена).ToList();

                dgvReport3.DataSource = report3Formatted;
            }
        }
    }
}