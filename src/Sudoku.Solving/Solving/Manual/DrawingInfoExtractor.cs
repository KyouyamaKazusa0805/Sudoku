using Sudoku.Solving.Manual.Chaining;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides extension methods on <see cref="Node"/>, about extraction the <see cref="DrawingInfo"/>s.
	/// </summary>
	/// <seealso cref="Node"/>
	/// <seealso cref="DrawingInfo"/>
	public static class DrawingInfoExtractor
	{
		/// <summary>
		/// Get highlight candidate offsets through the specified target node.
		/// </summary>
		/// <param name="target">The target node.</param>
		/// <param name="simpleChain">
		/// Indicates whether the current caller is in <see cref="AicStepSearcher"/>. The
		/// default value is <see langword="false"/>.
		/// </param>
		/// <returns>The candidate offsets.</returns>
		public static IList<DrawingInfo> GetCandidateOffsets(this in Node target, bool simpleChain = false)
		{
			var result = new List<DrawingInfo>();
			var chain = target.Chain;
			var (pCandidate, _) = chain[0];
			if (!simpleChain)
			{
				result.Add(new(2, pCandidate));
			}

			for (int i = 0, count = chain.Count; i < count; i++)
			{
				var p = chain[i];
				if (p.Parents is { } parents)
				{
					foreach (var pr in parents)
					{
						var (prCandidate, prIsOn) = pr;
						if (simpleChain && i != count - 2 || !simpleChain)
						{
							result.AddIfDoesNotContain(new(prIsOn ? 1 : 0, prCandidate));
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Get the links through the specified target node.
		/// </summary>
		/// <param name="this">The target node.</param>
		/// <param name="showAllLinks">
		/// Indicates whether the current chain will display all chains (even contains the weak links
		/// from the elimination node). The default value is <see langword="false"/>. If you want to
		/// draw the AIC, the elimination weak links don't need drawing.
		/// </param>
		/// <returns>The link.</returns>
		public static IReadOnlyList<Link> GetLinks(this in Node @this, bool showAllLinks = false)
		{
			var result = new List<Link>();
			var chain = @this.Chain;
			for (int i = showAllLinks ? 0 : 1, count = chain.Count - (showAllLinks ? 0 : 2); i < count; i++)
			{
				var p = chain[i];
				var (pCandidate, pIsOn) = p;
				for (int j = 0, parentsCount = p.Parents?.Count ?? 0; j < parentsCount; j++)
				{
					var (prCandidate, prIsOn) = p.Parents![j];
					result.Add(
						new(
							pCandidate,
							prCandidate,
							(A: prIsOn, B: pIsOn) switch
							{
								(A: false, B: true) => LinkType.Strong,
								(A: true, B: false) => LinkType.Weak,
								_ => LinkType.Default
							}
						)
					);
				}
			}

			return result;
		}
	}
}
