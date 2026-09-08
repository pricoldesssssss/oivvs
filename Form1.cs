using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form1 : Form
    {
        private Graph graph;
        private GraphVisualizer visualizer;
        private Panel matrixPanel;
        private Panel graphPanel;
        private Label lblResult;
        private NumericUpDown txtVertexCount;
        private NumericUpDown txtStart;
        private NumericUpDown txtEnd;
        private Label lblMatrixTitle;

        public Form1()
        {
            InitializeComponent1();
            // Создаем граф с примером из лабораторной работы
            CreateDefaultGraph();
            // Автоматически находим путь при загрузке
            this.Shown += Form1_Shown;
        }

        private void InitializeComponent1()
        {
            this.Text = "Алгоритм Дейкстры - Поиск кратчайшего пути";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 240, 245);

            // Главный контейнер разделения
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 500,
                BackColor = Color.FromArgb(255, 230, 240)
            };

            // Левая панель - только матрица
            var leftPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5),
                BackColor = Color.FromArgb(255, 240, 245)
            };

            // Заголовок матрицы
            lblMatrixTitle = new Label
            {
                Text = "МАТРИЦА ВЕСОВ ГРАФА",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 40, 90),
                BackColor = Color.FromArgb(255, 220, 235)
            };

            // Панель матрицы
            matrixPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 248, 250),
                Padding = new Padding(10)
            };

            // Правая панель - граф и кнопки
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5),
                BackColor = Color.FromArgb(255, 240, 245)
            };

            // Заголовок графа
            var lblGraphTitle = new Label
            {
                Text = "ВИЗУАЛИЗАЦИЯ ГРАФА",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 40, 90),
                BackColor = Color.FromArgb(255, 220, 235)
            };

            // Панель графа
            graphPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(150, 248, 250),
                BorderStyle = BorderStyle.FixedSingle
            };
            graphPanel.Paint += GraphPanel_Paint;
            graphPanel.MouseDown += GraphPanel_MouseDown;
            graphPanel.MouseMove += GraphPanel_MouseMove;
            graphPanel.MouseUp += GraphPanel_MouseUp;
            graphPanel.MouseClick += GraphPanel_MouseClick;

            // Панель кнопок управления (под графом)
            var controlsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 180,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(255, 220, 235)
            };

            // Создаем элементы управления на правой панели
            CreateControls(controlsPanel);

            // Собираем правую панель
            rightPanel.Controls.Add(graphPanel);
            rightPanel.Controls.Add(lblGraphTitle);
            rightPanel.Controls.Add(controlsPanel);

            // Собираем левую панель
            leftPanel.Controls.Add(matrixPanel);
            leftPanel.Controls.Add(lblMatrixTitle);

            // Добавляем в контейнер разделения
            splitContainer.Panel1.Controls.Add(leftPanel);
            splitContainer.Panel2.Controls.Add(rightPanel);

            this.Controls.Add(splitContainer);
        }

        private void CreateDefaultGraph()
        {
            // Создаем граф с 9 вершинами (как в примере из лабораторной работы)
            graph = new Graph(9);

            // Задаем связи как в примере из лабораторной работы (рисунок 1)
            int[,] defaultEdges = new int[,]
            {
                {0, 1, 10},  // x1→x2
                {0, 6, 3},   // x1→x7
                {0, 7, 6},   // x1→x8
                {0, 8, 12},  // x1→x9
                {1, 2, 18},  // x2→x3
                {1, 6, 2},   // x2→x7
                {1, 8, 12},  // x2→x9
                {2, 1, 18},  // x3→x2
                {2, 3, 5},   // x3→x4
                {2, 5, 6},   // x3→x6
                {3, 2, 5},   // x4→x3
                {3, 5, 10},  // x4→x6
                {3, 6, 4},   // x4→x7
                {4, 5, 6},   // x5→x6
                {5, 3, 10},  // x6→x4
                {5, 4, 6},   // x6→x5
                {6, 1, 2},   // x7→x2
                {6, 3, 4},   // x7→x4
                {6, 5, 17},  // x7→x6
                {6, 8, 12},  // x7→x9
                {7, 3, 12},  // x8→x4
                {7, 5, 8},   // x8→x6
                {7, 8, 2},   // x8→x9
                {8, 0, 12},  // x9→x1
                {8, 1, 12},  // x9→x2
                {8, 6, 12},  // x9→x7
                {8, 7, 2}    // x9→x8
            };

            for (int i = 0; i < defaultEdges.GetLength(0); i++)
            {
                int from = defaultEdges[i, 0];
                int to = defaultEdges[i, 1];
                int weight = defaultEdges[i, 2];
                graph.SetWeight(from, to, weight);
            }

            visualizer = new GraphVisualizer(graph);
            InitializeMatrixPanel();
            UpdateUI();

            txtStart.Value = 0;
            txtEnd.Value = 4;

            lblResult.Text = "Граф создан с примером из лабораторной работы (9 вершин, 27 рёбер)";
            lblResult.ForeColor = Color.FromArgb(200, 50, 100);
        }

        private void CreateControls(Panel parent)
        {
            int y = 5;
            Color pinkText = Color.FromArgb(180, 40, 90);
            Color pinkButton = Color.FromArgb(255, 200, 220);

            // Строка 1: Управление созданием графа
            var lblVertexCount = new Label
            {
                Text = "Вершины:",
                Location = new Point(10, y),
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = pinkText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtVertexCount = new NumericUpDown
            {
                Location = new Point(115, y),
                Width = 60,
                Minimum = 1,
                Maximum = 10,
                Value = 9,
                BackColor = Color.FromArgb(255, 240, 245),
                ForeColor = pinkText
            };

            var btnCreateGraph = new Button
            {
                Text = "Создать граф",
                Location = new Point(185, y),
                Size = new Size(100, 25),
                BackColor = pinkButton,
                ForeColor = pinkText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            btnCreateGraph.FlatAppearance.BorderColor = Color.FromArgb(200, 150, 180);
            btnCreateGraph.Click += (s, e) =>
            {
                int n = (int)txtVertexCount.Value;
                graph = new Graph(n);
                visualizer = new GraphVisualizer(graph);
                InitializeMatrixPanel();
                UpdateUI();
                graphPanel.Invalidate();
                lblResult.Text = $"Граф создан с {n} вершинами";
                lblResult.ForeColor = Color.FromArgb(200, 50, 100);
            };

            var btnLoadExample = new Button
            {
                Text = "Загрузить пример",
                Location = new Point(295, y),
                Size = new Size(120, 25),
                BackColor = Color.FromArgb(255, 180, 200),
                ForeColor = pinkText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            btnLoadExample.FlatAppearance.BorderColor = Color.FromArgb(200, 150, 180);
            btnLoadExample.Click += (s, e) =>
            {
                CreateDefaultGraph();
                graphPanel.Invalidate();
                FindShortestPath(0, 4);
            };

            y += 35;

            // Строка 2: Выбор начальной и конечной вершины
            var lblStart = new Label
            {
                Text = "Начало:",
                Location = new Point(10, y),
                Size = new Size(50, 25),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = pinkText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtStart = new NumericUpDown
            {
                Location = new Point(65, y),
                Width = 50,
                Minimum = 0,
                Maximum = 9,
                Value = 0,
                BackColor = Color.FromArgb(255, 240, 245),
                ForeColor = pinkText
            };

            var lblEnd = new Label
            {
                Text = "Конец:",
                Location = new Point(125, y),
                Size = new Size(45, 25),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = pinkText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtEnd = new NumericUpDown
            {
                Location = new Point(175, y),
                Width = 50,
                Minimum = 0,
                Maximum = 9,
                Value = 4,
                BackColor = Color.FromArgb(255, 240, 245),
                ForeColor = pinkText
            };

            var btnFindPath = new Button
            {
                Text = "Найти путь",
                Location = new Point(235, y),
                Size = new Size(100, 25),
                BackColor = Color.FromArgb(255, 150, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnFindPath.FlatAppearance.BorderColor = Color.FromArgb(200, 100, 140);
            btnFindPath.Click += (s, e) =>
            {
                int start = (int)txtStart.Value;
                int end = (int)txtEnd.Value;
                FindShortestPath(start, end);
            };

            var btnClearPath = new Button
            {
                Text = "Очистить путь",
                Location = new Point(345, y),
                Size = new Size(110, 25),
                BackColor = Color.FromArgb(255, 220, 230),
                ForeColor = pinkText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            btnClearPath.FlatAppearance.BorderColor = Color.FromArgb(200, 150, 180);
            btnClearPath.Click += (s, e) =>
            {
                visualizer.ClearPath();
                graphPanel.Invalidate();
                lblResult.Text = "🌸 Путь очищен";
                lblResult.ForeColor = Color.FromArgb(200, 50, 100);
            };

            y += 35;

            // Строка 3: Инструкции
            var lblInstructions = new Label
            {
                Text = "Левый клик = Начало" +
                "  Ctrl+Клик = Конец " +
                " Правый клик = Меню",
                Location = new Point(10, y),
                Size = new Size(500, 25),
                ForeColor = Color.FromArgb(180, 120, 150),
                Font = new Font("Arial", 9)
            };

            y += 30;

            // Строка 4: Результат
            lblResult = new Label
            {
                Text = "Загрузите пример или создайте свой граф",
                Location = new Point(10, y),
                Size = new Size(700, 40),
                ForeColor = Color.FromArgb(200, 50, 100),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            parent.Controls.AddRange(new Control[] {
                lblVertexCount, txtVertexCount, btnCreateGraph, btnLoadExample,
                lblStart, txtStart, lblEnd, txtEnd, btnFindPath, btnClearPath,
                lblInstructions, lblResult
            });
        }

        private void InitializeMatrixPanel()
        {
            matrixPanel.Controls.Clear();

            if (graph == null || graph.VerticesCount == 0)
            {
                var lblEmpty = new Label
                {
                    Text = "Сначала создайте граф",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    Font = new Font("Arial", 12),
                    ForeColor = Color.FromArgb(200, 50, 100)
                };
                matrixPanel.Controls.Add(lblEmpty);

                var lblInfo1 = new Label
                {
                    Text = "В матрице: строки = откуда, столбцы = куда",
                    Location = new Point(20, 50),
                    AutoSize = true,
                    Font = new Font("Arial", 10),
                    ForeColor = Color.FromArgb(150, 100, 130)
                };
                matrixPanel.Controls.Add(lblInfo1);
                return;
            }

            int n = graph.VerticesCount;
            int cellSize = 45;
            int padding = 5;
            int startX = 30;
            int startY = 50;

            var headerFont = new Font("Arial", 10, FontStyle.Bold);
            Color pinkHeader = Color.FromArgb(180, 40, 90);

            // Пояснение о матрице
            var lblInfo = new Label
            {
                Text = "Строка -> Столбец (вес ребра)",
                Location = new Point(startX, 10),
                Size = new Size(300, 30),
                Font = new Font("Arial", 9),
                ForeColor = Color.FromArgb(150, 100, 130)
            };
            matrixPanel.Controls.Add(lblInfo);

            // Верхний левый угол
            matrixPanel.Controls.Add(new Label
            {
                Text = "/",
                Location = new Point(startX, startY),
                Size = new Size(cellSize, cellSize),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = headerFont,
                ForeColor = pinkHeader
            });

            // Заголовки столбцов
            for (int j = 0; j < n; j++)
            {
                matrixPanel.Controls.Add(new Label
                {
                    Text = $"v{j}",
                    Location = new Point(startX + (j + 1) * (cellSize + padding), startY),
                    Size = new Size(cellSize, cellSize),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = headerFont,
                    ForeColor = pinkHeader,
                    BackColor = Color.FromArgb(255, 230, 240)
                });
            }

            // Заголовки строк и поля ввода
            for (int i = 0; i < n; i++)
            {
                matrixPanel.Controls.Add(new Label
                {
                    Text = $"v{i}",
                    Location = new Point(startX, startY + (i + 1) * (cellSize + padding)),
                    Size = new Size(cellSize, cellSize),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = headerFont,
                    ForeColor = pinkHeader,
                    BackColor = Color.FromArgb(255, 230, 240)
                });

                for (int j = 0; j < n; j++)
                {
                    int weight = graph.GetWeight(i, j);
                    var textBox = new TextBox
                    {
                        Location = new Point(startX + (j + 1) * (cellSize + padding),
                                            startY + (i + 1) * (cellSize + padding)),
                        Size = new Size(cellSize, cellSize),
                        Text = weight > 0 ? weight.ToString() : "",
                        TextAlign = HorizontalAlignment.Center,
                        Tag = new Point(i, j),
                        MaxLength = 5,
                        BackColor = (i == j) ? Color.FromArgb(255, 230, 235) : Color.FromArgb(255, 248, 250),
                        ReadOnly = (i == j),
                        ForeColor = Color.FromArgb(180, 40, 90),
                        Font = new Font("Arial", 9, FontStyle.Bold),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    if (weight > 0 && i != j)
                    {
                        textBox.BackColor = Color.FromArgb(255, 200, 220);
                        textBox.Text = weight.ToString();
                    }

                    if (weight == 0 && i != j)
                    {
                        textBox.ForeColor = Color.FromArgb(180, 180, 180);
                        textBox.Font = new Font("Arial", 8);
                    }

                    textBox.TextChanged += MatrixCell_TextChanged;
                    matrixPanel.Controls.Add(textBox);
                }
            }

            

        }

        private void MatrixCell_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || graph == null) return;

            var point = (Point)textBox.Tag;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                graph.SetWeight(point.X, point.Y, 0);
                textBox.BackColor = (point.X == point.Y) ? Color.FromArgb(255, 230, 235) : Color.FromArgb(255, 248, 250);
                textBox.ForeColor = Color.FromArgb(180, 180, 180);
                graphPanel.Invalidate();
                return;
            }

            if (int.TryParse(textBox.Text, out int weight))
            {
                if (weight < 0)
                {
                    MessageBox.Show("Отрицательные веса запрещены! Будет использовано 0.",
                                  "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    weight = 0;
                    textBox.Text = "";
                    return;
                }

                if (weight > 1000)
                {
                    MessageBox.Show("Максимальный вес - 1000!",
                                  "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    weight = 1000;
                    textBox.Text = "1000";
                }

                graph.SetWeight(point.X, point.Y, weight);

                if (weight > 0 && point.X != point.Y)
                {
                    textBox.BackColor = Color.FromArgb(255, 200, 220);
                    textBox.ForeColor = Color.FromArgb(180, 40, 90);
                    textBox.Font = new Font("Arial", 9, FontStyle.Bold);
                }
                else if (point.X == point.Y)
                {
                    textBox.BackColor = Color.FromArgb(255, 230, 235);
                    textBox.ForeColor = Color.FromArgb(180, 40, 90);
                }
                else
                {
                    textBox.BackColor = Color.FromArgb(255, 248, 250);
                    textBox.ForeColor = Color.FromArgb(180, 180, 180);
                    textBox.Font = new Font("Arial", 8);
                }

                graphPanel.Invalidate();
            }
            else
            {
                textBox.Text = "";
                graph.SetWeight(point.X, point.Y, 0);
                textBox.BackColor = (point.X == point.Y) ? Color.FromArgb(255, 230, 235) : Color.FromArgb(255, 248, 250);
                textBox.ForeColor = Color.FromArgb(180, 180, 180);
                graphPanel.Invalidate();
            }
        }

        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            if (graph == null || graph.VerticesCount == 0)
            {
                e.Graphics.Clear(Color.FromArgb(255, 248, 250));
                using (var font = new Font("Arial", 14, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(200, 120, 160)))
                {
                    e.Graphics.DrawString("Создайте граф для начала работы", font, brush,
                                new PointF(180, 200));
                }
                return;
            }
            visualizer.Draw(e.Graphics, e.ClipRectangle);
        }

        private void GraphPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (graph == null) return;

            if (e.Button == MouseButtons.Left)
            {
                int vertex = visualizer.GetVertexAt(e.Location);
                if (vertex != -1)
                {
                    // ДОБАВЬТЕ ЭТИ СТРОКИ:
                    isDragging = true;
                    draggedVertex = vertex;
                    dragStartPoint = e.Location;

                    visualizer.StartDragging(vertex);
                }
            }
        }

        private void GraphPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (graph == null) return;

            // ДОБАВЬТЕ ЭТИ СТРОКИ:
            isDragging = false;
            draggedVertex = -1;

            visualizer.StopDragging();
        }
        private bool isDragging = false;
        private int draggedVertex = -1;
        private Point dragStartPoint;
        private void GraphPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (graph == null) return;

            if (!isDragging) return; 

            visualizer.DragVertex(e.Location);
            graphPanel.Invalidate();
        }


        private void GraphPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (graph == null || graph.VerticesCount == 0) return;


            if (e.Button == MouseButtons.Right)
            {
                ShowContextMenu(e.Location);
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                int vertex = visualizer.GetVertexAt(e.Location);
                if (vertex != -1)
                {
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        visualizer.SetEndVertex(vertex);
                        txtEnd.Value = vertex;
                        lblResult.Text = $"Конечная вершина: v{vertex}";
                        lblResult.ForeColor = Color.FromArgb(200, 50, 100);
                    }
                    else
                    {
                        visualizer.SetStartVertex(vertex);
                        txtStart.Value = vertex;
                        lblResult.Text = $"Начальная вершина: v{vertex}";
                        lblResult.ForeColor = Color.FromArgb(200, 50, 100);
                    }
                    graphPanel.Invalidate();
                }
            }
        }

        private void ShowContextMenu(Point location)
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.BackColor = Color.FromArgb(255, 240, 245);
            contextMenu.ForeColor = Color.FromArgb(180, 40, 90);

            var addVertexItem = new ToolStripMenuItem("Добавить вершину");
            addVertexItem.ForeColor = Color.FromArgb(180, 40, 90);
            addVertexItem.Click += (s, e) =>
            {
                if (graph.VerticesCount < 10)
                {
                    graph.AddVertex();
                    visualizer = new GraphVisualizer(graph);
                    InitializeMatrixPanel();
                    UpdateUI();
                    graphPanel.Invalidate();
                    lblResult.Text = $"Вершина добавлена. Всего: {graph.VerticesCount}";
                    lblResult.ForeColor = Color.FromArgb(200, 50, 100);
                }
                else
                {
                    MessageBox.Show("Максимум 10 вершин!", "Лимит достигнут",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            var addEdgeItem = new ToolStripMenuItem("Добавить ребро");
            addEdgeItem.ForeColor = Color.FromArgb(180, 40, 90);
            addEdgeItem.Click += (s, e) =>
            {
                var fromTo = AddEdgeDialog.ShowDialog(graph.VerticesCount);
                if (fromTo != null)
                {
                    var weightDialog = new WeightInputDialog();
                    if (weightDialog.ShowDialog() == DialogResult.OK)
                    {
                        graph.SetWeight(fromTo.From, fromTo.To, weightDialog.Weight);
                        InitializeMatrixPanel();
                        graphPanel.Invalidate();
                        lblResult.Text = $"Ребро добавлено: v{fromTo.From} → v{fromTo.To} (вес: {weightDialog.Weight})";
                        lblResult.ForeColor = Color.FromArgb(200, 50, 100);
                    }
                }
            };

            var deleteVertexItem = new ToolStripMenuItem("Удалить вершину");
            deleteVertexItem.ForeColor = Color.FromArgb(180, 40, 90);
            deleteVertexItem.Click += (s, e) =>
            {
                var vertexDialog = new VertexSelectionDialog(graph.VerticesCount);
                if (vertexDialog.ShowDialog() == DialogResult.OK)
                {
                    int vertex = vertexDialog.SelectedVertex;
                    graph.RemoveVertex(vertex);
                    visualizer = new GraphVisualizer(graph);
                    InitializeMatrixPanel();
                    UpdateUI();
                    graphPanel.Invalidate();
                    lblResult.Text = $"Вершина v{vertex} удалена";
                    lblResult.ForeColor = Color.FromArgb(200, 50, 100);
                }
            };

            var deleteEdgeItem = new ToolStripMenuItem("Удалить ребро");
            deleteEdgeItem.ForeColor = Color.FromArgb(180, 40, 90);
            deleteEdgeItem.Click += (s, e) =>
            {
                var fromTo = AddEdgeDialog.ShowDialog(graph.VerticesCount);
                if (fromTo != null)
                {
                    graph.SetWeight(fromTo.From, fromTo.To, 0);
                    InitializeMatrixPanel();
                    graphPanel.Invalidate();
                    lblResult.Text = $"Ребро v{fromTo.From} → v{fromTo.To} удалено";
                    lblResult.ForeColor = Color.FromArgb(200, 50, 100);
                }
            };

            var changeWeightItem = new ToolStripMenuItem("Изменить вес");
            changeWeightItem.ForeColor = Color.FromArgb(180, 40, 90);
            changeWeightItem.Click += (s, e) =>
            {
                var fromTo = AddEdgeDialog.ShowDialog(graph.VerticesCount);
                if (fromTo != null)
                {
                    var weightDialog = new WeightInputDialog();
                    if (weightDialog.ShowDialog() == DialogResult.OK)
                    {
                        graph.SetWeight(fromTo.From, fromTo.To, weightDialog.Weight);
                        InitializeMatrixPanel();
                        graphPanel.Invalidate();
                        lblResult.Text = $"Вес изменён: v{fromTo.From} → v{fromTo.To} = {weightDialog.Weight}";
                        lblResult.ForeColor = Color.FromArgb(200, 50, 100);
                    }
                }
            };

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                addVertexItem,
                new ToolStripSeparator(),
                addEdgeItem,
                deleteEdgeItem,
                changeWeightItem,
                new ToolStripSeparator(),
                deleteVertexItem
            });

            contextMenu.Show(graphPanel, location);
        }

        private void FindShortestPath(int start, int end)
        {
            if (graph == null || graph.VerticesCount == 0)
            {
                lblResult.Text = "Сначала создайте граф!";
                lblResult.ForeColor = Color.Red;
                return;
            }

            if (start >= graph.VerticesCount || end >= graph.VerticesCount)
            {
                lblResult.Text = $"Неверные вершины! Макс: {graph.VerticesCount - 1}";
                lblResult.ForeColor = Color.Red;
                return;
            }

            if (graph.HasNegativeCycle())
            {
                lblResult.Text = "Граф содержит отрицательные циклы!";
                lblResult.ForeColor = Color.Red;
                MessageBox.Show("Граф содержит отрицательные циклы!\n\nАлгоритм Дейкстры не может работать с отрицательными циклами.",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = graph.Dijkstra(start, end);

            if (result == null)
            {
                lblResult.Text = $"Путь от v{start} до v{end} не существует!";
                lblResult.ForeColor = Color.Red;
                visualizer.ClearPath();
                graphPanel.Invalidate();
            }
            else
            {
                string pathStr = string.Join(" → ", result.Path.Select(v => $"v{v}"));
                lblResult.Text = $"Путь v{start}→v{end}: Расстояние = {result.Distance} | {pathStr}";
                lblResult.ForeColor = Color.FromArgb(200, 50, 100);
                visualizer.SetPath(result.Path, result.Distance);
                visualizer.SetStartVertex(start);
                visualizer.SetEndVertex(end);
                graphPanel.Invalidate();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            FindShortestPath(0, 4);
        }

        private void UpdateUI()
        {
            if (graph != null)
            {
                txtStart.Maximum = Math.Max(0, graph.VerticesCount - 1);
                txtEnd.Maximum = Math.Max(0, graph.VerticesCount - 1);
                if (txtStart.Value >= graph.VerticesCount) txtStart.Value = graph.VerticesCount - 1;
                if (txtEnd.Value >= graph.VerticesCount) txtEnd.Value = graph.VerticesCount - 1;
            }
        }
    }
}