using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace lab1
{
    public class PacketInfoForm : Form
    {
        private DataGridView dgvPackets;
        private DataGridView dgvRoutingTable;
        private Timer refreshTimer;
        private Func<List<Packet>> getPackets;
        private Func<RoutingTable[]> getRoutingTables;

        public PacketInfoForm(Func<List<Packet>> getPackets, Func<RoutingTable[]> getRoutingTables)
        {
            this.getPackets = getPackets;
            this.getRoutingTables = getRoutingTables;

            InitializeComponent();

            refreshTimer = new Timer { Interval = 200 };
            refreshTimer.Tick += (s, e) => RefreshData();
            refreshTimer.Start();
        }

        private void InitializeComponent()
        {
            this.Text = "Сведения о пакетах и таблицы маршрутизации";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 248, 255);

            // Заголовок
            var lblHeader = new Label
            {
                Text = "ИНФОРМАЦИЯ О ПАКЕТАХ И МАРШРУТИЗАЦИИ",
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 80, 180),
                BackColor = Color.FromArgb(200, 225, 245)
            };

            // Контейнер
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300,
                BackColor = Color.FromArgb(220, 235, 250)
            };

            // ========== ВЕРХ: ПАКЕТЫ ==========
            var topPanel = new Panel { Dock = DockStyle.Fill };

            var lblPacketsTitle = new Label
            {
                Text = "ПЕРЕДАВАЕМЫЕ ПАКЕТЫ",
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(70, 150, 220)
            };

            dgvPackets = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(248, 252, 255),
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Consolas", 9),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgvPackets.Columns.Add("Number", "№");
            dgvPackets.Columns.Add("From", "Откуда");
            dgvPackets.Columns.Add("To", "Куда");
            dgvPackets.Columns.Add("Current", "Сейчас");
            dgvPackets.Columns.Add("Size", "Размер");
            dgvPackets.Columns.Add("TTL", "TTL");
            dgvPackets.Columns.Add("Route", "Маршрут");
            dgvPackets.Columns.Add("Status", "Статус");

            dgvPackets.Columns["Number"].Width = 40;
            dgvPackets.Columns["From"].Width = 60;
            dgvPackets.Columns["To"].Width = 60;
            dgvPackets.Columns["Current"].Width = 60;
            dgvPackets.Columns["Size"].Width = 70;
            dgvPackets.Columns["TTL"].Width = 50;
            dgvPackets.Columns["Status"].Width = 90;

            topPanel.Controls.Add(dgvPackets);
            topPanel.Controls.Add(lblPacketsTitle);

            splitContainer.Panel1.Controls.Add(topPanel);

            // ========== НИЗ: ТАБЛИЦЫ МАРШРУТИЗАЦИИ ==========
            var bottomPanel = new Panel { Dock = DockStyle.Fill };

            var lblRoutingTitle = new Label
            {
                Text = "ТАБЛИЦЫ МАРШРУТИЗАЦИИ",
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 100, 180)
            };

            dgvRoutingTable = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(248, 252, 255),
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Consolas", 9)
            };

            dgvRoutingTable.Columns.Add("Vertex", "Узел");
            dgvRoutingTable.Columns.Add("Destination", "Куда");
            dgvRoutingTable.Columns.Add("HopCount", "Счётчик");
            dgvRoutingTable.Columns.Add("NextVertex", "Следующий");

            bottomPanel.Controls.Add(dgvRoutingTable);
            bottomPanel.Controls.Add(lblRoutingTitle);

            splitContainer.Panel2.Controls.Add(bottomPanel);

            // Кнопка OK
            var bottomButtonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
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
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderColor = Color.FromArgb(0, 80, 180);
            bottomButtonPanel.Controls.Add(btnOK);
            bottomButtonPanel.Resize += (s, e) =>
            {
                btnOK.Location = new Point((bottomButtonPanel.Width - btnOK.Width) / 2, 8);
            };

            this.Controls.Add(splitContainer);
            this.Controls.Add(bottomButtonPanel);
            this.Controls.Add(lblHeader);

            this.FormClosing += (s, e) => { refreshTimer.Stop(); };
        }

        private void RefreshData()
        {
            // Обновляем таблицу пакетов
            var packets = getPackets?.Invoke();
            if (packets != null)
            {
                dgvPackets.Rows.Clear();
                foreach (var p in packets.OrderBy(x => x.Number))
                {
                    string status = p.Delivered ? "Доставлен" :
                                    (!p.IsAlive ? "Просрочен" : "В пути");

                    dgvPackets.Rows.Add(
                        p.Number,
                        $"v{p.From}",
                        $"v{p.To}",
                        $"v{p.CurrentVertex}",
                        $"{p.Size} б",
                        $"{p.TicksAlive}/{p.TimeToLive}",
                        p.GetRouteString(),
                        status
                    );

                    // Подсветка
                    var row = dgvPackets.Rows[dgvPackets.Rows.Count - 1];
                    if (p.Delivered)
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    else if (!p.IsAlive)
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    else
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }

            // Обновляем таблицу маршрутизации
            var tables = getRoutingTables?.Invoke();
            if (tables != null)
            {
                dgvRoutingTable.Rows.Clear();
                foreach (var table in tables)
                {
                    if (table.Entries.Count == 0) continue;

                    foreach (var entry in table.Entries.Values.OrderBy(e => e.Destination))
                    {
                        dgvRoutingTable.Rows.Add(
                            $"v{table.Vertex}",
                            $"v{entry.Destination}",
                            entry.HopCount,
                            $"v{entry.NextVertex}"
                        );
                    }
                }
            }
        }
    }
}