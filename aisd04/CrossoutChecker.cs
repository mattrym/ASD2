using System;

namespace ASD
{
    class CrossoutChecker
    {
        /// <summary>
        /// Sprawdza, czy podana lista wzorców zawiera wzorzec x
        /// </summary>
        /// <param name="patterns">Lista wzorców</param>
        /// <param name="x">Jedyny znak szukanego wzorca</param>
        /// <returns></returns>
        static bool comparePattern(char[][] patterns, char x)
        {
            foreach (char[] pat in patterns)
            {
                if (pat.Length == 1 && pat[0] == x)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Sprawdza, czy podana lista wzorców zawiera wzorzec xy
        /// </summary>
        /// <param name="patterns">Lista wzorców</param>
        /// <param name="x">Pierwszy znak szukanego wzorca</param>
        /// <param name="y">Drugi znak szukanego wzorca</param>
        /// <returns></returns>
        static bool comparePattern(char[][] patterns, char x, char y)
        {
            foreach (char[] pat in patterns)
            {
                if (pat.GetLength(0) == 2 && pat[0] == x && pat[1] == y)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Metoda sprawdza, czy podany ciąg znaków można sprowadzić do ciągu pustego przez skreślanie zadanych wzorców.
        /// Zakładamy, że każdy wzorzec składa się z jednego lub dwóch znaków!
        /// </summary>
        /// <param name="sequence">Ciąg znaków</param>
        /// <param name="patterns">Lista wzorców</param>
        /// <param name="crossoutsNumber">Minimalna liczba skreśleń gwarantująca sukces lub int.MaxValue, jeżeli się nie da</param>
        /// <returns></returns>
        public static bool Erasable(char[] sequence, char[][] patterns, out int crossoutsNumber)
        {
			if (sequence == null || sequence.Length == 0)
			{
				crossoutsNumber = 0;
				return true;
			}

			int n = sequence.Length;
			int[][] er = new int[n][];

			for (int i = 0; i < n; ++i)
				er[i] = new int[n - i];

			for (int i = 0; i < n; ++i)
				if (comparePattern(patterns, sequence[i]))
					er[0][i] = 1;
				else er[0][i] = int.MaxValue;

			for (int i = 0; i < n - 1; ++i)
				if (comparePattern(patterns, sequence[i], sequence[i+1]))
					er[1][i] = 1;
				else if (er[0][i] == 1 && er[0][i+1] == 1)
					er[1][i] = 2;
				else er[1][i] = int.MaxValue;

			for (int i = 2; i < n; ++i)
				for(int j = 0; j < n - i; ++j)
				{
					er[i][j] = int.MaxValue;
					for (int k = 0; k < i; ++k)
					{
						int a = er[k][j];
						int b = er[i - k - 1][j + k + 1];
						if (a != int.MaxValue && b != int.MaxValue && a + b < er[i][j])
							er[i][j] = a + b;
					}
					if (comparePattern(patterns, sequence[j], sequence[j + i]) && er[i-2][j+1] < er[i][j] - 1)
						er[i][j] = er[i-2][j+1]+1;
				}

			crossoutsNumber = er[n-1][0];
			return crossoutsNumber != int.MaxValue;
		}

		/// <summary>
		/// Metoda sprawdza, jaka jest minimalna długość ciągu, który można uzyskać z podanego poprzez skreślanie zadanych wzorców.
		/// Zakładamy, że każdy wzorzec składa się z jednego lub dwóch znaków!
		/// </summary>
		/// <param name="sequence">Ciąg znaków</param>
		/// <param name="patterns">Lista wzorców</param>
		/// <returns></returns>
		public static int MinimumRemainder(char[] sequence, char[][] patterns)
		{
			if (sequence == null || sequence.Length == 0)
				return 0;

			int n = sequence.Length;
			int[][] er = new int[n][];

			for (int i = 0; i < n; ++i)
				er[i] = new int[n - i];

			for (int i = 0; i < n; ++i)
				if (!comparePattern(patterns, sequence[i]))
					er[0][i] = 1;

			for (int i = 0; i < n - 1; ++i)
			{ 
				er[1][i] = er[0][i] + er[0][i + 1];
				if (comparePattern(patterns, sequence[i], sequence[i + 1]))
					er[1][i] = 0;
			}

			for (int i = 2; i < n; ++i)
				for (int j = 0; j < n - i; ++j)
				{
					er[i][j] = int.MaxValue;
					for (int k = 0; k < i; ++k)
					{
						int a = er[k][j];
						int b = er[i - k - 1][j + k + 1];
						if (a + b < er[i][j])
							er[i][j] = a + b;
					}
					if (comparePattern(patterns, sequence[j], sequence[j + i]))
						if (er[i - 2][j + 1] == 0)
							er[i][j] = 0;
						else if(er[i - 2][j + 1] < er[i][j] - 2)
							er[i][j] = er[i - 2][j + 1] + 2;
				}

			return er[n - 1][0];
		}

    // można dopisać metody pomocnicze

    }
}
