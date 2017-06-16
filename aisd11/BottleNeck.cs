using System;
using System.Collections.Generic;
using ASD.Graphs;


public static class BottleNeckExtender
{
	/// <summary>
	/// Wyszukiwanie "wąskich gardeł" w sieci przesyłowej
	/// </summary>
	/// <param name="g">Graf przepustowości krawędzi</param>
	/// <param name="c">Graf kosztów rozbudowy sieci (kosztów zwiększenia przepustowości)</param>
	/// <param name="p">Tablica mocy produkcyjnych/zapotrzebowania w poszczególnych węzłach</param>
	/// <param name="flowValue">Maksymalna osiągalna produkcja (parametr wyjściowy)</param>
	/// <param name="cost">Koszt rozbudowy sieci, aby możliwe było osiągnięcie produkcji flowValue (parametr wyjściowy)</param>
	/// <param name="flow">Graf przepływu dla produkcji flowValue (parametr wyjściowy)</param>
	/// <param name="ext">Tablica rozbudowywanych krawędzi (parametr wyjściowy)</param>
	/// <returns>
	/// 0 - zapotrzebowanie można zaspokoić bez konieczności zwiększania przepustowości krawędzi<br/>
	/// 1 - zapotrzebowanie można zaspokoić, ale trzeba zwiększyć przepustowość (niektórych) krawędzi<br/>
	/// 2 - zapotrzebowania nie można zaspokoić (zbyt małe moce produkcyjne lub nieodpowiednia struktura sieci
	///     - można jedynie zwiększać przepustowości istniejących krawędzi, nie wolno dodawać nowych)
	/// </returns>
	/// <remarks>
	/// Każdy element tablicy p opisuje odpowiadający mu wierzchołek<br/>
	///    wartość dodatnia oznacza moce produkcyjne (wierzchołek jest źródłem)<br/>
	///    wartość ujemna oznacza zapotrzebowanie (wierzchołek jest ujściem),
	///       oczywiście "możliwości pochłaniające" ujścia to moduł wartości elementu<br/>
	///    "zwykłym" wierzchołkom odpowiada wartość 0 w tablicy p<br/>
	/// <br/>
	/// Jeśli funkcja zwraca 0, to<br/>
	///    parametr flowValue jest równy modułowi sumy zapotrzebowań<br/>
	///    parametr cost jest równy 0<br/>
	///    parametr ext jest pustą (zeroelementową) tablicą<br/>
	/// Jeśli funkcja zwraca 1, to<br/>
	///    parametr flowValue jest równy modułowi sumy zapotrzebowań<br/>
	///    parametr cost jest równy sumarycznemu kosztowi rozbudowy sieci (zwiększenia przepustowości krawędzi)<br/>
	///    parametr ext jest tablicą zawierającą informację o tym o ile należy zwiększyć przepustowości krawędzi<br/>
	/// Jeśli funkcja zwraca 2, to<br/>
	///    parametr flowValue jest równy maksymalnej możliwej do osiągnięcia produkcji
	///      (z uwzględnieniem zwiększenia przepustowości)<br/>
	///    parametr cost jest równy sumarycznemu kosztowi rozbudowy sieci (zwiększenia przepustowości krawędzi)<br/>
	///    parametr ext jest tablicą zawierającą informację o tym o ile należy zwiększyć przepustowości krawędzi<br/>
	/// Uwaga: parametr ext zawiera informacje jedynie o krawędziach, których przepustowości trzeba zwiększyć
	//     (każdy element tablicy to opis jednej takiej krawędzi)
	/// </remarks>


	public static int BottleNeck(this Graph g, Graph c, int[] p, 
		out int flowValue, out int cost, out Graph flow, out Edge[] ext)
	{
		int n, totalNeed;
		Dictionary<Edge, int> proxies;
		Graph throughputs, costs, helpFlow;

		n = g.VerticesCount + g.EdgesCount + 2;
		totalNeed = g.PrepareAdaptedNetwork(c, p, out throughputs, out costs, out proxies);
		flowValue = throughputs.MinCostFlow(costs, n - 2, n - 1, out cost, out helpFlow);
		flow = g.ConvertFlow(helpFlow, proxies, out ext);

		if (flowValue == totalNeed)
			return Math.Sign(cost);
		return 2;    
	}

	private static int PrepareAdaptedNetwork(this Graph g, Graph c, int[] p, 
		out Graph throughputs, out Graph costs, out Dictionary<Edge,int> proxies)
	{
		int n = g.VerticesCount + g.EdgesCount + 2;
		int itExtraVert = g.VerticesCount;
		int totalCost = 0;

		throughputs = g.IsolatedVerticesGraph(true, n);
		costs = g.IsolatedVerticesGraph(true, n);
		proxies = new Dictionary<Edge, int>();

		for (int v = 0; v < g.VerticesCount; ++v)
		{
			if (p[v] > 0)
			{
				throughputs.AddEdge(n - 2, v, p[v]);
				costs.AddEdge(n - 2, v, 0);
			}
			else if (p[v] < 0)
			{
				totalCost -= p[v];
				throughputs.AddEdge(v, n - 1, -p[v]);
				costs.AddEdge(v, n - 1, 0);
			}
			foreach (Edge e in g.OutEdges(v))
			{
				int from = v, to = e.To, proxy = itExtraVert++;
				int ww = e.Weight, cc = c.GetEdgeWeight(from, to).Value;
				proxies.Add(e, proxy);

				throughputs.AddEdge(from, to, int.MaxValue);
				costs.AddEdge(from, to, cc);

				throughputs.AddEdge(from, proxy, ww);
				costs.AddEdge(from, proxy, 0);
				throughputs.AddEdge(proxy, to, ww);
				costs.AddEdge(proxy, to, 0);
			}
		}
		return totalCost;
	}

	private static Graph ConvertFlow(this Graph g, Graph helpFlow, 
		Dictionary<Edge,int> proxies, out Edge[] ext)
	{
		Graph flow = g.IsolatedVerticesGraph();
		List<Edge> edges = new List<Edge>();

		foreach (Edge e in proxies.Keys)
		{
			int from = e.From, to = e.To, proxy = proxies[e];
			int normalFlow = helpFlow.GetEdgeWeight(from, proxy).Value;
			int extraFlow = helpFlow.GetEdgeWeight(from, to).Value;
			flow.AddEdge(from, to, normalFlow + extraFlow);
			if (extraFlow > 0)
				edges.Add(new Edge(from, to, extraFlow));
		}
		ext = edges.ToArray();
		return flow;
	}
}
