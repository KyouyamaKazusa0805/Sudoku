namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Provides with extension methods on <see cref="ChainLinkViewNode"/> instances.
/// </summary>
/// <seealso cref="ChainLinkViewNode"/>
public static class ChainLinkViewNodeExtensions
{
	/// <summary>
	/// Determine whether the specified <see cref="ChainLinkViewNode"/> instance has already passed through the specified nodes.
	/// </summary>
	/// <param name="link">The link to be checked.</param>
	/// <param name="nodes">The nodes to be checked.</param>
	/// <param name="candidateNodes">The candidate view nodes.</param>
	/// <param name="conclusions">The conclusions.</param>
	/// <param name="grid">The <see cref="GridLayout"/> instance represented by creating point calculator instance.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public static bool IsPassedThrough(
		this ChainLinkViewNode link,
		ReadOnlySpan<ILinkViewNode> nodes,
		ReadOnlySpan<CandidateViewNode> candidateNodes,
		ReadOnlySpan<Conclusion> conclusions,
		GridLayout grid
	)
	{
		var converter = new SudokuPanePositionConverter(grid);
		var points = converter.GetPoints(nodes, candidateNodes, conclusions);
		var ((cellSize, _), _, _, _) = converter;
		if (link is not ILinkViewNode(_, var start, var end) { Shape: LinkShape.Chain })
		{
			return false;
		}

		var (pt1, pt2) = (default(Point), default(Point));
		var distance = double.MaxValue;
		var startCandidates = start switch { CandidateMap c => c, Candidate c => c.AsCandidateMap() };
		var endCandidates = end switch { CandidateMap c => c, Candidate c => c.AsCandidateMap() };
		foreach (var s in startCandidates)
		{
			var tempPoint1 = converter.GetPosition(s);
			foreach (var e in endCandidates)
			{
				var tempPoint2 = converter.GetPosition(e);
				var d = tempPoint1.DistanceTo(tempPoint2);
				if (d <= distance)
				{
					(distance, pt1, pt2) = (d, tempPoint1, tempPoint2);
				}
			}
		}

		var (deltaX, deltaY) = (pt2.X - pt1.X, pt2.Y - pt1.Y);
		var alpha = Atan2(deltaY, deltaX);
		adjust(pt1, pt2, alpha, cellSize, out var p1/*, out _*/);

		var (dx1, dy1) = (deltaX, deltaY);
		foreach (var point in points)
		{
			if (point == pt1 || point == pt2)
			{
				continue;
			}

			if ((point.X - p1.X, point.Y - p1.Y) is var (dx2, dy2)
				&& Sign(dx1) == Sign(dx2) && Sign(dy1) == Sign(dy2)
				&& Abs(dx2) <= Abs(dx1) && Abs(dy2) <= Abs(dy1)
				&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, epsilon: 1E-1)))
			{
				return true;
			}
		}
		return false;


		static void adjust(Point pt1, Point pt2, double alpha, double cs, out Point p1/*, out Point p2*/)
		{
			(p1, /*p2, */var tempDelta) = (pt1, /*pt2, */cs / 2);
			var (px, py) = (tempDelta * Cos(alpha), tempDelta * Sin(alpha));
			p1.X += px;
			p1.Y += py;
			//p2.X -= px;
			//p2.Y -= py;
		}
	}
}
