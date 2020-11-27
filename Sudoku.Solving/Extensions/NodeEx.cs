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
		/// <param name="target">(<see langword="this in"/> parameter) The target node.</param>
		/// <returns>The candidate offsets.</returns>
		public static IReadOnlyList<DrawingInfo> GetCandidateOffsets(this in Node target)
		{
			var result = new List<DrawingInfo>();
			var chain = target.Chain;
			for (int i = 0, count = chain.Count; i < count; i++)
			{
				var p = chain[i];
				foreach (var pr in p.Parents)
				{
					var linkType = (pr.IsOn, p.IsOn) switch
					{
						(false, true) => LinkType.Strong,
						(true, false) => LinkType.Weak,
						_ => LinkType.Default
					};
					if ((linkType, i) == (LinkType.Weak, 0))
					{
						// Because of forcing chain, the first node won't be drawn.
						continue;
					}

					result.Add(new(pr.IsOn ? 0 : 1, pr.Cell * 9 + pr.Digit));
				}

				result.Add(new(p.IsOn ? 0 : 1, p.Cell * 9 + p.Digit));
			}

			return result;
		}

		/// <summary>
		/// Get the links through the specified target node.
		/// </summary>
		/// <param name="target">(<see langword="this in"/> parameter) The target node.</param>
		/// <param name="showAllLinks">
		/// Indicates whether the current chain will display all chains (even contains the weak links
		/// from the elimination node). The default value is <see langword="false"/>. If you want to
		/// draw the AIC, the elimination weak links don't need drawing.
		/// </param>
		/// <returns>The link.</returns>
		public static IReadOnlyList<Link> GetLinks(this in Node target, bool showAllLinks = false)
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
							p.Cell * 9 + p.Digit,
							pr.Cell * 9 + pr.Digit,
							(pr.IsOn, p.IsOn) switch
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
