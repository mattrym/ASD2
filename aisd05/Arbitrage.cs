using System;
using System.Collections;
using System.Collections.Generic;

namespace AsdLab5
{
	public class InvalidExchangeException : Exception
	{
		public InvalidExchangeException ()
		{
		}

		public InvalidExchangeException (string msg) : base(msg)
		{
		}

		public InvalidExchangeException (string msg, Exception ex) : base(msg, ex)
		{
		}
	}

	public struct ExchangePair
	{
		public readonly int From;
		public readonly int To;
		public readonly double Price;

		public ExchangePair (int from, int to, double price)
		{
			if (to < 0 || from < 0 || price <= 0.0)
				throw new InvalidExchangeException ();
			From = from;
			To = to;
			Price = price;
		}
	}

	public class CurrencyGraph
	{
		private static double priceToWeight (double price)
		{
			return -Math.Log (price);
		}

		private static double weightToPrice (double weight)
		{
			return Math.Exp (-weight);
		}

		private double[,] weights;

		public CurrencyGraph (int n, ExchangePair[] exchanges)
		{
			weights = new double[n, n];

			for (int i = 0; i < n; ++i)
				for (int j = 0; j < n; ++j)
					weights[i, j] = double.MaxValue;

			foreach (var ex in exchanges)
				weights[ex.From, ex.To] = priceToWeight(ex.Price);
			
			//
			// uzupelnic
			//
		}

		// wynik: true jesli nie na cyklu ujemnego
		// currency: waluta "startowa"
		// bestPrices: najlepszy (najwyzszy) kurs wszystkich walut w stosunku do currency (byc mo¿e osiagalny za pomoca wielu wymian)
		//   jesli wynik == false to bestPrices = null
		public bool findBestPrice (int currency, out double[] bestPrices)
		{
			//
			// wywolac odpowiednio FordBellmanShortestPaths
			// i na tej podstawie obliczyc bestPrices
			//

			int n = weights.GetLength(1);
			double[] distances = null;
			int[] previous = null;
			bestPrices = null;

			if (!FordBellmanShortestPaths(currency, out distances, out previous))
				return false;
			
			bestPrices = new double[n];
			for(int v=0; v<n; ++v)
				bestPrices[v] = weightToPrice(distances[v]);
			return true;
		}

		// wynik: true jesli jest mozliwosc arbitrazu, false jesli nie ma (nie rzucamy wyjatkow!)
		// currency: waluta "startowa"
		// exchangeCycle: a cycle of currencies starting from 'currency' and ending with 'currency'
		//  jesli wynik == false to exchangeCycle = null
		public bool findArbitrage (int currency, out int[]exchangeCycle)
		{
			//
			// Czêœæ 1: wywolac odpowiednio FordBellmanShortestPaths
			// Czêœæ 2: dodatkowo wywolac odpowiednio FindNegativeCostCycle
			//

			double[] distances = null;
			int[] previous = null;
			exchangeCycle = null;

			return !FordBellmanShortestPaths(currency, out distances, out previous) && FindNegativeCostCycle(distances, previous, out exchangeCycle);
		}

		// wynik: true jesli nie na cyklu ujemnego
		// s: wierzcho³ek startowy
		// dist: obliczone odleglosci
		// prev: tablica "poprzednich"
		private bool FordBellmanShortestPaths(int s, out double[] dist, out int[] prev)
		{
			int n = weights.GetLength(1);
			dist = new double[n];
			prev = new int[n];

			for (int v = 0; v < n; ++v)
			{
				dist[v] = double.MaxValue;
				prev[v] = -1;
			}
			dist[s] = 0;

			for (int k = 1; k < n; ++k)
				for (int i = 0; i < n; ++i)
					for (int j = 0; j < n; ++j)
						if (weights[i,j] != double.MaxValue && dist[j] > dist[i] + weights[i, j])
						{
							dist[j] = dist[i] + weights[i, j];
							prev[j] = i;
						}

			for (int i = 0; i < n; ++i)
				for (int j = 0; j < n; ++j)
					if (dist[j] > dist[i] + weights[i, j])
						return false;

			return true;
			//
			// implementacja algorytmu Forda-Bellmana
			//

		}

		// wynik: true jesli JEST cykl ujemny
		// dist: tablica odleglosci
		// prev: tablica "poprzednich"
		// cycle: wyznaczony cykl (kolejne elementy to kolejne wierzcholki w cyklu, pierwszy i ostatni element musza byc takie same - zamkniêcie cyklu)
		private bool FindNegativeCostCycle (double[] dist, int[] prev, out int[] cycle)
		{

			//
			// wyznaczanie cyklu ujemnego
			// przykladowy pomysl na algorytm
			// 1) znajdowanie wierzcholka, którego odleglosc zostalaby poprawiona w kolejnej iteracji algorytmu Forda-Bellmana
			// 2) cofanie sie po lancuchu poprzednich (prev) - gdy zaczna sie powtarzac to znaleŸlismy wierzcholek nale¿acy do cyklu o ujemnej dlugosci
			// 3) konstruowanie odpowiedzi zgodnie z wymogami zadania
			//

			cycle = null;
			int u, v=-1;
			int n = weights.GetLength(1);
			bool[] inCycle = new bool[n];
			List<int> cycleList = new List<int>();

			// znajdowanie wierzcholka do poprawy - znajduje sie on w cyklu (ustawiamy nowa odleglosc oraz poprzednika)
			for (int i=0; i<n; ++i)
				for(int j=0; j<n; ++j)
					if (dist[j] > dist[i] + weights[i, j])
					{
						dist[j] = dist[i] + weights[i, j];
						prev[j] = i;
						v = j;
						break;
					}

			// gdy nie znaleziono - nie ma ujemnych cykli
			if (v == -1)
				return false;

			// szukamy poczatku cyklu - wierzcholek v
			while (!inCycle[v])
			{
				inCycle[v] = true;
				v = prev[v];
			}
			u = prev[v];

			// dodajemy po kolei od v wierzcholki do listy i odwracamy ja
			cycleList.Add(v);
			while (u != v)
			{
				cycleList.Add(u);
				u = prev[u];
			}
			cycleList.Add(v);
			cycleList.Reverse();
			cycle = cycleList.ToArray();
			
			return true;
		}
	}
}