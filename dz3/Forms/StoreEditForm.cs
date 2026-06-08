using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Dz3.Forms
{
    /// <summary>
    /// Диалог добавления/редактирования магазина.
    /// </summary>
    public class StoreEditForm : Form
    {
        private TextBox txtName;
        private Button btnOk;
        private Button btnCancel;

        /// <summary>
        /// Название магазина для редактирования/создания.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StoreName
        {
            get => txtName.Text;
            set => txtName.Text = value;
        }

        public StoreEditForm()
        {
            Text = "Магазин";
            Width = 480;
            Height = 200;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            Padding = new Padding(12);

            // Table layout for label and textbox to make resize-friendly layout
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 1,
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var lbl = new Label { Text = "Название:", Anchor = AnchorStyles.Left | AnchorStyles.Top, AutoSize = true };
            txtName = new TextBox { Dock = DockStyle.Fill }; 

            table.Controls.Add(lbl, 0, 0);
            table.Controls.Add(txtName, 1, 0);

            // Flow panel for buttons anchored to bottom-right
            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 50,
                Padding = new Padding(0, 8, 0, 0)
            };

            btnOk = new Button
            {
                Text = "OK",
                Width = 100,
                Height = 36,
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(Font, FontStyle.Bold),
                Margin = new Padding(8, 0, 0, 0)
            };
            btnOk.FlatAppearance.BorderSize = 0;

            btnCancel = new Button
            {
                Text = "Отмена",
                Width = 100,
                Height = 36,
                DialogResult = DialogResult.Cancel,
                BackColor = SystemColors.ControlDark,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(Font, FontStyle.Bold),
                Margin = new Padding(8, 0, 0, 0)
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            buttons.Controls.Add(btnOk);
            buttons.Controls.Add(btnCancel);

            Controls.Add(buttons);
            Controls.Add(table);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}
