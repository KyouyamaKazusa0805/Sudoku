using System;
using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Checking;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Encapsulates a bivalue universal grave (BUG) technique searcher.
	/// </summary>
	public sealed class BivalueUniversalGraveTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var checker = new BugChecker(grid);
			var list = checker.TrueCandidates;
			if (list.Count == 0)
			{
				return Array.Empty<BivalueUniversalGraveTechniqueInfo>();
			}

			var result = new List<BivalueUniversalGraveTechniqueInfo>();

			if (list.Count == 1)
			{
				// BUG + 1 found.
				result.Add(
					new BivalueUniversalGraveTechniqueInfo(
						conclusions: new[] { new Conclusion(ConclusionType.Assignment, list[0]) },
						views: new[]
						{
							new View(
								cellOffsets: null,
								candidateOffsets: new[] { (0, list[0]) },
								regionOffsets: null,
								linkMasks: null)
						}));
			}
			else
			{

			}

			return result;
		}
	}
}
