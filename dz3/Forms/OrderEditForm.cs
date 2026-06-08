using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using Dz3.Data;

namespace Dz3.Forms
{
    /// <summary>
    /// Диалог добавления/редактирования заказа.
    /// </summary>
    public class OrderEditForm : Form
    {
        private TextBox txtName;
        private ComboBox cmbStores;
        private NumericUpDown nudAmount;
        private Button btnOk;
        private Button btnCancel;

        /// <summary>
        /// Название заказа.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string OrderName
        {
            get => txtName.Text;
            set => txtName.Text = value;
        }

        /// <summary>
        /// Id выбранного магазина.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int StoreId
        {
            get => (int)(cmbStores.SelectedValue ?? 0);
            set => cmbStores.SelectedValue = value;
        }

        /// <summary>
        /// Сумма заказа.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal Amount
        {
            get => nudAmount.Value;
            set => nudAmount.Value = value;
        }

        public OrderEditForm()
        {
            Text = "Заказ";
            Width = 520;
            Height = 260;
            StartPosition = FormStartPosition.CenterParent;

            Padding = new Padding(12);

            var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var lblName = new Label { Text = "Название:", Anchor = AnchorStyles.Left | AnchorStyles.Top, AutoSize = true };
            txtName = new TextBox { Dock = DockStyle.Fill };

            var lblStore = new Label { Text = "Магазин:", Anchor = AnchorStyles.Left | AnchorStyles.Top, AutoSize = true };
            cmbStores = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblAmount = new Label { Text = "Сумма (₽):", Anchor = AnchorStyles.Left | AnchorStyles.Top, AutoSize = true };
            nudAmount = new NumericUpDown { Dock = DockStyle.Left, Width = 180, DecimalPlaces = 2, Maximum = 100000000, Minimum = 0 };

            table.Controls.Add(lblName, 0, 0);
            table.Controls.Add(txtName, 1, 0);
            table.Controls.Add(lblStore, 0, 1);
            table.Controls.Add(cmbStores, 1, 1);
            table.Controls.Add(lblAmount, 0, 2);
            table.Controls.Add(nudAmount, 1, 2);

            // Buttons panel
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 56, Padding = new Padding(0,8,12,8) };
            btnOk = new Button { Text = "OK", Width = 120, Height = 36, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Отмена", Width = 120, Height = 36, DialogResult = DialogResult.Cancel };
            foreach (var b in new[] { btnOk, btnCancel })
            {
                b.FlatStyle = FlatStyle.Flat;
                b.ForeColor = Color.White;
                b.Font = new Font(Font, FontStyle.Bold);
                b.Margin = new Padding(8, 0, 0, 0);
            }
            btnOk.BackColor = Color.FromArgb(0, 120, 215);
            btnCancel.BackColor = SystemColors.ControlDark;

            buttons.Controls.Add(btnOk);
            buttons.Controls.Add(btnCancel);

            Controls.Add(buttons);
            Controls.Add(table);

            LoadStores();

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void LoadStores()
        {
            try
            {
                using var context = new AppDbContext();
                var list = context.Stores.OrderBy(s => s.Name).Select(s => new { s.Id, s.Name }).ToList();
                cmbStores.DataSource = list;
                cmbStores.ValueMember = "Id";
                cmbStores.DisplayMember = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
