using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Dz3.Data;

namespace Dz3.Forms
{
    /// <summary>
    /// Форма отчётов.
    /// </summary>
    public class ReportForm : Form
    {
        private DataGridView gridAllOrders;
        private DataGridView gridCounts;
        private DataGridView gridAverages;

        public ReportForm()
        {
            Text = "Отчет";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;

            gridAllOrders = new DataGridView { Dock = DockStyle.Top, Height = 240, ReadOnly = true, AutoGenerateColumns = true };
            gridCounts = new DataGridView { Dock = DockStyle.Top, Height = 200, ReadOnly = true, AutoGenerateColumns = true };
            gridAverages = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };

            Controls.Add(gridAverages);
            Controls.Add(gridCounts);
            Controls.Add(gridAllOrders);

            LoadReports();
        }

        private void LoadReports()
        {
            try
            {
                using var context = new AppDbContext();

                // Раздел 1: полный список заказов с названиями магазинов, отсортированный по названию магазина
                var all = context.Orders.Include(o => o.Store)
                    .Select(o => new { o.Id, o.Name, StoreName = o.Store != null ? o.Store.Name : string.Empty, o.Amount })
                    .OrderBy(o => o.StoreName)
                    .ToList();
                gridAllOrders.DataSource = all;

                // Раздел 2: количество заказов по магазинам
                var counts = context.Orders.Include(o => o.Store)
                    .GroupBy(o => o.Store != null ? o.Store.Name : "(Нет магазина)")
                    .Select(g => new { StoreName = g.Key, Count = g.Count() })
                    .OrderBy(x => x.StoreName)
                    .ToList();
                gridCounts.DataSource = counts;

                // Раздел 3: средняя сумма заказа по магазинам, сортировка по убыванию
                var averages = context.Orders.Include(o => o.Store)
                    .GroupBy(o => o.Store != null ? o.Store.Name : "(Нет магазина)")
                    .Select(g => new { StoreName = g.Key, AverageAmount = g.Average(x => x.Amount) })
                    .OrderByDescending(x => x.AverageAmount)
                    .ToList();
                gridAverages.DataSource = averages;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
