using System;
using System.Collections.Generic;
using System.Linq;

namespace lab1
{
    public class Graph
    {
        private int[,] adjacencyMatrix;
        private int verticesCount;

        public Graph(int n = 0)
        {
            verticesCount = Math.Min(Math.Max(n, 0), 20);  // до 20 вершин
            adjacencyMatrix = new int[verticesCount, verticesCount];
        }

        public int VerticesCount => verticesCount;

        public int GetWeight(int from, int to)
        {
            if (from < 0 || from >= verticesCount || to < 0 || to >= verticesCount)
                return 0;
            return adjacencyMatrix[from, to];
        }

        public void SetWeight(int from, int to, int weight)
        {
            if (from < 0 || from >= verticesCount || to < 0 || to >= verticesCount)
                return;
            adjacencyMatrix[from, to] = weight < 0 ? 0 : weight;
        }

        public void AddVertex()
        {
            if (verticesCount >= 20) return;

            int newSize = verticesCount + 1;
            var newMatrix = new int[newSize, newSize];

            for (int i = 0; i < verticesCount; i++)
                for (int j = 0; j < verticesCount; j++)
                    newMatrix[i, j] = adjacencyMatrix[i, j];

            adjacencyMatrix = newMatrix;
            verticesCount = newSize;
        }

        public void RemoveVertex(int vertex)
        {
            if (vertex < 0 || vertex >= verticesCount || verticesCount <= 1) return;

            int newSize = verticesCount - 1;
            var newMatrix = new int[newSize, newSize];

            int newI = 0;
            for (int i = 0; i < verticesCount; i++)
            {
                if (i == vertex) continue;
                int newJ = 0;
                for (int j = 0; j < verticesCount; j++)
                {
                    if (j == vertex) continue;
                    newMatrix[newI, newJ] = adjacencyMatrix[i, j];
                    newJ++;
                }
                newI++;
            }

            adjacencyMatrix = newMatrix;
            verticesCount = newSize;
        }

        public List<int> GetNeighbors(int vertex)
        {
            var neighbors = new List<int>();
            if (vertex < 0 || vertex >= verticesCount) return neighbors;

            for (int i = 0; i < verticesCount; i++)
            {
                if (adjacencyMatrix[vertex, i] > 0)
                    neighbors.Add(i);
            }
            return neighbors;
        }

        public bool HasNegativeCycle()
        {
            int n = verticesCount;
            var dist = new int[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    dist[i, j] = (i == j) ? 0 :
                                 (adjacencyMatrix[i, j] > 0 ? adjacencyMatrix[i, j] : int.MaxValue / 2);

            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                            dist[i, j] = dist[i, k] + dist[k, j];

            for (int i = 0; i < n; i++)
                if (dist[i, i] < 0)
                    return true;

            return false;
        }

        // ==================== АЛГОРИТМ ДЕЙКСТРЫ ====================

        public DijkstraResult Dijkstra(int start, int end)
        {
            if (start < 0 || start >= verticesCount || end < 0 || end >= verticesCount)
                return null;

            int n = verticesCount;
            var dist = new int[n];
            var prev = new int[n];
            var visited = new bool[n];

            // ШАГ 1: Инициализация
            for (int i = 0; i < n; i++)
            {
                dist[i] = int.MaxValue;
                prev[i] = -1;
            }

            dist[start] = 0;

           
            for (int count = 0; count < n - 1; count++)
            {
                
                int u = -1;
                int minDist = int.MaxValue;

                for (int i = 0; i < n; i++)
                {
                    if (!visited[i] && dist[i] < minDist)
                    {
                        minDist = dist[i];
                        u = i;
                    }
                }
                 
                //!!!!!!!!!!!!!!!!!!!
                if (u == -1 || u == end) break;

                
                visited[u] = true;

                
                for (int v = 0; v < n; v++)
                {
                    int weight = adjacencyMatrix[u, v];
                    if (weight > 0 && !visited[v] && dist[u] != int.MaxValue)
                    {
                        int newDist = dist[u] + weight;
                        if (newDist < dist[v])
                        {
                            dist[v] = newDist;
                            prev[v] = u;
                        }
                    }
                }
            }

            // ШАГ 5: Проверка на окончание
            if (dist[end] == int.MaxValue)
                return null;

            // Восстановление пути
            var path = new List<int>();
            int current = end;
            while (current != -1)
            {
                path.Insert(0, current);
                current = prev[current];
            }

            return new DijkstraResult
            {
                Distance = dist[end],
                Path = path
            };
        }

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!АЛГОРИТМ ФЛОЙДА 

        public FloydResult Floyd()
        {
            int n = verticesCount;

            int[,] dist = new int[n, n];
            int[,] next = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        dist[i, j] = 0;
                        next[i, j] = -1;
                    }
                    else if (adjacencyMatrix[i, j] > 0)
                    {
                        dist[i, j] = adjacencyMatrix[i, j];
                        next[i, j] = j;
                    }
                    else
                    {
                        dist[i, j] = int.MaxValue / 2;
                        next[i, j] = -1;
                    }
                }
            }

            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }

                if (dist[k, k] < 0)
                {
                    return null;
                }
            }

            return new FloydResult
            {
                Dist = dist,
                Next = next
            };
        }

        public List<int> GetFloydPath(int[,] next, int start, int end)
        {
            if (next == null || next[start, end] == -1)
                return null;

            var path = new List<int>();
            path.Add(start);

            int current = start;
            while (current != end)
            {
                current = next[current, end];
                if (current == -1)
                    return null;
                path.Add(current);
            }

            return path;
        }

        // ==================== ВСЕ ПУТИ ====================

        public List<PathResult> FindAllPathsDijkstra()
        {
            var results = new List<PathResult>();
            int n = verticesCount;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        var result = Dijkstra(i, j);
                        if (result != null)
                        {
                            results.Add(new PathResult
                            {
                                From = i,
                                To = j,
                                Distance = result.Distance,
                                Path = result.Path
                            });
                        }
                        else
                        {
                            results.Add(new PathResult
                            {
                                From = i,
                                To = j,
                                Distance = -1,
                                Path = null
                            });
                        }
                    }
                }
            }

            return results;
        }

        public List<PathResult> FindAllPathsFloyd(out int[,] nextMatrix)
        {
            var results = new List<PathResult>();
            var floydResult = Floyd();
            nextMatrix = floydResult?.Next;

            if (floydResult == null)
                return null;

            int n = verticesCount;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        var path = GetFloydPath(floydResult.Next, i, j);
                        if (path != null && path.Count > 0)
                        {
                            results.Add(new PathResult
                            {
                                From = i,
                                To = j,
                                Distance = floydResult.Dist[i, j],
                                Path = path
                            });
                        }
                        else
                        {
                            results.Add(new PathResult
                            {
                                From = i,
                                To = j,
                                Distance = -1,
                                Path = null
                            });
                        }
                    }
                }
            }

            return results;
        }

        public void PrintMatrix()
        {
            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = 0; j < verticesCount; j++)
                {
                    Console.Write($"{adjacencyMatrix[i, j],4}");
                }
                Console.WriteLine();
            }
        }
    }

    public class DijkstraResult
    {
        public int Distance { get; set; }
        public List<int> Path { get; set; }
    }

    public class FloydResult
    {
        public int[,] Dist { get; set; }
        public int[,] Next { get; set; }
    }

    public class PathResult
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Distance { get; set; }
        public List<int> Path { get; set; }
    }
}