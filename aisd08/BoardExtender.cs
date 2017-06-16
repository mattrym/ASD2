using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGreedyFish
{
    static class BoardExtender
    {
        /// <summary>
        /// Wyznacza ruchy i liczbę ryb zebranych dla algorytmu zachłannego (ruch wykonywany jest do najbliższego najlepszego pola [takiego z największą liczbą ryb])
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moves">Tablica ruchów wykonanych przez pingwiny</param>
        /// <returns>Liczba ryb zebrana przez pingwiny</returns>
        public static int GreedyAlgorithm1(this Board board, out Move[] moves)
        {
			moves = null;
			bool moved = true;
			int allFishes = 0;
			List<Move> bestMoves = new List<Move>();

			if (board.penguins == null)
				return 0;

			while(moved)
			{
				moved = false;
				int maxFishes = 0;
				Move bestMove = null;

				for (int id = 0; id < board.penguins.Count; ++id)
				{
					Point penguin = board.penguins.ElementAt(id);
					int fish = board.GetGridAt(penguin);
					for (int dir = 0; dir < 6; ++dir)
					{
						Point next = board.GetNeighbour(penguin.x, penguin.y, dir);
						while (next.x != -1 && next.y != -1 && board.GetGridAt(next) > 0 
							&& !board.IsPenguinAtField(next.x, next.y))
						{
							if(maxFishes < board.GetGridAt(next))
							{
								moved = true;
								maxFishes = board.GetGridAt(next);
								bestMove = new Move(penguin, next, fish, id);
							}
							next = board.GetNeighbour(next.x, next.y, dir);
						}
					}
				}
				if (moved)
				{
					allFishes += bestMove.fish;
					bestMoves.Add(bestMove);
					board.MovePenguin(bestMove.penguinId, bestMove.to.x, bestMove.to.y);
				}
			}

			if (bestMoves.Count <= 0)
				return 0;

			moves = bestMoves.ToArray();
			return allFishes; //zwrócona liczba ryb
        }

        /// <summary>
        /// Wyznacza ruchy i liczbę ryb zebranych dla algorytmu zachłannego (ruch wykonywany jest do końca kierunku, aż do napotkania "dziury" lub końca siatki)
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moves">Tablica ruchów wykonanych przez pingwiny</param>
        /// <returns>Liczba ryb zebrana przez pingwiny</returns>
        public static int GreedyAlgorithm2(this Board board, out Move[] moves)
		{
			moves = null;
			bool moved = true;
			int allFishes = 0;
			List<Move> bestMoves = new List<Move>();

			if (board.penguins == null)
				return 0;

			while (moved)
			{
				moved = false;
				int maxFishes = 0;
				Move bestMove = null;

				for (int id = 0; id < board.penguins.Count; ++id)
				{
					Point penguin = board.penguins.ElementAt(id);
					int fish = board.GetGridAt(penguin);
					for (int dir = 0; dir < 6; ++dir)
					{
						Point prev = new Point(-1, -1);
						Point next = board.GetNeighbour(penguin.x, penguin.y, dir);
						while (next.x != -1 && next.y != -1 && board.GetGridAt(next) > 0
							&& !board.IsPenguinAtField(next.x, next.y))
						{
							prev = next;
							next = board.GetNeighbour(next.x, next.y, dir);
						}
						if (prev.x != -1 && prev.y != -1 && maxFishes < board.GetGridAt(prev))
						{
							moved = true;
							maxFishes = board.GetGridAt(prev);
							bestMove = new Move(penguin, prev, fish, id);
						}
					}
				}
				if (moved)
				{
					allFishes += bestMove.fish;
					bestMoves.Add(bestMove);
					board.MovePenguin(bestMove.penguinId, bestMove.to.x, bestMove.to.y);
				}
			}

			if (bestMoves.Count <= 0)
				return 0;

			moves = bestMoves.ToArray();
			return allFishes; //zwrócona liczba ryb
		}
	}
}