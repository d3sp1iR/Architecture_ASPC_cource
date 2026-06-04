using System;
using System.Linq;
using System.Windows.Forms;
using GoodsManagerWinForms.Data;
using GoodsManagerWinForms.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodsManagerWinForms.Forms
{
    /// <summary>
    /// Форма для управления товарами (CRUD)
    /// </summary>
    public partial class ProductsForm : Form
    {
        private int? _editingId = null;

        public ProductsForm()
        {
            InitializeComponent();
            LoadProducts();
            LoadCategoriesToComboBox();
        }

        private void InitializeComponent()
        {
            this.dgvProducts = new DataGridView();
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.lblCategory = new Label();
            this.cmbCategory = new ComboBox();
            this.lblPrice = new Label();
            this.txtPrice = new TextBox();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            this.SuspendLayout();

            // dgvProducts
            this.dgvProducts.Location = new System.Drawing.Point(12, 12);
            this.dgvProducts.Size = new System.Drawing.Size(650, 300);
            this.dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvProducts.MultiSelect = false;
            this.dgvProducts.ReadOnly = true;
            this.dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProducts.SelectionChanged += new EventHandler(this.dgvProducts_SelectionChanged);

            // lblName
            this.lblName.Location = new System.Drawing.Point(12, 330);
            this.lblName.Text = "Название товара:";
            this.lblName.Size = new System.Drawing.Size(120, 23);

            // txtName
            this.txtName.Location = new System.Drawing.Point(140, 327);
            this.txtName.Size = new System.Drawing.Size(200, 23);

            // lblCategory
            this.lblCategory.Location = new System.Drawing.Point(350, 330);
            this.lblCategory.Text = "Категория:";
            this.lblCategory.Size = new System.Drawing.Size(70, 23);

            // cmbCategory
            this.cmbCategory.Location = new System.Drawing.Point(420, 327);
            this.cmbCategory.Size = new System.Drawing.Size(150, 23);
            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            // lblPrice
            this.lblPrice.Location = new System.Drawing.Point(580, 330);
            this.lblPrice.Text = "Цена (руб.):";
            this.lblPrice.Size = new System.Drawing.Size(80, 23);

            // txtPrice
            this.txtPrice.Location = new System.Drawing.Point(660, 327);
            this.txtPrice.Size = new System.Drawing.Size(100, 23);

            // btnAdd
            this.btnAdd.Location = new System.Drawing.Point(12, 370);
            this.btnAdd.Text = "Добавить";
            this.btnAdd.Size = new System.Drawing.Size(100, 30);
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);

            // btnEdit
            this.btnEdit.Location = new System.Drawing.Point(118, 370);
            this.btnEdit.Text = "Редактировать";
            this.btnEdit.Size = new System.Drawing.Size(100, 30);
            this.btnEdit.Click += new EventHandler(this.btnEdit_Click);

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(224, 370);
            this.btnDelete.Text = "Удалить";
            this.btnDelete.Size = new System.Drawing.Size(100, 30);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(330, 370);
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // ProductsForm
            this.Text = "Управление товарами";
            this.Size = new System.Drawing.Size(800, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Controls.AddRange(new Control[] { dgvProducts, lblName, txtName, lblCategory, cmbCategory,
                lblPrice, txtPrice, btnAdd, btnEdit, btnDelete, btnCancel });

            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private DataGridView dgvProducts;
        private TextBox txtName;
        private TextBox txtPrice;
        private ComboBox cmbCategory;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnCancel;
        private Label lblName;
        private Label lblCategory;
        private Label lblPrice;

        private void LoadProducts()
        {
            using (var context = new AppDbContext())
            {
                var products = context.Products
                    .Include(p => p.Category)
                    .OrderBy(p => p.Name)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        Category = p.Category != null ? p.Category.Name : "Без категории",
                        p.Price
                    })
                    .ToList();

                dgvProducts.DataSource = null;
                dgvProducts.DataSource = products;
                dgvProducts.Columns["Id"].HeaderText = "ID";
                dgvProducts.Columns["Name"].HeaderText = "Товар";
                dgvProducts.Columns["Category"].HeaderText = "Категория";
                dgvProducts.Columns["Price"].HeaderText = "Цена (руб.)";
            }
        }

        private void LoadCategoriesToComboBox()
        {
            using (var context = new AppDbContext())
            {
                var categories = context.Categories.OrderBy(c => c.Name).ToList();
                cmbCategory.DataSource = categories;
                cmbCategory.DisplayMember = "Name";
                cmbCategory.ValueMember = "Id";
            }
        }

        private void ClearForm()
        {
            _editingId = null;
            txtName.Text = "";
            txtPrice.Text = "";
            cmbCategory.SelectedIndex = -1;
            txtName.Focus();
        }

        private void dgvProducts_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var row = dgvProducts.SelectedRows[0];
                _editingId = (int)row.Cells["Id"].Value;
                txtName.Text = row.Cells["Name"].Value?.ToString() ?? "";
                txtPrice.Text = row.Cells["Price"].Value?.ToString() ?? "";

                var categoryName = row.Cells["Category"].Value?.ToString();
                if (!string.IsNullOrEmpty(categoryName))
                {
                    foreach (var item in cmbCategory.Items)
                    {
                        var cat = item as Category;
                        if (cat != null && cat.Name == categoryName)
                        {
                            cmbCategory.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    var product = new Product
                    {
                        Name = txtName.Text.Trim(),
                        CategoryId = (int)cmbCategory.SelectedValue,
                        Price = price
                    };
                    context.Products.Add(product);
                    context.SaveChanges();
                }

                LoadProducts();
                ClearForm();
                MessageBox.Show("Товар добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object? sender, EventArgs e)
        {
            if (_editingId == null)
            {
                MessageBox.Show("Выберите товар для редактирования!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    var product = context.Products.Find(_editingId);
                    if (product != null)
                    {
                        product.Name = txtName.Text.Trim();
                        product.CategoryId = (int)cmbCategory.SelectedValue;
                        product.Price = price;
                        context.SaveChanges();
                    }
                }

                LoadProducts();
                ClearForm();
                MessageBox.Show("Товар обновлён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (_editingId == null)
            {
                MessageBox.Show("Выберите товар для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Удалить выбранный товар?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                using (var context = new AppDbContext())
                {
                    var product = context.Products.Find(_editingId);
                    if (product != null)
                    {
                        context.Products.Remove(product);
                        context.SaveChanges();
                    }
                }

                LoadProducts();
                ClearForm();
                MessageBox.Show("Товар удалён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            ClearForm();
        }
    }
}