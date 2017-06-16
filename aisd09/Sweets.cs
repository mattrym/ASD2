using System;

using ASD.Graphs;

namespace Lab09
{
    class Sweets
    {
        /// <summary>
        /// Implementacja zadania 1
        /// </summary>
        /// <param name="childrenCount">Liczba dzieci</param>
        /// <param name="sweetsCount">Liczba batoników</param>
        /// <param name="childrenLikes">
        /// Tablica tablic upodobań dzieci. Tablica childrenLikes[i] zawiera indeksy batoników
        /// które lubi i-te dziecko. Dzieci i batoniki są indeksowane od 0.
        /// </param>
        /// <param name="assignment">
        /// Wynikowy parametr. assigment[i] zawiera indeks batonika które dostało i-te dziecko.
        /// Jeśli dziecko nie dostało żadnego batonika to -1.
        /// </param>
        /// <returns>Liczba dzieci, które dostały batonik.</returns>
        public static int Task1( int childrenCount, int sweetsCount, int[][] childrenLikes, out int[] assignment )
        {
			int n = childrenCount + sweetsCount, childrenSatisfied;
			Graph network = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n + 2), flow = null;
			for (int c = 0; c < childrenCount; ++c)
			{
				network.AddEdge(n, c);
				foreach (int p in childrenLikes[c])
					network.AddEdge(c, p + childrenCount);
			}
			for (int s = childrenCount; s < n; ++s)
				network.AddEdge(s, n + 1);

			childrenSatisfied = network.PushRelabelMaxFlow(n, n + 1, out flow);

			assignment = new int[childrenCount];
			for (int c = 0; c < childrenCount; ++c)
			{
				assignment[c] = -1;
				foreach (Edge e in flow.OutEdges(c))
					if (e.Weight == 1)
					{
						assignment[c] = e.To - childrenCount;
						break;
					}	
			}

			return childrenSatisfied;
		}


		/// <summary>
		/// Implementacja zadania 2
		/// </summary>
		/// <param name="childrenCount">Liczba dzieci</param>
		/// <param name="sweetsCount">Liczba batoników</param>
		/// <param name="childrenLikes">
		/// Tablica tablic upodobań dzieci. Tablica childrenLikes[i] zawiera indeksy batoników
		/// które lubi i-te dziecko. Dzieci i batoniki są indeksowane od 0.
		/// </param>
		/// <param name="childrenLimits">Tablica ograniczeń dla dzieci. childtrenLimits[i] to maksymalna liczba batoników jakie może zjeść i-te dziecko.</param>
		/// <param name="sweetsLimits">Tablica ograniczeń batoników. sweetsLimits[i] to dostępna liczba i-tego batonika.</param>
		/// <param name="happyChildren">Wynikowy parametr zadania 2a. happyChildren[i] powinien zawierać true jeśli dziecko jest zadowolone i false wpp.</param>
		/// <param name="shoppingList">Wynikowy parametr zadania 2b. shoppingList[i] poiwnno zawierać liczbę batoników i-tego rodzaju, które trzeba dokupić.</param>
		/// <returns>Maksymalna liczba rozdanych batoników.</returns>
		public static int Task2(int childrenCount, int sweetsCount, int[][] childrenLikes, int[] childrenLimits, int[] sweetsLimits, out bool[] happyChildren, out int[] shoppingList)
		{
			int n = childrenCount + sweetsCount, sweetsDistributed;
			Graph network = new AdjacencyListsGraph<HashTableAdjacencyList>(true, n + 2), flow = null;
			for (int c = 0; c < childrenCount; ++c)
			{
				network.AddEdge(n, c, childrenLimits[c]);
				foreach (int p in childrenLikes[c])
					network.AddEdge(c, p + childrenCount, childrenLimits[c]);
			}
			for (int s = 0; s < sweetsCount; ++s)
				network.AddEdge(s + childrenCount, n + 1, sweetsLimits[s]);

			sweetsDistributed = network.PushRelabelMaxFlow(n, n + 1, out flow);

			happyChildren = new bool[childrenCount];
			shoppingList = new int[sweetsCount];
			for (int c = 0; c < childrenCount; ++c)
			{
				int consumedSweets = 0;
				foreach (Edge e in flow.OutEdges(c))
					consumedSweets += e.Weight;
				happyChildren[c] = consumedSweets == childrenLimits[c];
				if (!happyChildren[c])
					shoppingList[childrenLikes[c][0]] += childrenLimits[c] - consumedSweets;
			}

			return sweetsDistributed;
		}
    }
}
