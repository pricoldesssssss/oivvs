using System;
using System.Collections.Generic;
using System.Drawing;

namespace lab1
{
    public class GraphVisualizer
    {
        private Graph graph;
        private List<Point> vertexPositions;
        private List<int> path;
        private int pathDistance;
        private int startVertex = -1;
        private int endVertex = -1;
        private const int VertexRadius = 20;
        private int draggingVertex = -1;

        private Color[] pinkColors = new Color[]
        {
            Color.FromArgb(255, 182, 193),
            Color.FromArgb(255, 192, 203),
            Color.FromArgb(255, 105, 180),
            Color.FromArgb(255, 20, 147),
            Color.FromArgb(255, 182, 193),
            Color.FromArgb(255, 192, 203),
            Color.FromArgb(255, 105, 180),
            Color.FromArgb(255, 20, 147),
            Color.FromArgb(255, 182, 193),
            Color.FromArgb(255, 192, 203)
        };

        public GraphVisualizer(Graph graph)
        {
            this.graph = graph;
            vertexPositions = new List<Point>();
            path = new List<int>();
            InitializePositions();
        }

        private void InitializePositions()
        {
            vertexPositions.Clear();
            if (graph == null || graph.VerticesCount == 0) return;

            int n = graph.VerticesCount;
            int centerX = 300;
            int centerY = 250;
            int radius = Math.Min(centerX, centerY) - 60;

            for (int i = 0; i < n; i++)
            {
                double angle = 2 * Math.PI * i / n - Math.PI / 2;
                int x = centerX + (int)(radius * Math.Cos(angle));
                int y = centerY + (int)(radius * Math.Sin(angle));
                vertexPositions.Add(new Point(x, y));
            }
        }

        public void StartDragging(int vertex)
        {
            draggingVertex = vertex;
        }

        public void DragVertex(Point position)
        {
            if (draggingVertex >= 0 && draggingVertex < vertexPositions.Count)
            {
                int margin = 50;
                position.X = Math.Max(margin, Math.Min(600 - margin, position.X));
                position.Y = Math.Max(margin, Math.Min(500 - margin, position.Y));
                vertexPositions[draggingVertex] = position;
            }
        }

        public void StopDragging()
        {
            draggingVertex = -1;
        }

        public void SetVertexPosition(int vertex, Point position)
        {
            if (vertex >= 0 && vertex < vertexPositions.Count)
            {
                int margin = 50;
                position.X = Math.Max(margin, Math.Min(600 - margin, position.X));
                position.Y = Math.Max(margin, Math.Min(500 - margin, position.Y));
                vertexPositions[vertex] = position;
            }
        }

        public void SetPath(List<int> path, int distance)
        {
            this.path = path ?? new List<int>();
            this.pathDistance = distance;
        }

        public void ClearPath()
        {
            path.Clear();
            pathDistance = 0;
            startVertex = -1;
            endVertex = -1;
        }

        public void SetStartVertex(int vertex)
        {
            startVertex = vertex;
        }

        public void SetEndVertex(int vertex)
        {
            endVertex = vertex;
        }

        public int GetVertexAt(Point point)
        {
            for (int i = 0; i < vertexPositions.Count; i++)
            {
                if (GetDistance(point, vertexPositions[i]) < VertexRadius)
                    return i;
            }
            return -1;
        }

        public void Draw(Graphics g, Rectangle clipRect)
        {
            if (graph == null || graph.VerticesCount == 0)
            {
                g.Clear(Color.FromArgb(255, 248, 250));
                return;
            }

            g.Clear(Color.FromArgb(255, 248, 250));
            DrawEdges(g);
            DrawVertices(g);
            DrawLabels(g);
            if (path.Count > 0)
                DrawPath(g);
            //DrawLegend(g);
        }

        private void DrawEdges(Graphics g)
        {
            int n = graph.VerticesCount;
            Color edgeColor = Color.FromArgb(150, 80, 120);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int weight = graph.GetWeight(i, j);
                    if (weight > 0 && i != j)
                    {
                        Point start = vertexPositions[i];
                        Point end = vertexPositions[j];

                        Vector direction = new Vector(end.X - start.X, end.Y - start.Y);
                        direction.Normalize();

                        Point startPoint = new Point(
                            start.X + (int)(direction.X * VertexRadius),
                            start.Y + (int)(direction.Y * VertexRadius)
                        );
                        Point endPoint = new Point(
                            end.X - (int)(direction.X * VertexRadius),
                            end.Y - (int)(direction.Y * VertexRadius)
                        );

                        using (var pen = new Pen(edgeColor, 1.5f))
                        {
                            DrawArrow(g, pen, startPoint, endPoint);
                        }

                        Point midPoint = new Point(
                            (startPoint.X + endPoint.X) / 2,
                            (startPoint.Y + endPoint.Y) / 2
                        );

                        Vector offset = new Vector(-direction.Y, direction.X);
                        offset.Normalize();
                        Point labelPos = new Point(
                            midPoint.X + (int)(offset.X * 12),
                            midPoint.Y + (int)(offset.Y * 12)
                        );

                        using (var font = new Font("Arial", 9, FontStyle.Bold))
                        using (var brush = new SolidBrush(Color.FromArgb(200, 50, 100)))
                        {
                            StringFormat sf = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };
                            string weightText = weight.ToString();
                            SizeF textSize = g.MeasureString(weightText, font);
                            Rectangle rect = new Rectangle(
                                labelPos.X - (int)(textSize.Width / 2) - 4,
                                labelPos.Y - (int)(textSize.Height / 2) - 2,
                                (int)textSize.Width + 8,
                                (int)textSize.Height + 4
                            );
                            using (var bgBrush = new SolidBrush(Color.FromArgb(255, 248, 250)))
                            {
                                g.FillRectangle(bgBrush, rect);
                            }
                            g.DrawString(weightText, font, brush, labelPos, sf);
                        }
                    }
                }
            }
        }

        private void DrawArrow(Graphics g, Pen pen, Point start, Point end)
        {
            g.DrawLine(pen, start, end);

            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float arrowSize = 8;

            Point arrow1 = new Point(
                end.X - (int)(arrowSize * Math.Cos(angle - 0.5f)),
                end.Y - (int)(arrowSize * Math.Sin(angle - 0.5f))
            );
            Point arrow2 = new Point(
                end.X - (int)(arrowSize * Math.Cos(angle + 0.5f)),
                end.Y - (int)(arrowSize * Math.Sin(angle + 0.5f))
            );

            g.DrawLine(pen, end, arrow1);
            g.DrawLine(pen, end, arrow2);
        }

        private void DrawVertices(Graphics g)
        {
            for (int i = 0; i < vertexPositions.Count; i++)
            {
                Point pos = vertexPositions[i];
                Rectangle rect = new Rectangle(
                    pos.X - VertexRadius,
                    pos.Y - VertexRadius,
                    VertexRadius * 2,
                    VertexRadius * 2
                );

                Color fillColor = pinkColors[i % pinkColors.Length];
                Color borderColor = Color.FromArgb(180, 40, 90);

                if (i == startVertex)
                {
                    fillColor = Color.FromArgb(255, 100, 150);
                    borderColor = Color.FromArgb(200, 50, 100);
                }
                else if (i == endVertex)
                {
                    fillColor = Color.FromArgb(255, 50, 100);
                    borderColor = Color.FromArgb(180, 20, 70);
                }
                else if (path.Contains(i))
                {
                    fillColor = Color.FromArgb(255, 220, 240);
                    borderColor = Color.FromArgb(255, 150, 200);
                }

                using (var fillBrush = new SolidBrush(fillColor))
                using (var borderPen = new Pen(borderColor, 2))
                {
                    g.FillEllipse(fillBrush, rect);
                    g.DrawEllipse(borderPen, rect);

                    if (i == startVertex || i == endVertex)
                    {
                        using (var glowPen = new Pen(Color.FromArgb(50, borderColor), 4))
                        {
                            g.DrawEllipse(glowPen, rect);
                        }
                    }
                }
            }
        }

        private void DrawLabels(Graphics g)
        {
            for (int i = 0; i < vertexPositions.Count; i++)
            {
                Point pos = vertexPositions[i];
                using (var font = new Font("Arial", 10, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(180, 40, 90)))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString($"v{i}", font, brush, pos, sf);
                }
            }
        }

        private void DrawPath(Graphics g)
        {
            if (path.Count < 2) return;

            using (var pen = new Pen(Color.FromArgb(255, 50, 120), 3))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                for (int i = 0; i < path.Count - 1; i++)
                {
                    int from = path[i];
                    int to = path[i + 1];

                    if (from < vertexPositions.Count && to < vertexPositions.Count)
                    {
                        Point start = vertexPositions[from];
                        Point end = vertexPositions[to];

                        Vector direction = new Vector(end.X - start.X, end.Y - start.Y);
                        direction.Normalize();

                        Point startPoint = new Point(
                            start.X + (int)(direction.X * VertexRadius),
                            start.Y + (int)(direction.Y * VertexRadius)
                        );
                        Point endPoint = new Point(
                            end.X - (int)(direction.X * VertexRadius),
                            end.Y - (int)(direction.Y * VertexRadius)
                        );

                        g.DrawLine(pen, startPoint, endPoint);
                    }
                }
            }

            if (pathDistance > 0 && path.Count > 0)
            {
                using (var font = new Font("Arial", 12, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(200, 50, 100)))
                {
                    Point lastPos = vertexPositions[path[path.Count - 1]];
                    g.DrawString($"🌸 Расстояние: {pathDistance}", font, brush,
                                new Point(lastPos.X + VertexRadius + 10, lastPos.Y - 10));
                }
            }
        }

        private void DrawLegend(Graphics g)
        {
            int x = 10, y = 10;
            using (var font = new Font("Arial", 9))
            {
                // Начальная вершина
                using (var brush = new SolidBrush(Color.FromArgb(255, 100, 150)))
                using (var pen = new Pen(Color.FromArgb(200, 50, 100), 2))
                {
                    g.FillEllipse(brush, x, y, 15, 15);
                    g.DrawEllipse(pen, x, y, 15, 15);
                }
                g.DrawString("= Начальная вершина", font, Brushes.DarkGray, x + 20, y);
                y += 22;

                // Конечная вершина
                using (var brush = new SolidBrush(Color.FromArgb(255, 50, 100)))
                using (var pen = new Pen(Color.FromArgb(180, 20, 70), 2))
                {
                    g.FillEllipse(brush, x, y, 15, 15);
                    g.DrawEllipse(pen, x, y, 15, 15);
                }
                g.DrawString("= Конечная вершина", font, Brushes.DarkGray, x + 20, y);
                y += 22;

                // Вершина на пути
                using (var brush = new SolidBrush(Color.FromArgb(255, 220, 240)))
                using (var pen = new Pen(Color.FromArgb(255, 150, 200), 2))
                {
                    g.FillEllipse(brush, x, y, 15, 15);
                    g.DrawEllipse(pen, x, y, 15, 15);
                }
                g.DrawString("= Вершина на пути", font, Brushes.DarkGray, x + 20, y);
                y += 22;

                // Кратчайший путь
                using (var pen = new Pen(Color.FromArgb(255, 50, 120), 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    g.DrawLine(pen, x, y + 7, x + 15, y + 7);
                }
                g.DrawString("= Кратчайший путь", font, Brushes.DarkGray, x + 20, y);
                y += 22;

                // Обычная вершина
                using (var brush = new SolidBrush(Color.FromArgb(255, 182, 193)))
                using (var pen = new Pen(Color.FromArgb(180, 40, 90), 1))
                {
                    g.FillEllipse(brush, x, y, 15, 15);
                    g.DrawEllipse(pen, x, y, 15, 15);
                }
                g.DrawString("= Обычная вершина", font, Brushes.DarkGray, x + 20, y);
                y += 22;

                g.DrawString("🌸 Правый клик = меню", font, Brushes.LightPink, x, y);
                y += 20;
                g.DrawString("🌸 Ctrl+Клик = выбрать конец", font, Brushes.LightPink, x, y);
            }
        }

        private double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }

    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Normalize()
        {
            double length = Math.Sqrt(X * X + Y * Y);
            if (length > 0)
            {
                X /= length;
                Y /= length;
            }
        }
    }
}