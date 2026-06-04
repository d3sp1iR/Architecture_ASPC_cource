using System;
using System.Linq;
using System.Windows.Forms;
using GoodsManagerWinForms.Data;
using GoodsManagerWinForms.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodsManagerWinForms.Forms
{
    /// <summary>
    /// Форма для управления категориями товаров (CRUD)
    /// </summary>
    public partial class CategoriesForm : Form
    {
        private AppDbContext _context;
        private int? _editingId = null;

        public CategoriesForm()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.dgvCategories = new DataGridView();
            this.txtName = new TextBox();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnCancel = new Button();
            this.lblName = new Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategories)).BeginInit();
            this.SuspendLayout();

            // dgvCategories
            this.dgvCategories.Location = new System.Drawing.Point(12, 12);
            this.dgvCategories.Size = new System.Drawing.Size(400, 300);
            this.dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvCategories.MultiSelect = false;
            this.dgvCategories.ReadOnly = true;
            this.dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCategories.SelectionChanged += new EventHandler(this.dgvCategories_SelectionChanged);

            // lblName
            this.lblName.Location = new System.Drawing.Point(12, 330);
            this.lblName.Text = "Название категории:";
            this.lblName.Size = new System.Drawing.Size(120, 23);

            // txtName
            this.txtName.Location = new System.Drawing.Point(140, 327);
            this.txtName.Size = new System.Drawing.Size(272, 23);

            // btnAdd
            this.btnAdd.Location = new System.Drawing.Point(12, 360);
            this.btnAdd.Text = "Добавить";
            this.btnAdd.Size = new System.Drawing.Size(90, 30);
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);

            // btnEdit
            this.btnEdit.Location = new System.Drawing.Point(108, 360);
            this.btnEdit.Text = "Редактировать";
            this.btnEdit.Size = new System.Drawing.Size(100, 30);
            this.btnEdit.Click += new EventHandler(this.btnEdit_Click);

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(214, 360);
            this.btnDelete.Text = "Удалить";
            this.btnDelete.Size = new System.Drawing.Size(90, 30);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(310, 360);
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // CategoriesForm
            this.Text = "Управление категориями";
            this.Size = new System.Drawing.Size(430, 430);
            this.Controls.AddRange(new Control[] { dgvCategories, lblName, txtName, btnAdd, btnEdit, btnDelete, btnCancel });

            ((System.ComponentModel.ISupportInitialize)(this.dgvCategories)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private DataGridView dgvCategories;
        private TextBox txtName;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnCancel;
        private Label lblName;

        private void LoadCategories()
        {
            using (_context = new AppDbContext())
            {
                var categories = _context.Categories.OrderBy(c => c.Name).ToList();
                dgvCategories.DataSource = null;
                dgvCategories.DataSource = categories;
                dgvCategories.Columns["Id"].HeaderText = "ID";
                dgvCategories.Columns["Name"].HeaderText = "Категория";
                dgvCategories.Columns["Products"].Visible = false;
            }
        }

        private void ClearForm()
        {
            _editingId = null;
            txtName.Text = "";
            txtName.Focus();
        }

        private void dgvCategories_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                var category = dgvCategories.SelectedRows[0].DataBoundItem as Category;
                if (category != null)
                {
                    _editingId = category.Id;
                    txtName.Text = category.Name;
                }
            }
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название категории!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new AppDbContext())
            {
                var category = new Category { Name = txtName.Text.Trim() };
                context.Categories.Add(category);
                context.SaveChanges();
            }

            LoadCategories();
            ClearForm();
            MessageBox.Show("Категория добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEdit_Click(object? sender, EventArgs e)
        {
            if (_editingId == null)
            {
                MessageBox.Show("Выберите категорию для редактирования!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название категории!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new AppDbContext())
            {
                var category = context.Categories.Find(_editingId);
                if (category != null)
                {
                    category.Name = txtName.Text.Trim();
                    context.SaveChanges();
                }
            }

            LoadCategories();
            ClearForm();
            MessageBox.Show("Категория обновлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (_editingId == null)
            {
                MessageBox.Show("Выберите категорию для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new AppDbContext())
            {
                var category = context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == _editingId);
                if (category != null)
                {
                    // Проверка на наличие связанных товаров
                    if (category.Products.Any())
                    {
                        MessageBox.Show($"Невозможно удалить категорию «{category.Name}», так как с ней связано {category.Products.Count} товаров!",
                            "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var result = MessageBox.Show($"Удалить категорию «{category.Name}»?", "Подтверждение",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        context.Categories.Remove(category);
                        context.SaveChanges();
                        LoadCategories();
                        ClearForm();
                        MessageBox.Show("Категория удалена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            ClearForm();
        }
    }
}