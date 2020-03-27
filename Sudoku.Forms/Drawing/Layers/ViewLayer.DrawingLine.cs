namespace Sudoku.Drawing.Layers
{
	public sealed partial class ViewLayer
	{
		private readonly struct DrawingLine
		{
			public DrawingLine(int startX, int startY, int endX, int endY) =>
				(StartX, StartY, EndX, EndY) = (startX, startY, endX, endY);


			public int StartX { get; }

			public int StartY { get; }

			public int EndX { get; }

			public int EndY { get; }


			public bool Overlaps(DrawingLine other)
			{
				return DistanceUnscaled(other.StartX, other.StartY) == 0
					&& DistanceUnscaled(other.EndX, other.EndY) == 0
					? IntervalOverlaps(StartX, EndX, other.StartX, other.EndX)
						|| IntervalOverlaps(StartY, EndY, other.StartY, other.EndY)
					: false;
			}

			private int DistanceUnscaled(int px, int py)
			{
				// Vectorial product, without normalization by length.
				return (px - StartX) * (EndY - StartY) - (py - StartY) * (EndX - StartX);
			}

			private bool IntervalOverlaps(int s1, int e1, int s2, int e2)
			{
				if (s1 > e1)
				{
					s1 ^= e1 ^= s1 ^= e1;
				}
				if (s2 > e2)
				{
					s2 ^= e2 ^= s2 ^= e2;
				}

				return s1 < e2 && e1 > s2;
			}
		}
	}
}
