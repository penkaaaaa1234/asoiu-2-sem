using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dz3.Data;
using Dz3.Models;
using Microsoft.EntityFrameworkCore;

namespace Dz3.Forms
{
    /// <summary>
    /// Форма управления заказами.
    /// </summary>
    public class OrdersForm : Form
    {
        private DataGridView grid;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;

        public OrdersForm()
        {
            Text = "Заказы";
            Width = 900;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;

            grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width = 50 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Название", DataPropertyName = "Name", Width = 250 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Магазин", DataPropertyName = "StoreName", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Сумма", DataPropertyName = "Amount", Width = 120 });


            // Styled buttons panel
            btnAdd = new Button { Text = "Добавить", Width = 120, Height = 36 };
            btnEdit = new Button { Text = "Редактировать", Width = 140, Height = 36 };
            btnDelete = new Button { Text = "Удалить", Width = 120, Height = 36 };

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 56, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0,8,12,8) };
            foreach (var b in new[] { btnAdd, btnEdit, btnDelete })
            {
                b.FlatStyle = FlatStyle.Flat;
                b.ForeColor = Color.White;
                b.Font = new Font(Font, FontStyle.Bold);
                b.Margin = new Padding(8, 0, 0, 0);
            }
            btnAdd.BackColor = Color.FromArgb(0, 120, 215);
            btnEdit.BackColor = Color.FromArgb(0, 120, 215);
            btnDelete.BackColor = Color.FromArgb(232, 17, 35);

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
                var list = context.Orders.Include(o => o.Store)
                    .Select(o => new
                    {
                        o.Id,
                        o.Name,
                        StoreName = o.Store != null ? o.Store.Name : string.Empty,
                        o.Amount,
                        o.StoreId
                    })
                    .OrderBy(o => o.Name)
                    .ToList();
                grid.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int? GetSelectedOrderId()
        {
            return grid.CurrentRow?.Cells[0]?.Value as int?;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using var dlg = new OrderEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.Amount < 0)
                {
                    MessageBox.Show("Amount не может быть отрицательным.", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using var context = new AppDbContext();
                    var order = new Order
                    {
                        Name = dlg.OrderName,
                        StoreId = dlg.StoreId,
                        Amount = dlg.Amount
                    };
                    context.Orders.Add(order);
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
            var selected = grid.CurrentRow?.DataBoundItem;
            if (selected == null) return;
            var id = (int)grid.CurrentRow.Cells[0].Value;
            try
            {
                using var context = new AppDbContext();
                var order = context.Orders.Find(id);
                if (order == null) return;

                using var dlg = new OrderEditForm
                {
                    OrderName = order.Name,
                    StoreId = order.StoreId,
                    Amount = order.Amount
                };

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.Amount < 0)
                    {
                        MessageBox.Show("Amount не может быть отрицательным.", "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    order.Name = dlg.OrderName;
                    order.StoreId = dlg.StoreId;
                    order.Amount = dlg.Amount;
                    context.SaveChanges();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var selected = grid.CurrentRow?.DataBoundItem;
            if (selected == null) return;
            var id = (int)grid.CurrentRow.Cells[0].Value;

            if (MessageBox.Show("Удалить заказ?", "Подтвердите", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using var context = new AppDbContext();
                var order = context.Orders.Find(id);
                if (order != null)
                {
                    context.Orders.Remove(order);
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
