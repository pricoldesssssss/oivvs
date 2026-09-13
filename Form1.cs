using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace lab1
{
    public partial class Form2 : Form
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

        private bool isDragging = false;

        // Разделители
        private SplitContainer splitContainerMain;
        private SplitContainer splitContainerRight;
        private SplitContainer splitContainerLeft;

        public Form2()
        {
            InitializeComponent1();
            CreateDefaultGraph();
            this.Shown += Form2_Shown;
        }

        private void InitializeComponent1()
        {
            this.Text = "Алгоритмы Дейкстры и Флойда - Все кратчайшие пути";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 248, 255);

            // ========== ГЛАВНЫЙ ВЕРТИКАЛЬНЫЙ РАЗДЕЛИТЕЛЬ ==========
            splitContainerMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 200,
                BackColor = Color.FromArgb(220, 235, 250),
                SplitterWidth = 6,
                SplitterIncrement = 10
            };

            // ========== ЛЕВАЯ ПАНЕЛЬ ==========
            splitContainerLeft = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 200,
                BackColor = Color.FromArgb(220, 235, 250),
                SplitterWidth = 6,
                SplitterIncrement = 10
            };

            // Матрица
            lblMatrixTitle = new Label
            {
                Text = "МАТРИЦА ВЕСОВ ГРАФА",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 80, 180),
                BackColor = Color.FromArgb(200, 225, 245)
            };

            matrixPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 248, 255),
                Padding = new Padding(10)
            };

            var matrixContainer = new Panel { Dock = DockStyle.Fill };
            matrixContainer.Controls.Add(matrixPanel);
            matrixContainer.Controls.Add(lblMatrixTitle);

            splitContainerLeft.Panel1.Controls.Add(matrixContainer);

            // Кнопки управления
            var controlsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(200, 225, 245),
                AutoScroll = true
            };

            CreateControls(controlsPanel);
            splitContainerLeft.Panel2.Controls.Add(controlsPanel);

            splitContainerMain.Panel1.Controls.Add(splitContainerLeft);

            // ========== ПРАВАЯ ПАНЕЛЬ ==========
            splitContainerRight = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 500,
                BackColor = Color.FromArgb(220, 235, 250),
                SplitterWidth = 6,
                SplitterIncrement = 10
            };

            // Граф
            var lblGraphTitle = new Label
            {
                Text = "ВИЗУАЛИЗАЦИЯ ГРАФА",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 80, 180),
                BackColor = Color.FromArgb(200, 225, 245)
            };

            graphPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };
            graphPanel.Paint += GraphPanel_Paint;
            graphPanel.MouseDown += GraphPanel_MouseDown;
            graphPanel.MouseMove += GraphPanel_MouseMove;
            graphPanel.MouseUp += GraphPanel_MouseUp;
            graphPanel.MouseClick += GraphPanel_MouseClick;

            var graphContainer = new Panel { Dock = DockStyle.Fill };
            graphContainer.Controls.Add(graphPanel);
            graphContainer.Controls.Add(lblGraphTitle);

            splitContainerRight.Panel1.Controls.Add(graphContainer);

            // Нижняя часть правой панели - информация/инструкции
            var infoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(220, 235, 250),
                Padding = new Padding(15)
            };

            var lblInfoTitle = new Label
            {
                Text = "ИНФОРМАЦИЯ",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 80, 180),
                BackColor = Color.FromArgb(200, 225, 245)
            };

            lblResult = new Label
            {
                Text = "Загрузите пример или создайте свой граф.\n\n" +
                       "Выберите начальную и конечную вершины и нажмите 'Найти путь'\n" +
                       "для поиска кратчайшего пути алгоритмом Дейкстры.\n\n" +
                       "Нажмите 'Сравнить Дейкстра × n и Флойд' для сравнения всех путей.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = Color.FromArgb(0, 80, 180),
                Font = new Font("Arial", 10, FontStyle.Bold),
                Padding = new Padding(10, 10, 10, 10)
            };

            infoPanel.Controls.Add(lblResult);
            infoPanel.Controls.Add(lblInfoTitle);

            splitContainerRight.Panel2.Controls.Add(infoPanel);

            splitContainerMain.Panel2.Controls.Add(splitContainerRight);

            this.Controls.Add(splitContainerMain);
        }

        private void CreateControls(Panel parent)
        {
            int y = 5;
            Color blueText = Color.FromArgb(0, 80, 180);
            Color blueButton = Color.FromArgb(200, 225, 245);

            // Строка 1: Управление графом
            var lblVertexCount = new Label
            {
                Text = "Вершин (1-20):",
                Location = new Point(10, y),
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = blueText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtVertexCount = new NumericUpDown
            {
                Location = new Point(115, y),
                Width = 60,
                Minimum = 1,
                Maximum = 20,
                Value = 4,
                BackColor = Color.FromArgb(240, 248, 255),
                ForeColor = blueText
            };

            var btnCreateGraph = new Button
            {
                Text = "Создать граф",
                Location = new Point(185, y),
                Size = new Size(100, 25),
                BackColor = blueButton,
                ForeColor = blueText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            btnCreateGraph.FlatAppearance.BorderColor = Color.FromArgb(100, 160, 210);
            btnCreateGraph.Click += (s, e) =>
            {
                int n = (int)txtVertexCount.Value;
                graph = new Graph(n);
                visualizer = new GraphVisualizer(graph);
                InitializeMatrixPanel();
                UpdateUI();
                graphPanel.Invalidate();
                lblResult.Text = $"Граф создан с {n} вершинами";
                lblResult.ForeColor = Color.FromArgb(0, 80, 180);
            };

            var btnLoadExample = new Button
            {
                Text = "Загрузить пример",
                Location = new Point(295, y),
                Size = new Size(120, 25),
                BackColor = Color.FromArgb(180, 215, 240),
                ForeColor = blueText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            btnLoadExample.FlatAppearance.BorderColor = Color.FromArgb(100, 160, 210);
            btnLoadExample.Click += (s, e) =>
            {
                CreateDefaultGraph();
                graphPanel.Invalidate();
            };

            y += 35;

            // Строка 2: Поиск пути (Дейкстра)
            var lblStart = new Label
            {
                Text = "Начало:",
                Location = new Point(10, y),
                Size = new Size(50, 25),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = blueText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtStart = new NumericUpDown
            {
                Location = new Point(65, y),
                Width = 50,
                Minimum = 0,
                Maximum = 19,
                Value = 0,
                BackColor = Color.FromArgb(240, 248, 255),
                ForeColor = blueText
            };

            var lblEnd = new Label
            {
                Text = "Конец:",
                Location = new Point(125, y),
                Size = new Size(45, 25),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = blueText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtEnd = new NumericUpDown
            {
                Location = new Point(175, y),
                Width = 50,
                Minimum = 0,
                Maximum = 19,
                Value = 3,
                BackColor = Color.FromArgb(240, 248, 255),
                ForeColor = blueText
            };

            var btnFindPath = new Button
            {
                Text = "Найти путь",
                Location = new Point(235, y),
                Size = new Size(100, 25),
                BackColor = Color.FromArgb(70, 150, 220),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnFindPath.FlatAppearance.BorderColor = Color.FromArgb(0, 80, 180);
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
                BackColor = Color.FromArgb(200, 225, 245),
                ForeColor = blueText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            btnClearPath.FlatAppearance.BorderColor = Color.FromArgb(100, 160, 210);
            btnClearPath.Click += (s, e) =>
            {
                visualizer.ClearPath();
                graphPanel.Invalidate();
                lblResult.Text = "Путь очищен";
                lblResult.ForeColor = Color.FromArgb(0, 80, 180);
            };

            y += 35;

            // Строка 3: Сравнение
            var btnCompare = new Button
            {
                Text = "Сравнить Дейкстра × n и Флойд",
                Location = new Point(10, y),
                Size = new Size(250, 30),
                BackColor = Color.FromArgb(180, 215, 240),
                ForeColor = blueText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnCompare.FlatAppearance.BorderColor = Color.FromArgb(100, 160, 210);
            btnCompare.Click += (s, e) => CompareAlgorithms();

            y += 40;

            // Строка 4: Сохранение/Загрузка
            var btnSave = new Button
            {
                Text = "Сохранить граф",
                Location = new Point(10, y),
                Size = new Size(140, 28),
                BackColor = Color.FromArgb(200, 225, 245),
                ForeColor = blueText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderColor = Color.FromArgb(100, 160, 210);
            btnSave.Click += (s, e) => SaveGraph();

            var btnLoad = new Button
            {
                Text = "Загрузить граф",
                Location = new Point(160, y),
                Size = new Size(140, 28),
                BackColor = Color.FromArgb(200, 225, 245),
                ForeColor = blueText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnLoad.FlatAppearance.BorderColor = Color.FromArgb(100, 160, 210);
            btnLoad.Click += (s, e) => LoadGraph();

            y += 38;

            // Строка 5: Инструкции
            var lblInstructions = new Label
            {
                Text = "Левый клик = Начало  Ctrl+Клик = Конец  Правый клик = Меню",
                Location = new Point(10, y),
                Size = new Size(500, 25),
                ForeColor = Color.FromArgb(80, 130, 200),
                Font = new Font("Arial", 8)
            };

            parent.Controls.AddRange(new Control[] {
                lblVertexCount, txtVertexCount, btnCreateGraph, btnLoadExample,
                lblStart, txtStart, lblEnd, txtEnd, btnFindPath, btnClearPath,
                btnCompare,
                btnSave, btnLoad,
                lblInstructions
            });
        }

        private void CreateDefaultGraph()
        {
            graph = new Graph(4);

            int[,] defaultEdges = new int[,]
            {
                {0, 1, 3},
                {0, 3, 7},
                {1, 0, 8},
                {1, 2, 2},
                {2, 0, 5},
                {2, 3, 1},
                {3, 0, 2}
            };

            for (int i = 0; i < defaultEdges.GetLength(0); i++)
            {
                graph.SetWeight(defaultEdges[i, 0], defaultEdges[i, 1], defaultEdges[i, 2]);
            }

            visualizer = new GraphVisualizer(graph);
            InitializeMatrixPanel();
            UpdateUI();

            txtStart.Value = 0;
            txtEnd.Value = 3;

            lblResult.Text = "Граф создан. Выберите начальную и конечную вершины.";
            lblResult.ForeColor = Color.FromArgb(0, 80, 180);
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
                    ForeColor = Color.FromArgb(0, 80, 180)
                };
                matrixPanel.Controls.Add(lblEmpty);
                return;
            }

            int n = graph.VerticesCount;
            int cellSize = n <= 10 ? 35 : (n <= 15 ? 28 : 24);
            int padding = 3;
            int startX = 30;
            int startY = 45;

            var headerFont = new Font("Arial", n <= 10 ? 8 : 7, FontStyle.Bold);
            Color blueHeader = Color.FromArgb(0, 80, 180);

            var lblInfo = new Label
            {
                Text = "Строка -> Столбец (вес ребра)",
                Location = new Point(startX, 5),
                Size = new Size(300, 25),
                Font = new Font("Arial", 8),
                ForeColor = Color.FromArgb(80, 130, 200)
            };
            matrixPanel.Controls.Add(lblInfo);

            matrixPanel.Controls.Add(new Label
            {
                Text = "Вес",
                Location = new Point(startX, startY),
                Size = new Size(cellSize, cellSize),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = headerFont,
                ForeColor = blueHeader
            });

            for (int j = 0; j < n; j++)
            {
                matrixPanel.Controls.Add(new Label
                {
                    Text = $"v{j}",
                    Location = new Point(startX + (j + 1) * (cellSize + padding), startY),
                    Size = new Size(cellSize, cellSize),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = headerFont,
                    ForeColor = blueHeader,
                    BackColor = Color.FromArgb(220, 235, 250)
                });
            }

            for (int i = 0; i < n; i++)
            {
                matrixPanel.Controls.Add(new Label
                {
                    Text = $"v{i}",
                    Location = new Point(startX, startY + (i + 1) * (cellSize + padding)),
                    Size = new Size(cellSize, cellSize),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = headerFont,
                    ForeColor = blueHeader,
                    BackColor = Color.FromArgb(220, 235, 250)
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
                        BackColor = (i == j) ? Color.FromArgb(220, 235, 250) : Color.FromArgb(240, 248, 255),
                        ReadOnly = (i == j),
                        ForeColor = Color.FromArgb(0, 80, 180),
                        Font = new Font("Arial", n <= 10 ? 8 : 7, FontStyle.Bold),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    if (weight > 0 && i != j)
                    {
                        textBox.BackColor = Color.FromArgb(180, 215, 240);
                    }

                    textBox.TextChanged += MatrixCell_TextChanged;
                    matrixPanel.Controls.Add(textBox);
                }
            }

            matrixPanel.AutoScrollMinSize = new Size(
                startX + (n + 1) * (cellSize + padding) + 20,
                startY + (n + 1) * (cellSize + padding) + 40
            );
        }

        private void MatrixCell_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || graph == null) return;

            var point = (Point)textBox.Tag;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                graph.SetWeight(point.X, point.Y, 0);
                textBox.BackColor = (point.X == point.Y) ? Color.FromArgb(220, 235, 250) : Color.FromArgb(240, 248, 255);
                graphPanel.Invalidate();
                return;
            }

            if (int.TryParse(textBox.Text, out int weight))
            {
                if (weight < 0)
                {
                    MessageBox.Show("Отрицательные веса запрещены!", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox.Text = "";
                    return;
                }

                graph.SetWeight(point.X, point.Y, weight);

                if (weight > 0 && point.X != point.Y)
                    textBox.BackColor = Color.FromArgb(180, 215, 240);
                else
                    textBox.BackColor = (point.X == point.Y) ? Color.FromArgb(220, 235, 250) : Color.FromArgb(240, 248, 255);

                graphPanel.Invalidate();
            }
            else
            {
                textBox.Text = "";
                graph.SetWeight(point.X, point.Y, 0);
                graphPanel.Invalidate();
            }
        }

        // ==================== ПОИСК ПУТИ (ДЕЙКСТРА) ====================

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

            if (start == end)
            {
                lblResult.Text = "Начальная и конечная вершины совпадают!";
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
                lblResult.Text = $"Путь v{start} → v{end}:\n" +
                                 $"Расстояние = {result.Distance}\n" +
                                 $"Маршрут: {pathStr}";
                lblResult.ForeColor = Color.FromArgb(0, 80, 180);
                visualizer.SetPath(result.Path, result.Distance);
                visualizer.SetStartVertex(start);
                visualizer.SetEndVertex(end);
                graphPanel.Invalidate();
            }
        }

        // ==================== СОХРАНЕНИЕ / ЗАГРУЗКА ====================

        private void SaveGraph()
        {
            if (graph == null || graph.VerticesCount == 0)
            {
                MessageBox.Show("Сначала создайте граф!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sfd = new SaveFileDialog
            {
                Filter = "Файлы графа (*.graph)|*.graph|Все файлы (*.*)|*.*",
                DefaultExt = "graph",
                FileName = "graph"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Не сохраняем координаты — только матрицу
                    GraphFile.Save(graph, sfd.FileName, null);
                    lblResult.Text = $"Граф сохранён: {Path.GetFileName(sfd.FileName)}";
                    lblResult.ForeColor = Color.FromArgb(0, 80, 180);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadGraph()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Файлы графа (*.graph)|*.graph|Все файлы (*.*)|*.*",
                DefaultExt = "graph"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var loadedGraph = GraphFile.Load(ofd.FileName, out var positions);
                    if (loadedGraph == null)
                    {
                        MessageBox.Show("Не удалось загрузить граф из файла!", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    graph = loadedGraph;
                    visualizer = new GraphVisualizer(graph);  // ← расставит по кругу

                    // НЕ восстанавливаем координаты — всегда рисуем по кругу
                    // (блок с positions удалён)

                    InitializeMatrixPanel();
                    UpdateUI();
                    graphPanel.Invalidate();
                    visualizer.ClearPath();

                    lblResult.Text = $"Граф загружен: {Path.GetFileName(ofd.FileName)} ({graph.VerticesCount} вершин)";
                    lblResult.ForeColor = Color.FromArgb(0, 80, 180);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        
        }

        // ==================== СРАВНЕНИЕ АЛГОРИТМОВ ====================

        private void CompareAlgorithms()
        {
            if (graph == null || graph.VerticesCount == 0)
            {
                lblResult.Text = "Ошибка: сначала создайте граф!";
                lblResult.ForeColor = Color.Red;
                return;
            }

            if (graph.HasNegativeCycle())
            {
                lblResult.Text = "Ошибка: граф содержит отрицательные циклы!";
                lblResult.ForeColor = Color.Red;
                MessageBox.Show("Граф содержит отрицательные циклы!\n\nАлгоритмы не могут работать с отрицательными циклами.",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int n = graph.VerticesCount;

            // ========== ФЛОЙД ==========
            var watchFloyd = Stopwatch.StartNew();
            var floydPaths = graph.FindAllPathsFloyd(out int[,] nextMatrix);
            watchFloyd.Stop();

            double floydTime = watchFloyd.Elapsed.TotalMilliseconds;  // ← микросекунды (точнее)

            // ========== ДЕЙКСТРА × n ==========
            var watchDijkstra = Stopwatch.StartNew();
            var dijkstraPaths = graph.FindAllPathsDijkstra();
            watchDijkstra.Stop();

            double dijkstraTime = watchDijkstra.Elapsed.TotalMilliseconds;  // ← микросекунды (точнее)

            // ========== ПОКАЗЫВАЕМ ПЕРВЫЙ ПУТЬ НА ГРАФЕ ==========
            if (floydPaths.Count > 0 && floydPaths[0].Path != null)
            {
                visualizer.SetPath(floydPaths[0].Path, floydPaths[0].Distance);
                visualizer.SetStartVertex(floydPaths[0].From);
                visualizer.SetEndVertex(floydPaths[0].To);
                graphPanel.Invalidate();
            }

            lblResult.Text = $"Сравнение завершено. Флойд: {floydTime} мс, Дейкстра × {n}: {dijkstraTime} мс";
            lblResult.ForeColor = Color.FromArgb(0, 80, 180);

            // ========== ОТКРЫВАЕМ ОКНО СРАВНЕНИЯ ==========
            int edgeCount = CountEdges();
            var comparisonForm = new ComparisonForm(
                dijkstraPaths,
                floydPaths,
                dijkstraTime,
                floydTime,
                n,
                edgeCount
            );
            comparisonForm.ShowDialog(this);
        }

        private int CountEdges()
        {
            int count = 0;
            int n = graph.VerticesCount;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (graph.GetWeight(i, j) > 0)
                        count++;
            return count;
        }

        // ==================== ОБРАБОТЧИКИ МЫШИ ====================

        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            if (graph == null || graph.VerticesCount == 0)
            {
                e.Graphics.Clear(Color.FromArgb(240, 248, 255));
                using (var font = new Font("Arial", 14, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(100, 150, 200)))
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
                    isDragging = true;
                    visualizer.StartDragging(vertex);
                }
            }
        }

        private void GraphPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (graph == null) return;
            if (!isDragging) return;

            visualizer.DragVertex(e.Location);
            graphPanel.Invalidate();
        }

        private void GraphPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (graph == null) return;
            isDragging = false;
            visualizer.StopDragging();
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
                        lblResult.ForeColor = Color.FromArgb(0, 80, 180);
                    }
                    else
                    {
                        visualizer.SetStartVertex(vertex);
                        txtStart.Value = vertex;
                        lblResult.Text = $"Начальная вершина: v{vertex}";
                        lblResult.ForeColor = Color.FromArgb(0, 80, 180);
                    }
                    graphPanel.Invalidate();
                }
            }
        }

        private void ShowContextMenu(Point location)
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.BackColor = Color.FromArgb(240, 248, 255);
            contextMenu.ForeColor = Color.FromArgb(0, 80, 180);

            var addVertexItem = new ToolStripMenuItem("Добавить вершину");
            addVertexItem.ForeColor = Color.FromArgb(0, 80, 180);
            addVertexItem.Click += (s, e) =>
            {
                if (graph.VerticesCount < 20)
                {
                    graph.AddVertex();
                    visualizer = new GraphVisualizer(graph);
                    InitializeMatrixPanel();
                    UpdateUI();
                    graphPanel.Invalidate();
                    lblResult.Text = $"Вершина добавлена. Всего: {graph.VerticesCount}";
                    lblResult.ForeColor = Color.FromArgb(0, 80, 180);
                }
                else
                {
                    MessageBox.Show("Максимум 20 вершин!", "Лимит достигнут",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            var addEdgeItem = new ToolStripMenuItem("Добавить ребро");
            addEdgeItem.ForeColor = Color.FromArgb(0, 80, 180);
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
                        lblResult.ForeColor = Color.FromArgb(0, 80, 180);
                    }
                }
            };

            var deleteVertexItem = new ToolStripMenuItem("Удалить вершину");
            deleteVertexItem.ForeColor = Color.FromArgb(0, 80, 180);
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
                    lblResult.ForeColor = Color.FromArgb(0, 80, 180);
                }
            };

            var deleteEdgeItem = new ToolStripMenuItem("Удалить ребро");
            deleteEdgeItem.ForeColor = Color.FromArgb(0, 80, 180);
            deleteEdgeItem.Click += (s, e) =>
            {
                var fromTo = AddEdgeDialog.ShowDialog(graph.VerticesCount);
                if (fromTo != null)
                {
                    graph.SetWeight(fromTo.From, fromTo.To, 0);
                    InitializeMatrixPanel();
                    graphPanel.Invalidate();
                    lblResult.Text = $"Ребро v{fromTo.From} → v{fromTo.To} удалено";
                    lblResult.ForeColor = Color.FromArgb(0, 80, 180);
                }
            };

            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                addVertexItem,
                new ToolStripSeparator(),
                addEdgeItem,
                deleteEdgeItem,
                new ToolStripSeparator(),
                deleteVertexItem
            });

            contextMenu.Show(graphPanel, location);
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            // При запуске показываем пример, но не ищем путь автоматически
            lblResult.Text = "Граф создан. Выберите начальную и конечную вершины\n" +
                             "и нажмите 'Найти путь' (алгоритм Дейкстры).\n\n" +
                             "Для сравнения всех алгоритмов нажмите 'Сравнить'.";
        }

        private void UpdateUI()
        {
            if (graph != null)
            {
                txtVertexCount.Value = graph.VerticesCount;
                txtStart.Maximum = Math.Max(0, graph.VerticesCount - 1);
                txtEnd.Maximum = Math.Max(0, graph.VerticesCount - 1);
                if (txtStart.Value >= graph.VerticesCount) txtStart.Value = graph.VerticesCount - 1;
                if (txtEnd.Value >= graph.VerticesCount) txtEnd.Value = graph.VerticesCount - 1;
            }
        }
    }
}