using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dz3.Forms
{
    /// <summary>
    /// Главная форма приложения.
    /// </summary>
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Simple Shop App";
            Width = 640;
            Height = 280;
            StartPosition = FormStartPosition.CenterScreen;

            var btnStores = new Button { Text = "Магазины", Width = 200, Height = 48 };
            var btnOrders = new Button { Text = "Заказы", Width = 200, Height = 48 };
            var btnReport = new Button { Text = "Отчет", Width = 200, Height = 48 };


            foreach (var b in new[] { btnStores, btnOrders, btnReport })
            {
                b.FlatStyle = FlatStyle.Flat;
                b.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
                b.ForeColor = System.Drawing.Color.White;
                b.Font = new System.Drawing.Font(Font.FontFamily, 12F, FontStyle.Bold);
                b.Margin = new Padding(8);
                b.Dock = DockStyle.Fill; // fill available width so text is not clipped
            }

            btnStores.Click += (s, e) => new StoresForm().ShowDialog();
            btnOrders.Click += (s, e) => new OrdersForm().ShowDialog();
            btnReport.Click += (s, e) => new ReportForm().ShowDialog();

            var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(20) };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
            table.Controls.Add(btnStores, 0, 0);
            table.Controls.Add(btnOrders, 0, 1);
            table.Controls.Add(btnReport, 0, 2);

            // buttons will fill their rows
            table.SetColumnSpan(btnStores, 1);
            btnStores.Anchor = AnchorStyles.None;
            btnOrders.Anchor = AnchorStyles.None;
            btnReport.Anchor = AnchorStyles.None;

            Controls.Add(table);
        }

        private void InitializeComponent()
        {

        }
    }
}
