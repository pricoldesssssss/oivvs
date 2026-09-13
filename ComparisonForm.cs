using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace lab1
{
    public class ComparisonForm : Form
    {
        public ComparisonForm(List<PathResult> dijkstraPaths, List<PathResult> floydPaths,
                              double dijkstraTime, double floydTime, int vertexCount, int edgeCount)
        {
            InitializeComponent(dijkstraPaths, floydPaths, dijkstraTime, floydTime, vertexCount, edgeCount);
        }

        private void InitializeComponent(List<PathResult> dijkstraPaths, List<PathResult> floydPaths,
                                          double dijkstraTime, double floydTime, int vertexCount, int edgeCount)
        {
            this.Text = "Сравнение алгоритмов Дейкстры и Флойда";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 248, 255);
            this.MinimumSize = new Size(800, 500);

            // ========== ЗАГОЛОВОК ==========
            var lblHeader = new Label
            {
                Text = $"СРАВНЕНИЕ АЛГОРИТМОВ  |  Вершин: {vertexCount}  |  Рёбер: {edgeCount}",
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 80, 180),
                BackColor = Color.FromArgb(200, 225, 245)
            };

            // ========== НИЖНЯЯ ПАНЕЛЬ С КНОПКОЙ ==========
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(200, 225, 245)
            };

            var btnOK = new Button
            {
                Text = "OK",
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(70, 150, 220),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold),
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.None
            };
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(0, 80, 180);
            bottomPanel.Controls.Add(btnOK);

            // Центрирование кнопки
            Action centerButton = () =>
            {
                btnOK.Location = new Point(
                    (bottomPanel.Width - btnOK.Width) / 2,
                    (bottomPanel.Height - btnOK.Height) / 2
                );
            };
            bottomPanel.Resize += (s, e) => centerButton();
            centerButton();

            // ========== ЦЕНТРАЛЬНЫЙ КОНТЕЙНЕР С ДВУМЯ КОЛОНКАМИ ==========
            var columnsContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor = Color.FromArgb(200, 225, 245),
                SplitterWidth = 6,
                IsSplitterFixed = true  // запрет на перемещение разделителя
            };

            // Делаем колонки одинаковой ширины
            columnsContainer.HandleCreated += (s, e) =>
            {
                try
                {
                    columnsContainer.SplitterDistance = (columnsContainer.Width - columnsContainer.SplitterWidth) / 2;
                }
                catch { }
            };

            columnsContainer.SizeChanged += (s, e) =>
            {
                try
                {
                    columnsContainer.SplitterDistance = (columnsContainer.Width - columnsContainer.SplitterWidth) / 2;
                }
                catch { }
            };

            // ========== ЛЕВАЯ КОЛОНКА (ДЕЙКСТРА) ==========
            var leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 248, 255)
            };

            var lblDijkstraHeader = new Label
            {
                Text = $"АЛГОРИТМ ДЕЙКСТРЫ × {vertexCount}",
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(70, 150, 220)
            };

            var lblDijkstraTime = new Label
            {
                Text = $"⏱ Время выполнения: {dijkstraTime:F3} мс",
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 80, 180),
                BackColor = Color.FromArgb(220, 235, 250)
            };

            var txtDijkstra = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 252, 255),
                ForeColor = Color.FromArgb(0, 80, 180),
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = false
            };

            // Заполняем текстом
            txtDijkstra.AppendText($"Найдено путей: {dijkstraPaths.Count}\r\n");
            txtDijkstra.AppendText(new string('─', 60) + "\r\n\r\n");

            foreach (var item in dijkstraPaths)
            {
                if (item.Path != null && item.Path.Count > 0)
                {
                    string pathStr = string.Join(" → ", item.Path.Select(v => $"v{v}"));
                    txtDijkstra.AppendText($"v{item.From} → v{item.To}: {item.Distance}\r\n");
                    txtDijkstra.AppendText($"   путь: {pathStr}\r\n\r\n");
                }
                else
                {
                    txtDijkstra.AppendText($"v{item.From} → v{item.To}: путь не существует\r\n\r\n");
                }
            }

            leftPanel.Controls.Add(txtDijkstra);
            leftPanel.Controls.Add(lblDijkstraTime);
            leftPanel.Controls.Add(lblDijkstraHeader);

            // ========== ПРАВАЯ КОЛОНКА (ФЛОЙД) ==========
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 248, 255)
            };

            var lblFloydHeader = new Label
            {
                Text = "АЛГОРИТМ ФЛОЙДА",
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 100, 180)
            };

            var lblFloydTime = new Label
            {
                Text = $"⏱ Время выполнения: {floydTime:F3} мс",
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 80, 180),
                BackColor = Color.FromArgb(220, 235, 250)
            };

            var txtFloyd = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 252, 255),
                ForeColor = Color.FromArgb(0, 80, 180),
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = false
            };

            // Заполняем текстом
            txtFloyd.AppendText($"Найдено путей: {floydPaths.Count}\r\n");
            txtFloyd.AppendText(new string('─', 60) + "\r\n\r\n");

            foreach (var item in floydPaths)
            {
                if (item.Path != null && item.Path.Count > 0)
                {
                    string pathStr = string.Join(" → ", item.Path.Select(v => $"v{v}"));
                    txtFloyd.AppendText($"v{item.From} → v{item.To}: {item.Distance}\r\n");
                    txtFloyd.AppendText($"   путь: {pathStr}\r\n\r\n");
                }
                else
                {
                    txtFloyd.AppendText($"v{item.From} → v{item.To}: путь не существует\r\n\r\n");
                }
            }

            rightPanel.Controls.Add(txtFloyd);
            rightPanel.Controls.Add(lblFloydTime);
            rightPanel.Controls.Add(lblFloydHeader);

            // Добавляем колонки в SplitContainer
            columnsContainer.Panel1.Controls.Add(leftPanel);
            columnsContainer.Panel2.Controls.Add(rightPanel);

            // Собираем всё вместе
            this.Controls.Add(columnsContainer);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(lblHeader);

            this.AcceptButton = btnOK;
            this.CancelButton = btnOK;
        }
    }
}