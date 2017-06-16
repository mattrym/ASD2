using System;
using System.Linq;
using ASD.Graphs;

namespace lab06
{
    class Pathfinder
    {
        Graph RoadsGraph;
        int[] CityCosts;
		int n;
		const int penalty = 10000;

		public Pathfinder(Graph roads, int[] cityCosts)
        {
            RoadsGraph = roads;
            CityCosts = cityCosts;
			n = roads.VerticesCount;
		}

        //uwagi do wszystkich części (graf najkrótszych ścieżek)
        //   Jeżeli nie ma ścieżki pomiędzy miastami A i B to tworzymy sztuczną krawędź od A do B o karnym koszcie 10 000.

        // return: tablica kosztów organizacji ŚDM dla poszczególnym miast gdzie
        // za koszt organizacji ŚDM uznajemy sumę kosztów dostania się ze wszystkim miast do danego miasta, bez uwzględnienia kosztów przechodzenia przez miasta.
        // minCost: najmniejszy koszt
        // paths: graf skierowany zawierający drzewo najkrótyszch scieżek od wszyskich miast do miasta organizującego ŚDM (miasta z najmniejszym kosztem organizacji). 
        public int[] FindBestLocationWithoutCityCosts(out int minCost, out Graph paths)
        {
			PathsInfo[][] pi = new PathsInfo[n][];
			for(int v = 0; v < n; ++v)
				RoadsGraph.DijkstraShortestPaths(v, out pi[v]);
			return CalculateMinCost(pi, true, out minCost, out paths);
		}

        // return: tak jak w punkcie poprzednim, ale tym razem
        // za koszt organizacji ŚDM uznajemy sumę kosztów dostania się ze wszystkim miast do wskazanego miasta z uwzględnieniem kosztów przechodzenia przez miasta (cityCosts[]).
        // Nie uwzględniamy kosztu przejścia przez miasto które organizuje ŚDM.
        // minCost: najlepszy koszt
        // paths: graf skierowany zawierający drzewo najkrótyszch scieżek od wszyskich miast do miasta organizującego ŚDM (miasta z najmniejszym kosztem organizacji). 
        public int[] FindBestLocation(out int minCost, out Graph paths)
        {
			PathsInfo[][] pi = new PathsInfo[n][];
			Graph graph = RoadsGraph.IsolatedVerticesGraph(true, n);
			for (int v = 0; v < n; ++v)
				foreach (Edge e in RoadsGraph.OutEdges(v))
					graph.AddEdge(v, e.To, e.Weight + CityCosts[v]);
			for (int v = 0; v < n; ++v)
				graph.DijkstraShortestPaths(v, out pi[v]);
			return CalculateMinCost(pi, false, out minCost, out paths);
		}

        // return: tak jak w punkcie poprzednim, ale tym razem uznajemy zarówno koszt przechodzenia przez miasta, jak i wielkość miasta startowego z którego wyruszają pielgrzymi.
        // Szczegółowo opisane jest to w treści zadania "Częśc 2". 
        // minCost: najlepszy koszt
        // paths: graf skierowany zawierający drzewo najkrótyszch scieżek od wszyskich miast do miasta organizującego ŚDM (miasta z najmniejszym kosztem organizacji). 
        public int[] FindBestLocationSecondMetric(out int minCost, out Graph paths)
        {
			PathsInfo[][] pi = new PathsInfo[n][];
			for (int u = 0; u < n; ++u)
			{
				Graph graph = RoadsGraph.IsolatedVerticesGraph(true, n);
				for (int v = 0; v < n; ++v)
					foreach (Edge e in RoadsGraph.OutEdges(v))
						graph.AddEdge(v, e.To, Math.Min(e.Weight, CityCosts[u]) + Math.Min(CityCosts[v], CityCosts[u]));
				graph.DijkstraShortestPaths(u, out pi[u]);
			}
			return CalculateMinCost(pi, false, out minCost, out paths);
		}

		private int[] CalculateMinCost(PathsInfo[][] pi, bool weightFromSource, out int minCost, out Graph paths)
		{
			minCost = int.MaxValue;
			int loc = -1;
			int[] costs = new int[n];
			paths = RoadsGraph.IsolatedVerticesGraph(true, n);

			for (int v = 0; v < n; ++v)
			{
				for (int u = 0; u < n; ++u)
					costs[v] += pi[u][v].Dist.GetValueOrDefault(penalty);
				if (costs[v] < minCost)
				{
					minCost = costs[v];
					loc = v;
				}
			}

			for (int u = 0; u < n; ++u)
				if (u != loc)
				{
					Edge[] edges = PathsInfo.ConstructPath(u, loc, pi[u]);
					if (edges != null)
						foreach (var e in edges)
						{
							int p = e.From, q = e.To;
							if(weightFromSource)
								paths.AddEdge(e);
							else
								paths.AddEdge(p, q, RoadsGraph.GetEdgeWeight(p, q).Value);
						}
					else
						paths.AddEdge(u, loc, penalty);
				}
			return costs;
		}

	}
}
