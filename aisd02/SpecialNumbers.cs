using System;


namespace ASD
{
	class SpecialNumbers
	{
		const int mod = 10000;

		// funkcja rekurencyjna
		// n cyfr
		public static int SpecialNumbersRec(int n)
		{
			if (n == 0)
				return 0;
			if (n == 1)
				return 9;
			int result = 0;
			for (int i = 9; i >= 1; --i)
				result += SpecialNumbersBegin(n-1, i);
			return result % mod;
		}

		private static int SpecialNumbersBegin(int n, int c)
		{
			if (n == 1)
				return (c >> 1) + 1;
			int result = SpecialNumbersBegin(n - 1, c);
			for(int i=c-1; i>=1; i -= 2)
			{
				result = (result + SpecialNumbersBegin(n - 1, i));
			}
			return result % mod;
		}

		// programowanie dynamiczne
		// n cyfr
		public static int SpecialNumbersDP(int n)
		{
			if (n == 0)
				return 0;
			if (n == 1)
				return 9;
			int i, j, k;
			int[] mem = new int[9];
			for (i = 0; i < 9; ++i)
				mem[i] = (i + 3) >> 1;
			for (i = 2; i < n; ++i)
				for (j = 8; j >= 0; --j)
				{
					for (k = j - 1; k >= 0; k -= 2)
						mem[j] += mem[k];
					mem[j] %= mod;
				}
			int result = 0;
			for (i = 0; i < 9; ++i)
				result += mem[i];
			return result % mod;
        }

        // programowanie dynamiczne
        // n cyfr
        // req - tablica z wymaganiami, jezeli req[i, j] == 0 to znaczy, ze  i + 1 nie moze stac PRZED j + 1
        public static int SpecialNumbersDP(int n, bool[,] req)
        {
			if (n == 0)
				return 0;
			if (n == 1)
				return 9;
			int i, j, k;
			int result = 0;
			int[] mem = new int[9], hmem;
			for (i = 0; i < 9; ++i)
				for (j = 0; j < 9; ++j)
					mem[i] += req[i, j] ? 1 : 0;
			for (i = 2; i < n; ++i)
			{
				hmem = new int[9];
				for (j = 8; j >= 0; --j)
				{
					for (k = 8; k >= 0; --k)
						if (req[j, k])
							hmem[j] += mem[k];
					hmem[j] %= mod;
				}
				mem = hmem;
			}
			for (int c = 0; c < 9; ++c)
				result += mem[c];
			return result % mod;
		}

    }//class SpecialNumbers

}//namespace ASD