using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace lab1
{
    public static class GraphFile
    {
        // Формат файла:
        // Строка 1: количество вершин
        // Строки 2..N+1: матрица весов (N чисел через пробел)
        // Строка N+2: координаты вершин (x1,y1;x2,y2;...)

        public static void Save(Graph graph, string fileName, List<Point> positions = null)
        {
            using (var writer = new StreamWriter(fileName))
            {
                int n = graph.VerticesCount;

                // Строка 1: количество вершин
                writer.WriteLine(n);

                // Строки 2..N+1: матрица весов
                for (int i = 0; i < n; i++)
                {
                    var row = new List<string>();
                    for (int j = 0; j < n; j++)
                    {
                        row.Add(graph.GetWeight(i, j).ToString());
                    }
                    writer.WriteLine(string.Join(" ", row));
                }

                // Строка N+2: координаты вершин (если есть)
                if (positions != null && positions.Count == n)
                {
                    var coords = positions.Select(p => $"{p.X},{p.Y}");
                    writer.WriteLine(string.Join(";", coords));
                }
            }
        }

        public static Graph Load(string fileName, out List<Point> positions)
        {
            positions = null;

            if (!File.Exists(fileName))
                return null;

            var lines = File.ReadAllLines(fileName);

            if (lines.Length < 1)
                return null;

            // Читаем количество вершин
            if (!int.TryParse(lines[0].Trim(), out int n))
                return null;

            if (n < 1 || n > 20)
                return null;

            var graph = new Graph(n);

            // Читаем матрицу весов
            for (int i = 0; i < n && i + 1 < lines.Length; i++)
            {
                var parts = lines[i + 1].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < n && j < parts.Length; j++)
                {
                    if (int.TryParse(parts[j], out int weight))
                    {
                        graph.SetWeight(i, j, weight);
                    }
                }
            }

            // Читаем координаты (если есть)
            if (lines.Length > n + 1)
            {
                var coordLine = lines[n + 1].Trim();
                if (!string.IsNullOrEmpty(coordLine))
                {
                    var coords = coordLine.Split(';');
                    positions = new List<Point>();
                    foreach (var coord in coords)
                    {
                        var xy = coord.Split(',');
                        if (xy.Length == 2 &&
                            int.TryParse(xy[0], out int x) &&
                            int.TryParse(xy[1], out int y))
                        {
                            positions.Add(new Point(x, y));
                        }
                    }
                    if (positions.Count != n)
                        positions = null;
                }
            }

            return graph;
        }
    }
}