using System;
using System.Collections.Generic;

namespace GraphRepresentations
{
    class Program
    {
        // Вершини графа
        static char[] verts = { 'a', 'b', 'c', 'd', 'e', 'f' };

        // Ребра (для твого графа)
        static List<(char, char)> edges = new List<(char, char)>
        {
            ('a','b'),
            ('a','c'),
            ('b','b'), // петля
            ('b','d'),
            ('c','e'),
            ('d','e'),
            ('e','f')
        };

        static void Main()
        {
            bool directed = false; // false = неорiєнтований граф

            Console.WriteLine("==== Завдання 1. Усi подання графа ====");

            // 1. Матриця сумiжностi
            int[,] adjMatrix = BuildAdjacencyMatrixFromEdgeList(verts, edges, directed);
            Console.WriteLine("\nМатриця сумiжностi:");
            PrintAdjacencyMatrix(adjMatrix, verts);

            // 2. Матриця iнцидентностi
            int[,] incMatrix = AdjacencyToIncidence(adjMatrix, directed);
            Console.WriteLine("\nМатриця iнцидентностi:");
            PrintIncidenceMatrix(incMatrix, verts);

            // 3. Список ребер
            Console.WriteLine("\nСписок ребер:");
            var edgeList = AdjacencyToEdgeList(adjMatrix, directed);
            PrintEdgeList(edgeList, verts, directed);

            // 4. Список сумiжностi
            Console.WriteLine("\nСписок сумiжностi:");
            var adjList = AdjacencyMatrixToAdjList(adjMatrix, verts, directed);
            PrintAdjacencyList(adjList);

            Console.WriteLine("\n==== Завдання 2. Перетворення ====");

            // 1) За матрицею сумiжностi -> матриця iнцидентностi
            Console.WriteLine("\n1) Матриця iнцидентності з матрицi сумiжностi:");
            var incFromAdj = AdjacencyToIncidence(adjMatrix, directed);
            PrintIncidenceMatrix(incFromAdj, verts);

            // 2) За матрицею iнцидентностi -> список ребер
            Console.WriteLine("\n2) Список ребер з матрицi iнцидентностi:");
            var edgeListFromInc = IncidenceToEdgeList(incFromAdj, directed);
            PrintEdgeList(edgeListFromInc, verts, directed);

            // 3) За матрицею сумiжностi -> список сумiжностi
            Console.WriteLine("\n3) Список сумiжності з матрицi сумiжностi:");
            var adjListFromAdj = AdjacencyMatrixToAdjList(adjMatrix, verts, directed);
            PrintAdjacencyList(adjListFromAdj);

            // 4) За матрицею iнцидентностi -> матриця сумiжностi
            Console.WriteLine("\n4) Матриця сумiжностi з матрицi iнцидентностi:");
            var adjFromInc = IncidenceToAdjacency(incFromAdj, directed);
            PrintAdjacencyMatrix(adjFromInc, verts);

            // 5) За матрицею сумiжностi -> список ребер
            Console.WriteLine("\n5) Список ребер з матрицi сумiжностi:");
            var edgeListFromAdj = AdjacencyToEdgeList(adjMatrix, directed);
            PrintEdgeList(edgeListFromAdj, verts, directed);

            // 6) За матрицею iнцидентностi -> список сумiжностi
            Console.WriteLine("\n6) Список сумiжностi з матрицi iнцидентностi:");
            var adjListFromInc = IncidenceToAdjList(incFromAdj, verts, directed);
            PrintAdjacencyList(adjListFromInc);

            Console.WriteLine("\n--- Кiнець. Натиснiть будь-яку клавiшу для виходу ---");
            Console.ReadKey();
        }

        // ==== Методи побудови і перетворень ====

        static int[,] BuildAdjacencyMatrixFromEdgeList(char[] vertices, List<(char, char)> edgesList, bool directed)
        {
            int n = vertices.Length;
            int[,] A = new int[n, n];
            foreach (var e in edgesList)
            {
                int i = Array.IndexOf(vertices, e.Item1);
                int j = Array.IndexOf(vertices, e.Item2);
                A[i, j]++;
                if (!directed && i != j) A[j, i]++;
            }
            return A;
        }

        static int[,] AdjacencyToIncidence(int[,] A, bool directed)
        {
            int n = A.GetLength(0);
            var cols = new List<int[]>();

            if (directed)
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        for (int k = 0; k < A[i, j]; k++)
                        {
                            int[] col = new int[n];
                            if (i == j) col[i] = 2;
                            else { col[i] = -1; col[j] = 1; }
                            cols.Add(col);
                        }
            }
            else
            {
                for (int i = 0; i < n; i++)
                    for (int j = i; j < n; j++)
                        for (int k = 0; k < A[i, j]; k++)
                        {
                            int[] col = new int[n];
                            if (i == j) col[i] = 2;
                            else { col[i] = 1; col[j] = 1; }
                            cols.Add(col);
                        }
            }

            int m = cols.Count;
            int[,] inc = new int[n, m];
            for (int c = 0; c < m; c++)
                for (int r = 0; r < n; r++)
                    inc[r, c] = cols[c][r];
            return inc;
        }

        static List<(int, int)> IncidenceToEdgeList(int[,] inc, bool directed)
        {
            int n = inc.GetLength(0), m = inc.GetLength(1);
            var res = new List<(int, int)>();

            for (int c = 0; c < m; c++)
            {
                if (directed)
                {
                    int tail = -1, head = -1;
                    for (int r = 0; r < n; r++)
                    {
                        if (inc[r, c] == -1) tail = r;
                        else if (inc[r, c] == 1) head = r;
                        else if (inc[r, c] == 2) { tail = r; head = r; }
                    }
                    if (tail != -1 && head != -1) res.Add((tail, head));
                }
                else
                {
                    var list = new List<int>();
                    for (int r = 0; r < n; r++)
                    {
                        if (inc[r, c] > 0) list.Add(r);
                    }
                    if (list.Count == 1) res.Add((list[0], list[0]));
                    else if (list.Count == 2) res.Add((list[0], list[1]));
                }
            }
            return res;
        }

        static int[,] IncidenceToAdjacency(int[,] inc, bool directed)
        {
            var edgeList = IncidenceToEdgeList(inc, directed);
            int n = inc.GetLength(0);
            int[,] A = new int[n, n];
            foreach (var e in edgeList)
            {
                A[e.Item1, e.Item2]++;
                if (!directed && e.Item1 != e.Item2)
                    A[e.Item2, e.Item1]++;
            }
            return A;
        }

        static List<(int, int)> AdjacencyToEdgeList(int[,] A, bool directed)
        {
            int n = A.GetLength(0);
            var list = new List<(int, int)>();
            if (directed)
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        for (int k = 0; k < A[i, j]; k++)
                            list.Add((i, j));
            }
            else
            {
                for (int i = 0; i < n; i++)
                    for (int j = i; j < n; j++)
                        for (int k = 0; k < A[i, j]; k++)
                            list.Add((i, j));
            }
            return list;
        }

        static Dictionary<char, List<char>> AdjacencyMatrixToAdjList(int[,] A, char[] vertices, bool directed)
        {
            int n = A.GetLength(0);
            var dict = new Dictionary<char, List<char>>();
            foreach (var v in vertices) dict[v] = new List<char>();

            if (directed)
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        for (int k = 0; k < A[i, j]; k++)
                            dict[vertices[i]].Add(vertices[j]);
            }
            else
            {
                for (int i = 0; i < n; i++)
                    for (int j = i; j < n; j++)
                        for (int k = 0; k < A[i, j]; k++)
                        {
                            if (i == j) dict[vertices[i]].Add(vertices[i]);
                            else
                            {
                                dict[vertices[i]].Add(vertices[j]);
                                dict[vertices[j]].Add(vertices[i]);
                            }
                        }
            }
            return dict;
        }

        static Dictionary<char, List<char>> IncidenceToAdjList(int[,] inc, char[] vertices, bool directed)
        {
            var edges = IncidenceToEdgeList(inc, directed);
            var dict = new Dictionary<char, List<char>>();
            foreach (var v in vertices) dict[v] = new List<char>();

            foreach (var e in edges)
            {
                char u = vertices[e.Item1], vch = vertices[e.Item2];
                dict[u].Add(vch);
                if (!directed && u != vch) dict[vch].Add(u);
            }
            return dict;
        }

        // ==== Вивід ====
        static void PrintAdjacencyMatrix(int[,] A, char[] vertices)
        {
            int n = A.GetLength(0);
            Console.Write("\t");
            for (int j = 0; j < n; j++) Console.Write(vertices[j] + "\t");
            Console.WriteLine();
            for (int i = 0; i < n; i++)
            {
                Console.Write(vertices[i] + "\t");
                for (int j = 0; j < n; j++) Console.Write(A[i, j] + "\t");
                Console.WriteLine();
            }
        }

        static void PrintIncidenceMatrix(int[,] inc, char[] vertices)
        {
            int n = inc.GetLength(0), m = inc.GetLength(1);
            Console.Write("\t");
            for (int c = 0; c < m; c++) Console.Write("e" + (c + 1) + "\t");
            Console.WriteLine();
            for (int r = 0; r < n; r++)
            {
                Console.Write(vertices[r] + "\t");
                for (int c = 0; c < m; c++) Console.Write(inc[r, c] + "\t");
                Console.WriteLine();
            }
        }

        static void PrintEdgeList(List<(int, int)> edgesList, char[] vertices, bool directed)
        {
            foreach (var e in edgesList)
            {
                char u = vertices[e.Item1], v = vertices[e.Item2];
                Console.WriteLine(directed ? $"{u} -> {v}" : $"{u} - {v}");
            }
        }

        static void PrintAdjacencyList(Dictionary<char, List<char>> adjList)
        {
            foreach (var kv in adjList)
            {
                Console.Write(kv.Key + ": ");
                Console.WriteLine(string.Join(", ", kv.Value));
            }
        }
    }
}
