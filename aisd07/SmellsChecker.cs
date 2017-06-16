using System;
using System.Collections.Generic;

namespace Lab07
{

    public class SmellsChecker
    {

        private readonly int smellCount;
        private readonly int[][] customerPreferences;
        private readonly int satisfactionLevel;
		private readonly int customerCount;

        /// <summary>
        ///   
        /// </summary>
        /// <param name="smellCount">Liczba zapachów, którymi dysponuje sklep</param>
        /// <param name="customerPreferences">Preferencje klientów
        /// Każda tablica -- element tablicy tablic -- to preferencje jednego klienta.
        /// Preferencje klienta mają długość smellCount, na i-tej pozycji jest
        ///  1 -- klient preferuje zapach
        ///  0 -- zapach neutralny
        /// -1 -- klient nie lubi zapachu
        /// 
        /// Zapachy numerujemy od 0
        /// </param>
        /// <param name="satisfactionLevel">Oczekiwany poziom satysfakcji</param>
        public SmellsChecker(int smellCount, int[][] customerPreferences, int satisfactionLevel)
        {
            this.smellCount = smellCount;
			this.customerCount = customerPreferences.Length;
            this.customerPreferences = customerPreferences;
            this.satisfactionLevel = satisfactionLevel;
        }

		private int maxSatisfiedCustomers;
		private int[] customerSatisfaction;
		private int[,] bounds;
		private bool[] currentSmells;
		private bool[] maximizeSmells;

		private int[,] DefineBounds()
		{
			int[,] maxOnesAfter = new int[customerCount, smellCount];
			for (int smell = smellCount - 2; smell >= 0; --smell)
				for (int cust = 0; cust < customerCount; ++cust)
					maxOnesAfter[cust, smell] = maxOnesAfter[cust, smell + 1] + (customerPreferences[cust][smell + 1] + 1) / 2;
			return maxOnesAfter;
		}

		/// <summary>
		/// Implementacja etapu 1
		/// </summary>
		/// <returns><c>true</c>, jeśli przypisanie jest możliwe <c>false</c> w p.p.</returns>
		/// <param name="smells">Wyjściowa tablica rozpylonych zapachów realizująca rozwiązanie, jeśli się da. null w p.p. </param>
		/// 


        public Boolean AssignSmells(out bool[] smells)
        {
			currentSmells = new bool[smellCount];
			customerSatisfaction = new int[customerCount];

			smells = currentSmells;
			bounds = DefineBounds();

			for (int smell = 0; smell < smellCount; ++smell)
			{
				currentSmells[smell] = true;
				for (int c = 0; c < customerCount; ++c)
					customerSatisfaction[c] += customerPreferences[c][smell];

				if (CheckSmells(smell))
					return true;

				for (int cust = 0; cust < customerCount; ++cust)
					customerSatisfaction[cust] -= customerPreferences[cust][smell];
				currentSmells[smell] = false;
			}
			smells = null;
            return false;
        }

		private bool CheckSmells(int lastSmell)
		{
			bool success = true;
			for (int cust = 0; cust < customerCount; ++cust)
			{
				if (customerSatisfaction[cust] + bounds[cust, lastSmell] < satisfactionLevel)
					return false;
				if (customerSatisfaction[cust] < satisfactionLevel)
					success = false;
			}
			if (success)
				return true;

			for (int smell = lastSmell + 1; smell < smellCount; ++smell)
			{
				currentSmells[smell] = true;
				for (int c = 0; c < customerCount; ++c)
					customerSatisfaction[c] += customerPreferences[c][smell];

				if (CheckSmells(smell))
					return true;	

				for (int cust = 0; cust < customerCount; ++cust)
					customerSatisfaction[cust] -= customerPreferences[cust][smell];
				currentSmells[smell] = false;
			}
			return false;
		}

        /// <summary>
        /// Implementacja etapu 2
        /// </summary>
        /// <returns>Maksymalna liczba klientów, których można usatysfakcjonować</returns>
        /// <param name="smells">Wyjściowa tablica rozpylonych zapachów, realizująca ten poziom satysfakcji</param>
        public int AssignSmellsMaximizeHappyCustomers(out bool[] smells)
        {
			maxSatisfiedCustomers = 0;
			currentSmells = new bool[smellCount];
			maximizeSmells = new bool[smellCount];
			customerSatisfaction = new int[customerCount];

			smells = maximizeSmells;
			bounds = DefineBounds();

			if (satisfactionLevel <= 0)
				return customerCount;
			
			for (int smell = 0; smell < smellCount; ++smell)
			{
				currentSmells[smell] = true;
				for (int c = 0; c < customerCount; ++c)
					customerSatisfaction[c] += customerPreferences[c][smell];

				CheckSmellsMaximizeHappyCustomers(smell);

				for (int cust = 0; cust < customerCount; ++cust)
					customerSatisfaction[cust] -= customerPreferences[cust][smell];
				currentSmells[smell] = false;
			}
			return maxSatisfiedCustomers;
		}

		private void CheckSmellsMaximizeHappyCustomers(int lastSmell)
		{
			int satisfiableCustomers = 0, satisfiedCustomers = 0;
			for (int cust = 0; cust < customerCount; ++cust)
			{
				if (customerSatisfaction[cust] + bounds[cust, lastSmell] >= satisfactionLevel)
					satisfiableCustomers++;
				if (customerSatisfaction[cust] >= satisfactionLevel)
					satisfiedCustomers++;
			}
			if (satisfiableCustomers <= maxSatisfiedCustomers)
				return;
			if (satisfiedCustomers > maxSatisfiedCustomers)
			{
				maxSatisfiedCustomers = satisfiedCustomers;
				currentSmells.CopyTo(maximizeSmells, 0);
				if (satisfiedCustomers == customerCount)
					return;
			}

			for (int smell = lastSmell + 1; smell < smellCount; ++smell)
			{
				currentSmells[smell] = true;
				for (int c = 0; c < customerCount; ++c)
					customerSatisfaction[c] += customerPreferences[c][smell];

				CheckSmellsMaximizeHappyCustomers(smell);

				for (int cust = 0; cust < customerCount; ++cust)
					customerSatisfaction[cust] -= customerPreferences[cust][smell];
				currentSmells[smell] = false;
			}
		}
	}

}

