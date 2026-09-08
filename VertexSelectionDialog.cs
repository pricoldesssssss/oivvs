using System;
using System.Windows.Forms;

namespace lab1
{
    public partial class VertexSelectionDialog : Form
    {
        public int SelectedVertex { get; private set; }

        public VertexSelectionDialog(int maxVertices)
        {
            InitializeComponent(maxVertices);
        }

        private void InitializeComponent(int maxVertices)
        {
            this.Text = "Выберите вершину";
            this.Size = new System.Drawing.Size(300, 130);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(255, 248, 250);

            var lblVertex = new Label
            {
                Text = "Выберите вершину для удаления:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true,
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            var numVertex = new NumericUpDown
            {
                Location = new System.Drawing.Point(200, 18),
                Width = 50,
                Minimum = 0,
                Maximum = maxVertices - 1,
                Value = 0,
                BackColor = System.Drawing.Color.FromArgb(255, 240, 245),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            var btnOK = new Button
            {
                Text = "OK",
                Location = new System.Drawing.Point(70, 55),
                Width = 60,
                DialogResult = DialogResult.OK,
                BackColor = System.Drawing.Color.FromArgb(255, 200, 220),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };
            btnOK.Click += (s, e) => { SelectedVertex = (int)numVertex.Value; };

            var btnCancel = new Button
            {
                Text = "Отмена",
                Location = new System.Drawing.Point(155, 55),
                Width = 60,
                DialogResult = DialogResult.Cancel,
                BackColor = System.Drawing.Color.FromArgb(255, 220, 230),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            this.Controls.AddRange(new Control[] { lblVertex, numVertex, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }
}