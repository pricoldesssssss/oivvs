using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace lab1
{
    public partial class Form3 : Form
    {
        private Graph graph;
        private GraphVisualizer visualizer;

        // UI
        private Button btnStep;
        private Panel matrixPanel;
        private Panel graphPanel;
        private Label lblResult;
        private NumericUpDown txtVertexCount;
        private NumericUpDown txtStart;
        private NumericUpDown txtEnd;
        private NumericUpDown txtPacketCount;
        private NumericUpDown txtTimeToLive;
        private RadioButton rbRandom;
        private RadioButton rbFlooding;
        private RadioButton rbPreviousExperience;
        private RadioButton rbVirtualChannel;
        private RadioButton rbDatagram;
        private Button btnStart;
        private Button btnStop;
        private Button btnShowInfo;
        private Label lblMatrixTitle;

        // Разделители
        private SplitContainer splitContainerMain;
        private SplitContainer splitContainerRight;
        private SplitContainer splitContainerLeft;

        // Данные лабы
        private List<Packet> allPackets = new List<Packet>();
        private List<Packet> activePackets = new List<Packet>();
        private RoutingTable[] routingTables;
        private Timer animationTimer;
        private Random random = new Random();
        private bool isRunning = false;
        private int nextPacketNumber = 1;
        private int packetDelayCounter = 0;
        private int packetDelay = 3;

        private PacketInfoForm infoForm;
        private bool isDragging = false;

        public Form3()
        {
            InitializeComponent1();
            CreateDefaultGraph();
            this.Shown += Form3_Shown;
        }

        private void InitializeComponent1()
        {
            this.Text = "Лабораторная №3 - Алгоритмы маршрутизации";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 248, 255);

            splitContainerMain = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400,
                BackColor = Color.FromArgb(220, 235, 250),
                SplitterWidth = 6
            };

            splitContainerLeft = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300,
                BackColor = Color.FromArgb(220, 235, 250),
                SplitterWidth = 6
            };

            lblMatrixTitle = new Label
            {
                Text = "МАТРИЦА ВЕСОВ СЕТИ",
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

            splitContainerRight = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 550,
                BackColor = Color.FromArgb(220, 235, 250),
                SplitterWidth = 6
            };

            var lblGraphTitle = new Label
            {
                Text = "ВИЗУАЛИЗАЦИЯ ПЕРЕДАЧИ ПАКЕТОВ",
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

            var graphContainer = new Panel { Dock = DockStyle.Fill };
            graphContainer.Controls.Add(graphPanel);
            graphContainer.Controls.Add(lblGraphTitle);

            splitContainerRight.Panel1.Controls.Add(graphContainer);

            var infoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(220, 235, 250),
                Padding = new Padding(10)
            };

            var lblInfoTitle = new Label
            {
                Text = "ИНФОРМАЦИЯ О ПЕРЕДАЧЕ",
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 80, 180),
                BackColor = Color.FromArgb(200, 225, 245)
            };

            lblResult = new Label
            {
                Text = "Выберите параметры и нажмите 'Запустить передачу'",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = Color.FromArgb(0, 80, 180),
                Font = new Font("Consolas", 9),
                Padding = new Padding(10)
            };

            infoPanel.Controls.Add(lblResult);
            infoPanel.Controls.Add(lblInfoTitle);

            splitContainerRight.Panel2.Controls.Add(infoPanel);

            splitContainerMain.Panel2.Controls.Add(splitContainerRight);

            this.Controls.Add(splitContainerMain);

            animationTimer = new Timer { Interval = 50 };
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void CreateControls(Panel parent)
        {
            int y = 5;
            Color blueText = Color.FromArgb(0, 80, 180);
            Color blueButton = Color.FromArgb(200, 225, 245);

            // ===== ГРАФ =====
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
                Text = "Создать",
                Location = new Point(185, y),
                Size = new Size(80, 25),
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
                visualizer.SetPanelSize(graphPanel.Width, graphPanel.Height);
                routingTables = new RoutingTable[graph.VerticesCount];
                for (int i = 0; i < graph.VerticesCount; i++)
                    routingTables[i] = new RoutingTable(i);
                InitializeMatrixPanel();
                UpdateUI();
                graphPanel.Invalidate();
                lblResult.Text = $"Сеть создана с {n} узлами";
            };

            var btnLoadExample = new Button
            {
                Text = "Пример",
                Location = new Point(270, y),
                Size = new Size(80, 25),
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

            y += 32;

            // ===== ЗАГРУЗКА / СОХРАНЕНИЕ =====
            var btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(10, y),
                Size = new Size(100, 25),
                BackColor = blueButton,
                ForeColor = blueText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderColor = Color.FromArgb(100, 160, 210);
            btnSave.Click += (s, e) => SaveGraph();

            var btnLoad = new Button
            {
                Text = "Загрузить",
                Location = new Point(115, y),
                Size = new Size(100, 25),
                BackColor = blueButton,
                ForeColor = blueText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            btnLoad.FlatAppearance.BorderColor = Color.FromArgb(100, 160, 210);
            btnLoad.Click += (s, e) => LoadGraph();

            y += 32;

            // ===== АЛГОРИТМ =====
            var lblAlgorithm = new Label
            {
                Text = "Алгоритм маршрутизации:",
                Location = new Point(10, y),
                Size = new Size(200, 20),
                ForeColor = blueText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            y += 22;

            var algorithmGroup = new Panel
            {
                Location = new Point(10, y),
                Size = new Size(340, 75),
                BackColor = Color.FromArgb(200, 225, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            rbRandom = new RadioButton
            {
                Text = "Случайная",
                Location = new Point(10, 5),
                Size = new Size(150, 22),
                ForeColor = blueText,
                Font = new Font("Arial", 9),
                BackColor = Color.Transparent
            };

            rbFlooding = new RadioButton
            {
                Text = "Лавинная",
                Location = new Point(10, 27),
                Size = new Size(150, 22),
                ForeColor = blueText,
                Font = new Font("Arial", 9),
                Checked = true,
                BackColor = Color.Transparent
            };

            rbPreviousExperience = new RadioButton
            {
                Text = "По предыдущему опыту",
                Location = new Point(10, 49),
                Size = new Size(200, 22),
                ForeColor = blueText,
                Font = new Font("Arial", 9),
                BackColor = Color.Transparent
            };

            algorithmGroup.Controls.AddRange(new Control[] { rbRandom, rbFlooding, rbPreviousExperience });
            y += 80;

            // ===== МЕТОД ПЕРЕДАЧИ =====
            var lblMethod = new Label
            {
                Text = "Метод передачи:",
                Location = new Point(10, y),
                Size = new Size(200, 20),
                ForeColor = blueText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            y += 22;

            var methodGroup = new Panel
            {
                Location = new Point(10, y),
                Size = new Size(340, 50),
                BackColor = Color.FromArgb(200, 225, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            rbVirtualChannel = new RadioButton
            {
                Text = "Виртуальный канал",
                Location = new Point(10, 5),
                Size = new Size(200, 22),
                ForeColor = blueText,
                Font = new Font("Arial", 9),
                Checked = true,
                BackColor = Color.Transparent
            };

            rbDatagram = new RadioButton
            {
                Text = "Дейтаграммный",
                Location = new Point(10, 27),
                Size = new Size(200, 22),
                ForeColor = blueText,
                Font = new Font("Arial", 9),
                BackColor = Color.Transparent
            };

            methodGroup.Controls.AddRange(new Control[] { rbVirtualChannel, rbDatagram });
            y += 55;

            // ===== ПАРАМЕТРЫ =====
            var lblStart = new Label
            {
                Text = "Откуда:",
                Location = new Point(10, y),
                Size = new Size(60, 22),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = blueText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtStart = new NumericUpDown
            {
                Location = new Point(75, y),
                Width = 50,
                Minimum = 0,
                Maximum = 19,
                Value = 0,
                BackColor = Color.FromArgb(240, 248, 255),
                ForeColor = blueText
            };

            var lblEnd = new Label
            {
                Text = "Куда:",
                Location = new Point(135, y),
                Size = new Size(45, 22),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = blueText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtEnd = new NumericUpDown
            {
                Location = new Point(185, y),
                Width = 50,
                Minimum = 0,
                Maximum = 19,
                Value = 3,
                BackColor = Color.FromArgb(240, 248, 255),
                ForeColor = blueText
            };

            y += 28;

            var lblPacketCount = new Label
            {
                Text = "Пакетов:",
                Location = new Point(10, y),
                Size = new Size(60, 22),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = blueText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtPacketCount = new NumericUpDown
            {
                Location = new Point(75, y),
                Width = 50,
                Minimum = 1,
                Maximum = 50,
                Value = 5,
                BackColor = Color.FromArgb(240, 248, 255),
                ForeColor = blueText
            };

            var lblTTL = new Label
            {
                Text = "TTL:",
                Location = new Point(135, y),
                Size = new Size(45, 22),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = blueText,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            txtTimeToLive = new NumericUpDown
            {
                Location = new Point(185, y),
                Width = 50,
                Minimum = 5,
                Maximum = 100,
                Value = 30,
                BackColor = Color.FromArgb(240, 248, 255),
                ForeColor = blueText
            };

            y += 35;

            // ===== КНОПКИ УПРАВЛЕНИЯ =====
            btnStart = new Button
            {
                Text = "Запустить",
                Location = new Point(10, y),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(70, 150, 220),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnStart.FlatAppearance.BorderColor = Color.FromArgb(0, 80, 180);
            btnStart.Click += (s, e) => StartTransmission();

            btnStop = new Button
            {
                Text = "Стоп",
                Location = new Point(135, y),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(220, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Enabled = false
            };
            btnStop.FlatAppearance.BorderColor = Color.FromArgb(180, 100, 100);
            btnStop.Click += (s, e) => StopTransmission();

            btnStep = new Button
            {
                Text = "Далее ▶",
                Location = new Point(240, y),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(150, 220, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Enabled = false
            };
            btnStep.FlatAppearance.BorderColor = Color.FromArgb(80, 160, 80);
            btnStep.Click += (s, e) => StepTransmission();

            y += 42;

            btnShowInfo = new Button
            {
                Text = "Сведения о пакетах и таблицы",
                Location = new Point(10, y),
                Size = new Size(340, 30),
                BackColor = Color.FromArgb(180, 215, 240),
                ForeColor = blueText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnShowInfo.FlatAppearance.BorderColor = Color.FromArgb(100, 160, 210);
            btnShowInfo.Click += (s, e) => ShowInfoForm();

            parent.Controls.AddRange(new Control[] {
                lblVertexCount, txtVertexCount, btnCreateGraph, btnLoadExample,
                btnSave, btnLoad,
                lblAlgorithm, algorithmGroup,
                lblMethod, methodGroup,
                lblStart, txtStart, lblEnd, txtEnd,
                lblPacketCount, txtPacketCount, lblTTL, txtTimeToLive,
                btnStart, btnStop, btnStep, btnShowInfo
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
                graph.SetWeight(defaultEdges[i, 0], defaultEdges[i, 1], defaultEdges[i, 2]);

            visualizer = new GraphVisualizer(graph);
            visualizer.SetPanelSize(graphPanel.Width, graphPanel.Height);

            routingTables = new RoutingTable[graph.VerticesCount];
            for (int i = 0; i < graph.VerticesCount; i++)
                routingTables[i] = new RoutingTable(i);

            InitializeMatrixPanel();
            UpdateUI();

            lblResult.Text = "Сеть создана с примером. Выберите параметры.";
        }

        private void InitializeMatrixPanel()
        {
            matrixPanel.Controls.Clear();

            if (graph == null || graph.VerticesCount == 0) return;

            int n = graph.VerticesCount;
            int cellSize = n <= 10 ? 35 : (n <= 15 ? 28 : 24);
            int padding = 3;
            int startX = 30;
            int startY = 30;

            var headerFont = new Font("Arial", n <= 10 ? 8 : 7, FontStyle.Bold);
            Color blueHeader = Color.FromArgb(0, 80, 180);

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
                        textBox.BackColor = Color.FromArgb(180, 215, 240);

                    textBox.TextChanged += MatrixCell_TextChanged;
                    matrixPanel.Controls.Add(textBox);
                }
            }

            matrixPanel.AutoScrollMinSize = new Size(
                startX + (n + 1) * (cellSize + padding) + 20,
                startY + (n + 1) * (cellSize + padding) + 20);
        }

        private void MatrixCell_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null || graph == null) return;

            var pt = (Point)tb.Tag;

            if (string.IsNullOrEmpty(tb.Text))
            {
                graph.SetWeight(pt.X, pt.Y, 0);
                tb.BackColor = (pt.X == pt.Y) ? Color.FromArgb(220, 235, 250) : Color.FromArgb(240, 248, 255);
                graphPanel.Invalidate();
                return;
            }

            if (int.TryParse(tb.Text, out int w) && w >= 0)
            {
                graph.SetWeight(pt.X, pt.Y, w);
                tb.BackColor = (w > 0 && pt.X != pt.Y) ? Color.FromArgb(180, 215, 240) :
                               (pt.X == pt.Y) ? Color.FromArgb(220, 235, 250) : Color.FromArgb(240, 248, 255);
                graphPanel.Invalidate();
            }
            else
            {
                tb.Text = "";
                graph.SetWeight(pt.X, pt.Y, 0);
                graphPanel.Invalidate();
            }
        }

        // ==================== ЗАПУСК ПЕРЕДАЧИ ====================

        private void StartTransmission()
        {
            if (graph == null || graph.VerticesCount == 0)
            {
                MessageBox.Show("Сначала создайте сеть!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtStart.Value == txtEnd.Value)
            {
                MessageBox.Show("Начальный и конечный узлы совпадают!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StopTransmission();

            allPackets.Clear();
            activePackets.Clear();
            nextPacketNumber = 1;
            packetDelayCounter = 0;

            routingTables = new RoutingTable[graph.VerticesCount];
            for (int i = 0; i < graph.VerticesCount; i++)
                routingTables[i] = new RoutingTable(i);

            if (rbPreviousExperience.Checked)
            {
                for (int i = 0; i < graph.VerticesCount; i++)
                {
                    for (int j = 0; j < graph.VerticesCount; j++)
                    {
                        if (i != j && graph.GetWeight(i, j) > 0)
                        {
                            routingTables[i].UpdateEntry(j, 1, j);
                        }
                    }
                    routingTables[i].UpdateEntry(i, 0, i);
                }
            }

            isRunning = true;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnStep.Enabled = false;

            string algoName = rbRandom.Checked ? "Случайная" :
                              rbFlooding.Checked ? "Лавинная" : "По предыдущему опыту";
            string methodName = rbVirtualChannel.Checked ? "Виртуальный канал" : "Дейтаграммный";

            lblResult.Text = $"ЗАПУСК ПЕРЕДАЧИ\r\n" +
                             $"Алгоритм: {algoName}\r\n" +
                             $"Метод: {methodName}\r\n" +
                             $"Откуда: v{txtStart.Value} → Куда: v{txtEnd.Value}\r\n" +
                             $"Пакетов: {txtPacketCount.Value}\r\n" +
                             $"TTL: {txtTimeToLive.Value} тактов\r\n" +
                             $"──────────────────────────────\r\n";

            animationTimer.Start();
        }

        private void StopTransmission()
        {
            isRunning = false;
            animationTimer.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnStep.Enabled = true;
        }

        // ==================== ОДИН ШАГ АНИМАЦИИ ====================

        private void PerformOneTick()
        {
            int startNode = (int)txtStart.Value;
            int endNode = (int)txtEnd.Value;
            int totalPackets = (int)txtPacketCount.Value;
            int ttl = (int)txtTimeToLive.Value;

            packetDelayCounter++;
            if (packetDelayCounter >= packetDelay && allPackets.Count < totalPackets)
            {
                packetDelayCounter = 0;
                CreateNewPacket(startNode, endNode, ttl);
            }

            var toRemove = new List<Packet>();

            foreach (var packet in activePackets.ToList())
            {
                MovePacket(packet);

                if (packet.Delivered || !packet.IsAlive)
                {
                    toRemove.Add(packet);
                }
            }

            foreach (var p in toRemove)
            {
                activePackets.Remove(p);

                if (rbPreviousExperience.Checked && p.Delivered)
                {
                    UpdateRoutingTablesFromPacket(p);
                }
            }

            if (allPackets.Count >= totalPackets && activePackets.Count == 0)
            {
                FinishTransmission();
                return;
            }

            UpdateInfoLabel();
            graphPanel.Invalidate();
        }

        private void StepTransmission()
        {
            if (graph == null || graph.VerticesCount == 0) return;

            // Если передача не запущена вообще — сначала запускаем
            if (allPackets.Count == 0 && activePackets.Count == 0 && !isRunning)
            {
                StartTransmission();
                StopTransmission();
            }

            // Один тик анимации
            PerformOneTick();

            // Проверяем, всё ли завершено
            if (allPackets.Count >= (int)txtPacketCount.Value && activePackets.Count == 0)
            {
                btnStep.Enabled = false;
            }
        }

        // ==================== ТАЙМЕР ====================

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!isRunning) return;
            PerformOneTick();
        }

        // ==================== СОЗДАНИЕ ПАКЕТА ====================

        private void CreateNewPacket(int from, int to, int ttl)
        {
            var packet = new Packet(nextPacketNumber++, from, to)
            {
                TimeToLive = ttl,
                Size = 512 + random.Next(0, 1024),
                Color = Color.FromArgb(
                    100 + random.Next(155),
                    100 + random.Next(155),
                    100 + random.Next(155))
            };

            if (rbVirtualChannel.Checked)
            {
                var result = graph.Dijkstra(from, to);
                if (result == null)
                {
                    packet.IsAlive = false;
                    allPackets.Add(packet);
                    return;
                }
                packet.Route = new List<int>(result.Path);
                packet.RoutePosition = 0;
            }
            else
            {
                packet.Route = new List<int> { from };
                packet.RoutePosition = 0;
            }

            var startPos = visualizer.GetVertexPosition(from);
            packet.Position = new PointF(startPos.X, startPos.Y);
            packet.StartPos = packet.Position;

            // Для виртуального канала - идём по маршруту
            if (rbVirtualChannel.Checked && packet.Route.Count >= 2)
            {
                packet.NextVertex = packet.Route[1];
            }
            else
            {
                packet.NextVertex = DetermineNextVertex(packet);
            }

            if (packet.NextVertex != -1)
            {
                var nextPos = visualizer.GetVertexPosition(packet.NextVertex);
                packet.TargetPos = new PointF(nextPos.X, nextPos.Y);
            }
            else
            {
                packet.IsAlive = false;
            }

            allPackets.Add(packet);

            if (packet.IsAlive)
                activePackets.Add(packet);
        }

        // ==================== ВЫБОР СЛЕДУЮЩЕГО УЗЛА ====================

        private int DetermineNextVertex(Packet packet)
        {
            if (rbRandom.Checked)
            {
                var neighbors = graph.GetNeighbors(packet.CurrentVertex);

                if (packet.Route.Count >= 2)
                {
                    int previous = packet.Route[packet.Route.Count - 2];
                    neighbors.Remove(previous);
                }

                // Убираем уже посещённые, чтобы не зациклиться
                neighbors.RemoveAll(n => packet.Route.Contains(n));

                if (neighbors.Count == 0) return -1;
                return neighbors[random.Next(neighbors.Count)];
            }
            else if (rbFlooding.Checked)
            {
                var neighbors = graph.GetNeighbors(packet.CurrentVertex);

                if (packet.Route.Count >= 2)
                {
                    int previous = packet.Route[packet.Route.Count - 2];
                    neighbors.Remove(previous);
                }

                // ← КЛЮЧЕВОЕ: убираем уже посещённые, иначе бесконечное размножение
                neighbors.RemoveAll(n => packet.Route.Contains(n));

                if (neighbors.Count == 0) return -1;

                // Создаём копии для всех соседей, кроме первого
                for (int i = 1; i < neighbors.Count; i++)
                {
                    var copy = packet.Copy();
                    copy.Number = nextPacketNumber++;
                    copy.Route = new List<int>(packet.Route);
                    copy.Route.Add(neighbors[i]);
                    copy.NextVertex = neighbors[i];
                    var pos = visualizer.GetVertexPosition(neighbors[i]);
                    copy.StartPos = packet.Position;
                    copy.TargetPos = new PointF(pos.X, pos.Y);
                    copy.Position = packet.Position;
                    copy.Progress = 0;
                    copy.Color = Color.FromArgb(
                        100 + random.Next(155),
                        100 + random.Next(155),
                        100 + random.Next(155));

                    allPackets.Add(copy);
                    activePackets.Add(copy);
                }

                return neighbors[0];
            }
            else if (rbPreviousExperience.Checked)
            {
                var entry = routingTables[packet.CurrentVertex].GetEntry(packet.To);
                if (entry != null)
                    return entry.NextVertex;

                var neighbors = graph.GetNeighbors(packet.CurrentVertex);

                if (packet.Route.Count >= 2)
                {
                    int previous = packet.Route[packet.Route.Count - 2];
                    neighbors.Remove(previous);
                }

                neighbors.RemoveAll(n => packet.Route.Contains(n));

                if (neighbors.Count == 0) return -1;
                return neighbors[random.Next(neighbors.Count)];
            }

            return -1;
        }

        // ==================== ДВИЖЕНИЕ ПАКЕТА ====================

        private void MovePacket(Packet packet)
        {
            if (!packet.IsAlive || packet.NextVertex == -1) return;

            packet.Progress += 0.2f;

            float x = packet.StartPos.X + (packet.TargetPos.X - packet.StartPos.X) * packet.Progress;
            float y = packet.StartPos.Y + (packet.TargetPos.Y - packet.StartPos.Y) * packet.Progress;
            packet.Position = new PointF(x, y);

            if (packet.Progress >= 1.0f)
            {
                packet.Progress = 0;
                packet.CurrentVertex = packet.NextVertex;
                packet.Route.Add(packet.CurrentVertex);
                packet.RoutePosition = packet.Route.Count - 1;
                packet.TicksAlive++;

                // Проверка на доставку
                if (packet.CurrentVertex == packet.To)
                {
                    packet.Delivered = true;
                    return;
                }

                // Проверка TTL
                if (packet.TicksAlive >= packet.TimeToLive)
                {
                    packet.IsAlive = false;
                    return;
                }

                // Защита от бесконечного цикла
                if (packet.Route.Count > graph.VerticesCount * 3)
                {
                    packet.IsAlive = false;
                    return;
                }

                // Для виртуального канала - идём по маршруту
                if (rbVirtualChannel.Checked && packet.Route.Count < packet.Route.Count)
                {
                    // Получаем следующий узел из предварительно построенного маршрута
                }

                // Определяем следующий узел
                if (rbVirtualChannel.Checked)
                {
                    // Для виртуального канала маршрут уже построен заранее
                    int routeIndex = packet.RoutePosition + 1;
                    if (routeIndex < packet.Route.Count)
                    {
                        // Используем заранее построенный маршрут
                        // Но так как мы добавляем текущий узел в Route, нужно проверить
                    }

                    // Пересчитываем маршрут Дейкстрой
                    var result = graph.Dijkstra(packet.CurrentVertex, packet.To);
                    if (result != null && result.Path.Count >= 2)
                    {
                        packet.NextVertex = result.Path[1];
                    }
                    else
                    {
                        packet.NextVertex = -1;
                    }
                }
                else
                {
                    packet.NextVertex = DetermineNextVertex(packet);
                }

                if (packet.NextVertex == -1)
                {
                    packet.IsAlive = false;
                    return;
                }

                var pos = visualizer.GetVertexPosition(packet.CurrentVertex);
                packet.StartPos = new PointF(pos.X, pos.Y);
                packet.Position = packet.StartPos;

                var nextPos = visualizer.GetVertexPosition(packet.NextVertex);
                packet.TargetPos = new PointF(nextPos.X, nextPos.Y);
            }
        }

        private void UpdateRoutingTablesFromPacket(Packet packet)
        {
            for (int i = 0; i < packet.Route.Count - 1; i++)
            {
                int node = packet.Route[i];
                int hops = packet.Route.Count - i - 1;
                int nextNode = packet.Route[i + 1];

                routingTables[node].UpdateEntry(packet.To, hops, nextNode);

                for (int j = i + 1; j < packet.Route.Count; j++)
                {
                    int intermediate = packet.Route[j];
                    int intermediateHops = j - i;
                    int intermediateNext = (j == packet.Route.Count - 1) ? -1 : packet.Route[j + 1];

                    if (intermediateNext != -1)
                    {
                        routingTables[node].UpdateEntry(intermediate, intermediateHops, intermediateNext);
                    }
                }
            }
        }

        private void FinishTransmission()
        {
            isRunning = false;
            animationTimer.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnStep.Enabled = false;

            int delivered = allPackets.Count(p => p.Delivered);
            int expired = allPackets.Count(p => !p.Delivered && !p.IsAlive);
            int total = allPackets.Count;

            lblResult.Text = $"═══════════════════════════════\r\n" +
                             $"ПЕРЕДАЧА ЗАВЕРШЕНА\r\n" +
                             $"Всего пакетов: {total}\r\n" +
                             $"Доставлено: {delivered}\r\n" +
                             $"Просрочено (TTL): {expired}\r\n" +
                             $"Успешность: {(total > 0 ? 100.0 * delivered / total : 0):F1}%";
        }

        private void UpdateInfoLabel()
        {
            int delivered = allPackets.Count(p => p.Delivered);
            int active = activePackets.Count;
            int total = allPackets.Count;

            var recentPackets = allPackets.OrderByDescending(p => p.Number).Take(5).ToList();

            string info = $"Активных пакетов: {active} | Доставлено: {delivered}/{total}\r\n";
            info += "───────────────────────────────\r\n";
            info += "Последние пакеты:\r\n";

            foreach (var p in recentPackets)
            {
                string status = p.Delivered ? "✓ Доставлен" :
                                (!p.IsAlive ? "✗ Просрочен" : $"→ v{p.NextVertex}");
                info += $"#{p.Number}: v{p.From}→v{p.To} | {p.GetRouteString()} | {status}\r\n";
            }

            lblResult.Text = info;
        }

        private void ShowInfoForm()
        {
            if (infoForm == null || infoForm.IsDisposed)
            {
                infoForm = new PacketInfoForm(
                    () => allPackets,
                    () => routingTables
                );
                infoForm.Show(this);
            }
            else
            {
                infoForm.Activate();
            }
        }

        // ==================== ОТРИСОВКА ====================

        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            if (graph == null || graph.VerticesCount == 0)
            {
                e.Graphics.Clear(Color.FromArgb(240, 248, 255));
                using (var font = new Font("Arial", 14, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(100, 150, 200)))
                {
                    e.Graphics.DrawString("Создайте сеть для начала работы", font, brush,
                                new PointF(150, 200));
                }
                return;
            }

            visualizer.Draw(e.Graphics, e.ClipRectangle);
            DrawPackets(e.Graphics);
        }

        private void DrawPackets(Graphics g)
        {
            foreach (var packet in allPackets)
            {
                if (packet.Delivered && packet.Progress == 0) continue;

                float radius = 8;

                using (var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                {
                    g.FillEllipse(shadowBrush,
                                 packet.Position.X - radius + 2,
                                 packet.Position.Y - radius + 2,
                                 radius * 2, radius * 2);
                }

                using (var brush = new SolidBrush(packet.Color))
                {
                    g.FillEllipse(brush,
                                 packet.Position.X - radius,
                                 packet.Position.Y - radius,
                                 radius * 2, radius * 2);
                }

                using (var pen = new Pen(Color.FromArgb(80, 40, 0), 2))
                {
                    g.DrawEllipse(pen,
                                 packet.Position.X - radius,
                                 packet.Position.Y - radius,
                                 radius * 2, radius * 2);
                }

                using (var font = new Font("Arial", 7, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.Black))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(packet.Number.ToString(), font, brush, packet.Position, sf);
                }
            }
        }

        // ==================== МЫШЬ ====================

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
            if (graph == null || !isDragging) return;

            visualizer.DragVertex(e.Location);
            graphPanel.Invalidate();
        }

        private void GraphPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (graph == null) return;
            isDragging = false;
            visualizer.StopDragging();
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
                FileName = "network"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    GraphFile.Save(graph, sfd.FileName, null);
                    lblResult.Text = $"Сохранено: {Path.GetFileName(sfd.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
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
                    var loadedGraph = GraphFile.Load(ofd.FileName, out _);
                    if (loadedGraph == null)
                    {
                        MessageBox.Show("Не удалось загрузить!");
                        return;
                    }

                    graph = loadedGraph;
                    visualizer = new GraphVisualizer(graph);
                    visualizer.SetPanelSize(graphPanel.Width, graphPanel.Height);

                    routingTables = new RoutingTable[graph.VerticesCount];
                    for (int i = 0; i < graph.VerticesCount; i++)
                        routingTables[i] = new RoutingTable(i);

                    InitializeMatrixPanel();
                    UpdateUI();
                    graphPanel.Invalidate();

                    lblResult.Text = $"Загружено: {Path.GetFileName(ofd.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        // ==================== UI ====================

        private void Form3_Shown(object sender, EventArgs e)
        {
            if (visualizer != null)
            {
                visualizer.SetPanelSize(graphPanel.Width, graphPanel.Height);
                graphPanel.Invalidate();
            }

            graphPanel.Resize += (s, ev) =>
            {
                if (visualizer != null)
                {
                    visualizer.SetPanelSize(graphPanel.Width, graphPanel.Height);
                    graphPanel.Invalidate();
                }
            };
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