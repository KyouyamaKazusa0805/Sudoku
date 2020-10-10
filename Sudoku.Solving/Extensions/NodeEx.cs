using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Node"/>.
	/// </summary>
	/// <seealso cref="Node"/>
	public static class NodeEx
	{
		/// <summary>
		/// Get highlight candidate offsets through the specified target node.
		/// </summary>
		/// <param name="target">(<see langword="this"/> parameter) The target node.</param>
		/// <returns>The candidate offsets.</returns>
		public static IReadOnlyList<DrawingInfo> GetCandidateOffsets(this Node target)
		{
			var result = new List<DrawingInfo>();
			var map = new HashSet<(int, bool)>();
			var chain = target.Chain;
			for (int i = 0, count = chain.Count; i < count; i++)
			{
				if (chain[i] is { ParentsCount: not 0 } p)
				{
					var pr = p[0];
					var linkType = (pr.IsOn, p.IsOn) switch
					{
						(false, true) => LinkType.Strong,
						(true, false) => LinkType.Weak,
						_ => LinkType.Default
					};
					if (linkType == LinkType.Weak && i == 0)
					{
						// Because of forcing chain, the first node won't be drawn.
						continue;
					}

					map.Add((p.Cell * 9 + p.Digit, p.IsOn));
				}
			}

			foreach (var (candidate, isOn) in map)
			{
				result.Add(new(isOn ? 0 : 1, candidate));
			}

			return result;
		}

		/// <summary>
		/// Get the links through the specified target node.
		/// </summary>
		/// <param name="target">(<see langword="this"/> parameter) The target node.</param>
		/// <param name="showAllLinks">
		/// Indicates whether the current chain will display all chains (even contains the weak links
		/// from the elimination node). The default value is <see langword="false"/>. If you want to
		/// draw the AIC, the elimination weak links don't need drawing.
		/// </param>
		/// <returns>The link.</returns>
		public static IReadOnlyList<Link> GetLinks(this Node target, bool showAllLinks = false)
		{
			var result = new List<Link>();
			var chain = target.Chain;
			for (int i = showAllLinks ? 0 : 1, count = chain.Count - (showAllLinks ? 0 : 2); i < count; i++)
			{
				var p = chain[i];
				for (int j = 0; j < p.ParentsCount; j++)
				{
					var pr = p[j];
					result.Add(
						new(
							startCandidate: p.Cell * 9 + p.Digit,
							endCandidate: pr.Cell * 9 + pr.Digit,
							linkType: (pr.IsOn, p.IsOn) switch
							{
								(false, true) => LinkType.Strong,
								(true, false) => LinkType.Weak,
								_ => LinkType.Default
							}));
				}
			}

			return result;
		}
	}
}
