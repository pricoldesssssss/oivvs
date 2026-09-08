using System;
using System.Windows.Forms;

namespace lab1
{
    public partial class WeightInputDialog : Form
    {
        public int Weight { get; private set; }

        public WeightInputDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Введите вес ребра";
            this.Size = new System.Drawing.Size(250, 120);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblWeight = new Label
            {
                Text = "Вес (1-1000):",
                Location = new System.Drawing.Point(15, 20),
                AutoSize = true,
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            var txtWeight = new NumericUpDown
            {
                Location = new System.Drawing.Point(120, 18),
                Width = 80,
                Minimum = 1,
                Maximum = 1000,
                Value = 1,
                BackColor = System.Drawing.Color.FromArgb(255, 240, 245),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            var btnOK = new Button
            {
                Text = "OK",
                Location = new System.Drawing.Point(50, 55),
                Width = 60,
                DialogResult = DialogResult.OK,
                BackColor = System.Drawing.Color.FromArgb(255, 200, 220),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };
            btnOK.Click += (s, e) => { Weight = (int)txtWeight.Value; };

            var btnCancel = new Button
            {
                Text = "Отмена",
                Location = new System.Drawing.Point(130, 55),
                Width = 60,
                DialogResult = DialogResult.Cancel,
                BackColor = System.Drawing.Color.FromArgb(255, 220, 230),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            this.Controls.AddRange(new Control[] { lblWeight, txtWeight, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.BackColor = System.Drawing.Color.FromArgb(255, 248, 250);
        }
    }
}