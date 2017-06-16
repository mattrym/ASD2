
using System;
using System.Collections.Generic;
using ASD.Graphs;
using System.Linq;

/// <summary>
/// Klasa rozszerzająca klasę Graph o rozwiązania problemów największej kliki i izomorfizmu grafów metodą pełnego przeglądu (backtracking)
/// </summary>
public static class Lab10GraphExtender
    {
	/// <summary>
	/// Wyznacza największą klikę w grafie i jej rozmiar metodą pełnego przeglądu (backtracking)
	/// </summary>
	/// <param name="g">Badany graf</param>
	/// <param name="clique">Wierzchołki znalezionej największej kliki - parametr wyjściowy</param>
	/// <returns>Rozmiar największej kliki</returns>
	/// <remarks>
	/// 1) Uwzględniamy grafy sierowane i nieskierowane.
	/// 2) Nie wolno modyfikować badanego grafu.
	/// </remarks>

	public static int MaxClique(this Graph g, out int[] clique)
	{
		int n = g.VerticesCount;
		int[] verts = new int[n];
		clique = new int[1];
		clique[0] = 0;

		for (int v = 0; v < n; ++v)
		{
			verts[0] = v;
			CliqueAddVertex(g, 1, verts, ref clique);
		}
		return clique.Length;
	}

	private static void CliqueAddVertex(Graph g, int v, int[] verts, ref int[] maxClique)
	{
		if (v >= g.VerticesCount)
			return;

		for (int u = verts[v - 1] + 1; u < g.VerticesCount; ++u)
		{
			bool clique = true;
			for (int w = 0; w < v; ++w)
				if (g.GetEdgeWeight(u, verts[w]) == null
					|| (g.Directed && g.GetEdgeWeight(verts[w], u) == null))
				{
					clique = false;
					break;
				}
			if (clique)
			{
				verts[v] = u;
				if (v + 1 > maxClique.Length)
				{
					maxClique = new int[v + 1];
					for (int w = 0; w <= v; ++w)
						maxClique[w] = verts[w];
				}
				CliqueAddVertex(g, v + 1, verts, ref maxClique);
			}
		}
	}

	/// <summary>
	/// Bada izomorfizm grafów metodą pełnego przeglądu (backtracking)
	/// </summary>
	/// <param name="g">Pierwszy badany graf</param>
	/// <param name="h">Drugi badany graf</param>
	/// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g (jeśli grafy nie są izomorficzne to null) - parametr wyjściowy</param>
	/// <returns>Informacja, czy grafy g i h są izomorficzne</returns>
	/// <remarks>
	/// 1) Nie wolno korzystać z bibliotecznych metod do badania izomorfizmu
	/// 2) Uwzględniamy wagi krawędzi i "skierowalność grafu"
	/// 3) Nie wolno modyfikować badanych grafów.
	/// </remarks>

	public static bool IsomorpchismTest(this Graph g, Graph h, out int[] map)
	{
		map = null;
		if (h.VerticesCount != g.VerticesCount || h.EdgesCount != h.EdgesCount || g.Directed != h.Directed)
			return false;

		int n = g.VerticesCount;
		bool[] gUsed = new bool[n];
		map = new int[n];
		for (int i = 0; i < n; ++i)
			map[i] = -1;
		
		if (IsomorphismAddVertex(g, h, 0, map, gUsed))
			return true;
		map = null;
		return false;
	}

	private static bool IsomorphismAddVertex(Graph g, Graph h, int vh, int[] map, bool[] gUsed)
	{
		if (vh == g.VerticesCount)
			return true;

		int n = g.VerticesCount;
		for (int vg = 0; vg < n; ++vg)
			if (!gUsed[vg] && g.OutDegree(vg) == h.OutDegree(vh) && g.InDegree(vg) == h.InDegree(vh))
			{
				bool subIzo = true;
				for (int vTo = 0; vTo < vh; ++vTo)
					if (h.GetEdgeWeight(vh, vTo) != g.GetEdgeWeight(vg, map[vTo])
						|| (g.Directed && h.GetEdgeWeight(vTo, vh) != g.GetEdgeWeight(map[vTo], vg)))
					{
						subIzo = false;
						break;
					}

				if(subIzo)
				{
					map[vh] = vg;
					gUsed[vg] = true;
					if (IsomorphismAddVertex(g, h, vh + 1, map, gUsed))
						return true;
					gUsed[vg] = false;
				}
			}
		return false;
	}

}

