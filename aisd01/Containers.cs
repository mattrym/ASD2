
using System;

namespace ASD
{

	static class IContainerExtender                                                     // metoda rozszerzaj¹ca s³u¿¹ca do powiêkszania
	{                                                                                   // tablicy obiektu implementuj¹cego IContainer
		public static void Resize(this IContainer ic, ref int[] tab, int begin = 0)     // begin - indeks elementu pocz¹tkowego (domyœlnie 0)
		{
			int[] ntab = new int[ic.Count << 1];
			for (int i = begin; i < ic.Size; ++i)                                      // odwijanie
				ntab[i - begin] = tab[i];
			for (int i = 0; i < begin; ++i)
				ntab[ic.Size - begin + i] = tab[i];
			tab = ntab;
		}
	}

	public interface IContainer
    {
    void Put(int x);      //  dodaje element do kontenera

    int  Get();           //  zwraca pierwszy element kontenera i usuwa go z kontenera
                          //  w przypadku pustego kontenera zg³asza wyj¹tek typu EmptyException (zdefiniowany w Lab01_Main.cs)

    int  Peek();          //  zwraca pierwszy element kontenera (ten, który bêdzie pobrany jako pierwszy),
                          //  ale pozostawia go w kontenerze (czyli nie zmienia zawartoœci kontenera)
                          //  w przypadku pustego kontenera zg³asza wyj¹tek typu EmptyException (zdefiniowany w Lab01_Main.cs)

    int  Count { get; }   //  zwraca liczbê elementów w kontenerze

    int  Size  { get; }   //  zwraca rozmiar kontenera (rozmiar wewnêtznej tablicy)
    }

public class Stack : IContainer
    {
		private int[] tab;      // wewnêtrzna tablica do pamiêtania elementów
		private int count = 0;  // liczba elementów kontenera - metody Put i Get powinny (musz¹) to aktualizowaæ
		// nie wolno dodawaæ ¿adnych pól ani innych sk³adowych

		public Stack(int n=2)
			{
			tab = new int[n>2?n:2];
			}

		public void Put(int x)
			{
				if (count == Size)
					this.Resize(ref tab);
				tab[count++] = x;
			}

		public int Get()
			{
				if (count == 0)
					throw new EmptyException();
				return tab[--count];
			}

		public int Peek()
			{
				if (count == 0)
					throw new EmptyException();
				return tab[count - 1];
			}

		public int Count => count;

		public int Size => tab.Length;

    } // class Stack


public class Queue : IContainer
    {
		private int[] tab;      // wewnêtrzna tablica do pamiêtania elementów
		private int count = 0;  // liczba elementów kontenera - metody Put i Get powinny (musz¹) to aktualizowaæ
		private int begin = 0;	// mo¿na dodaæ jedno pole (wiêcej nie potrzeba)

		public Queue(int n=2)
        {
        tab = new int[n>2?n:2];
        }

		public void Put(int x)
		{
			if (count == Size)
			{
				this.Resize(ref tab, begin);
				begin = 0;
			}
			tab[(begin + count++) % Size] = x;
		}

		public int Get()
		{
			if (count == 0)
				throw new EmptyException();
			int val = tab[begin];
			begin = (begin + 1) % Size;
			count--;
			return val;
		}

		public int Peek()
		{
			if (count == 0)
				throw new EmptyException();
			return tab[begin]; 
		}

		public int Count => count;

		public int Size => tab.Length;

    } // class Queue


public class LazyPriorityQueue : IContainer
    {
		private int[] tab;      // wewnêtrzna tablica do pamiêtania elementów
		private int count = 0;  // liczba elementów kontenera - metody Put i Get powinny (musz¹) to aktualizowaæ
		// nie wolno dodawaæ ¿adnych pól ani innych sk³adowych

		public LazyPriorityQueue(int n=2)
        {
			tab = new int[n>2?n:2];
        }

		public void Put(int x)
		{
			if (count == Size)
				this.Resize(ref tab);
			tab[count++] = x;
		}

		public int Get()
		{
			if (count == 0)
				throw new EmptyException();
			int val, max = 0;
			for (int i = 1; i < count; ++i)
				if (tab[i] > tab[max])
					max = i;
			val = tab[max];
			tab[max] = tab[--count];
			return val; 
		}

		public int Peek()
		{
			if (count == 0)
				throw new EmptyException();
			int max = 0;
			for (int i = 1; i < count; ++i)
				if (tab[i] > tab[max])
					max = i;
			return tab[max];
		}

		public int Count => count;

		public int Size => tab.Length;

    } // class LazyPriorityQueue


public class HeapPriorityQueue : IContainer
    {
		private int[] tab;      // wewnêtrzna tablica do pamiêtania elementów
		private int count = 0;  // liczba elementów kontenera - metody Put i Get powinny (musz¹) to aktualizowaæ
		// nie wolno dodawaæ ¿adnych pól ani innych sk³adowych

		public HeapPriorityQueue(int n=2)
        {
			tab = new int[n>2?n:2];
        }

		public void Put(int x)
		{
			if (count == Size)
				this.Resize(ref tab);
			int k = ++count;
			while (k > 1 && tab[(k >> 1) - 1] < x)
			{
				tab[k - 1] = tab[(k >> 1) - 1];
				k >>= 1;
			}
			tab[k - 1] = x;
		}

		public int Get()
		{
			if (count == 0)
				throw new EmptyException();
			int val = tab[0], v = tab[--count];
			int i = 1, k = 2;
			while (k <= count)
			{
				if (k + 1 <= count && tab[k] > tab[k - 1])
					k++;
				if (tab[k - 1] > v)
				{
					tab[i - 1] = tab[k - 1];
					i = k;
					k <<= 1;
				}
				else
					break;
			}
			tab[i - 1] = v;
			return val;
		}

		public int Peek()
		{
			if (count == 0)
				throw new EmptyException();
			return tab[0]; 
		}

		public int Count => count;

		public int Size => tab.Length;

    } // class HeapPriorityQueue

} // namespace ASD
