using System;
using System.Windows.Forms;

namespace lab1
{
    public static class AddEdgeDialog
    {
        public class EdgeInfo
        {
            public int From { get; set; }
            public int To { get; set; }
        }

        public static EdgeInfo ShowDialog(int maxVertices)
        {
            var form = new Form
            {
                Text = "Добавить/Изменить ребро",
                Size = new System.Drawing.Size(300, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = System.Drawing.Color.FromArgb(255, 248, 250)
            };

            var lblFrom = new Label
            {
                Text = "От вершины:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true,
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            var numFrom = new NumericUpDown
            {
                Location = new System.Drawing.Point(120, 18),
                Width = 50,
                Minimum = 0,
                Maximum = maxVertices - 1,
                Value = 0,
                BackColor = System.Drawing.Color.FromArgb(255, 240, 245),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            var lblTo = new Label
            {
                Text = "К вершине:",
                Location = new System.Drawing.Point(20, 50),
                AutoSize = true,
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            var numTo = new NumericUpDown
            {
                Location = new System.Drawing.Point(120, 48),
                Width = 50,
                Minimum = 0,
                Maximum = maxVertices - 1,
                Value = Math.Min(1, maxVertices - 1),
                BackColor = System.Drawing.Color.FromArgb(255, 240, 245),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            var btnOK = new Button
            {
                Text = "OK",
                Location = new System.Drawing.Point(70, 80),
                Width = 60,
                DialogResult = DialogResult.OK,
                BackColor = System.Drawing.Color.FromArgb(255, 200, 220),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            var btnCancel = new Button
            {
                Text = "Отмена",
                Location = new System.Drawing.Point(150, 80),
                Width = 60,
                DialogResult = DialogResult.Cancel,
                BackColor = System.Drawing.Color.FromArgb(255, 220, 230),
                ForeColor = System.Drawing.Color.FromArgb(180, 40, 90)
            };

            form.Controls.AddRange(new Control[] { lblFrom, numFrom, lblTo, numTo, btnOK, btnCancel });
            form.AcceptButton = btnOK;
            form.CancelButton = btnCancel;

            EdgeInfo result = null;
            btnOK.Click += (s, e) =>
            {
                if (numFrom.Value == numTo.Value)
                {
                    MessageBox.Show("Нельзя добавить ребро из вершины в саму себя!",
                                  "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                result = new EdgeInfo
                {
                    From = (int)numFrom.Value,
                    To = (int)numTo.Value
                };
                form.Close();
            };

            form.ShowDialog();
            return result;
        }
    }
}