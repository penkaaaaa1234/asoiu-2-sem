using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dz3.Data;
using Dz3.Models;

namespace Dz3.Forms
{
    /// <summary>
    /// Форма управления магазинами.
    /// </summary>
    public class StoresForm : Form
    {
        private DataGridView grid;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;

        public StoresForm()
        {
            Text = "Магазины";
            Width = 700;
            Height = 450;
            StartPosition = FormStartPosition.CenterParent;
            Padding = new Padding(12);
            AutoScaleMode = AutoScaleMode.Font;

            // grid fills available space above the buttons
            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width = 50 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            btnAdd = new Button { Text = "Добавить" };
            btnEdit = new Button { Text = "Редактировать" };
            btnDelete = new Button { Text = "Удалить" };

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 64, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0,8,12,8) };
            btnPanel.Controls.Add(btnDelete);
            btnPanel.Controls.Add(btnEdit);
            btnPanel.Controls.Add(btnAdd);

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            Controls.Add(grid);
            Controls.Add(btnPanel);

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var context = new AppDbContext();
                var list = context.Stores.OrderBy(s => s.Name).ToList();
                grid.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Store GetSelectedStore()
        {
            return grid.CurrentRow?.DataBoundItem as Store;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new StoreEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var context = new AppDbContext();
                    var s = new Store { Name = dlg.StoreName };
                    context.Stores.Add(s);
                    context.SaveChanges();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var store = GetSelectedStore();
            if (store == null) return;

            using var dlg = new StoreEditForm { StoreName = store.Name };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var context = new AppDbContext();
                    var s = context.Stores.Find(store.Id);
                    if (s != null)
                    {
                        s.Name = dlg.StoreName;
                        context.SaveChanges();
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var store = GetSelectedStore();
            if (store == null) return;

            if (MessageBox.Show("Удалить магазин?", "Подтвердите", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using var context = new AppDbContext();
                var hasOrders = context.Orders.Any(o => o.StoreId == store.Id);
                if (hasOrders)
                {
                    MessageBox.Show("Нельзя удалить магазин, у него есть связанные заказы.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var s = context.Stores.Find(store.Id);
                if (s != null)
                {
                    context.Stores.Remove(s);
                    context.SaveChanges();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
