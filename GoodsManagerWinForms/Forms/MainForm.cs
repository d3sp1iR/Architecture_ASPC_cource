using System;
using System.Windows.Forms;

namespace GoodsManagerWinForms.Forms
{
    /// <summary>
    /// Главная форма приложения с навигационным меню
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnCategories = new Button();
            this.btnProducts = new Button();
            this.btnReport = new Button();
            this.SuspendLayout();

            // btnCategories
            this.btnCategories.Location = new System.Drawing.Point(50, 50);
            this.btnCategories.Size = new System.Drawing.Size(200, 50);
            this.btnCategories.Text = "Управление категориями";
            this.btnCategories.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCategories.Click += new EventHandler(this.btnCategories_Click);

            // btnProducts
            this.btnProducts.Location = new System.Drawing.Point(50, 120);
            this.btnProducts.Size = new System.Drawing.Size(200, 50);
            this.btnProducts.Text = "Управление товарами";
            this.btnProducts.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnProducts.Click += new EventHandler(this.btnProducts_Click);

            // btnReport
            this.btnReport.Location = new System.Drawing.Point(50, 190);
            this.btnReport.Size = new System.Drawing.Size(200, 50);
            this.btnReport.Text = "Отчёты";
            this.btnReport.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnReport.Click += new EventHandler(this.btnReport_Click);

            // MainForm
            this.Text = "Управление каталогом товаров";
            this.Size = new System.Drawing.Size(320, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Controls.AddRange(new Control[] { btnCategories, btnProducts, btnReport });

            this.ResumeLayout(false);
        }

        private Button btnCategories;
        private Button btnProducts;
        private Button btnReport;

        private void btnCategories_Click(object? sender, EventArgs e)
        {
            var form = new CategoriesForm();
            form.ShowDialog();
        }

        private void btnProducts_Click(object? sender, EventArgs e)
        {
            var form = new ProductsForm();
            form.ShowDialog();
        }

        private void btnReport_Click(object? sender, EventArgs e)
        {
            var form = new ReportForm();
            form.ShowDialog();
        }
    }
}