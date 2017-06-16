using System;
using System.Collections.Generic;

namespace discs
{
    public struct Point
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; private set; }
        public double Y { get; private set; }

        public override string ToString()
        {
            return string.Format("[{0};{1}]", X, Y);
        }


        public bool IsRightOf(Point b)
        {
            return (this.X > b.X || (this.X == b.X && this.Y > b.Y));
        }
    }
    
    public enum IntersectionType
    {
        Disjoint,
        Contains,
        IsContained,
        Identical,
        Touches,
        Crosses
    }

    public struct Disk
    {
        public Point Center { get; private set; }
        public double Radius { get; private set; }

        public Disk(Point center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        public bool Contains(Point p)
        {
            return (p.X - Center.X) * (p.X - Center.X) + (p.Y - Center.Y) * (p.Y - Center.Y) <= Radius * Radius + Program.epsilon;
        }

        /// <summary>
        ///  Funkcja sprawdza wzajemne położenie dwóch kół.
        /// </summary>
        /// <param name="other">drugie koło</param>
        /// <param name="crossingPoints">
        /// Punkty przecięcia obwodów kół, jeśli zwracana jest wartość: Touches albo Crosses.
        /// Pusta tablica wpp.
        /// <returns>
        /// Disjoint - kiedy koła nie mają punktów wspólnych
        /// Contains - kiedy pierwsze koło całkowicie zawiera drugie
        /// IsContained - kiedy pierwsze koło jest całkowicie zawarte w drugim
        /// Identical - kiedy koła pokrywają się
        /// Touches - kiedy koła mają dokładnie jeden punkt wspólny
        /// Crosses - kiedy obwody kół mają dokładnie dwa punkty wspólne
        /// </returns>
        public IntersectionType GetIntersectionType(Disk other, out Point[] crossingPoints)
        {
            double dX = other.Center.X - this.Center.X;
            double dY = other.Center.Y - this.Center.Y;
            double dist2 = dX * dX + dY * dY;
            double dist = Math.Sqrt(dist2);

			/*
             * tu zajmij się wszystkimi przypadkami wzajemnego położenia kół,
             * oprócz Crosses i Touches
             */

			crossingPoints = new Point[0];

			if (Math.Abs(Center.X - other.Center.X) < Program.epsilon 
				&& Math.Abs(Center.Y - other.Center.Y) < Program.epsilon 
				&& Math.Abs(Radius - other.Radius) < Program.epsilon)
				return IntersectionType.Identical;

			if (dist + other.Radius <= Radius + Program.epsilon)
				return IntersectionType.Contains;

			if (dist + Radius <= other.Radius + Program.epsilon)
				return IntersectionType.IsContained;

			if (other.Radius + Radius + Program.epsilon < dist)
				return IntersectionType.Disjoint;

			// odległość od środka aktualnego koła (this) do punktu P,
			// który jest punktem przecięcia odcinka łączącego środki kół (this i other)
			// z odcinkiem łączącym punkty wspólne obwodów naszych kół
			double a = (this.Radius * this.Radius - other.Radius * other.Radius + dist2) / (2 * dist);

            // odległość punktów przecięcia obwodów do punktu P
            double h = Math.Sqrt(this.Radius * this.Radius - a * a);

            // punkt P
            double px = this.Center.X + (dX * a / dist);
            double py = this.Center.Y + (dY * a / dist);

            /*
             * teraz wiesz już wszystko co potrzebne do rozpoznania położenia Touches
             * zajmij się tym
             */

			if (h < Program.epsilon)
			{
				crossingPoints = new Point[1];
				crossingPoints[0] = new Point(px, py);
				return IntersectionType.Touches;
			}

            // przypadek Crosses - dwa punkty przecięcia - już jest zrobiony

            double rX = -dY * h / dist;
            double rY = dX * h / dist;
            
            crossingPoints = new Point[2];
            crossingPoints[0] = new Point(px + rX, py + rY);
            crossingPoints[1] = new Point(px - rX, py - rY);
            return IntersectionType.Crosses;
        }


		/*
         * dopisz wszystkie inne metody, które uznasz za stosowne         
         * 
         */

		public Point? MaxRightCommonPoint(Disk other)
		{
			Point[] crossingPoints, rightPoints = new Point[2];
			rightPoints[0] = new Point(Center.X + Radius, Center.Y);
			rightPoints[1] = new Point(other.Center.X + other.Radius, other.Center.Y);

			switch (GetIntersectionType(other, out crossingPoints))
			{
				case IntersectionType.Disjoint:
					return null;
				case IntersectionType.Identical:
					return rightPoints[0];
				case IntersectionType.IsContained:
					return rightPoints[0];
				case IntersectionType.Contains:
					return rightPoints[1];
				case IntersectionType.Touches:
					return crossingPoints[0];
			}

			if (other.Contains(rightPoints[0]))
				return rightPoints[0];
			if (Contains(rightPoints[1]))
				return rightPoints[1];
			return crossingPoints[0].X > crossingPoints[1].X ? crossingPoints[0] : crossingPoints[1];
		}

	}

    static class IntersectionFinder
    {

        public static Point? FindCommonPoint(Disk[] disks)
        {
			/*
			 * uzupełnij
			 */
			List<Point> intersectionPoints = new List<Point>();
			for (int i = 0; i < disks.Length; ++i)
				for (int j = 0; j < disks.Length; ++j)
				{
					Point? p = disks[i].MaxRightCommonPoint(disks[j]);
					if (!p.HasValue)
						return null;
					intersectionPoints.Add(p.Value);
				}

			Point maxLeft = intersectionPoints[0];
			foreach (Point p in intersectionPoints)
				if (maxLeft.IsRightOf(p))
					maxLeft = p;
			foreach (Disk d in disks)
				if (!d.Contains(maxLeft))
					return null;
			return maxLeft;
		}

    }
}
