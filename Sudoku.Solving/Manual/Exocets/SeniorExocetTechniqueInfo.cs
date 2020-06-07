using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Manual.Exocets.Eliminations;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>senior exocet</b> (SE) technique.
	/// </summary>
	public sealed class SeniorExocetTechniqueInfo : ExocetTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="exocet">The exocet.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="endoTargetCell">The endo target cell.</param>
		/// <param name="extraRegionsMask">The extra regions mask.</param>
		/// <param name="targetEliminations">The target eliminations.</param>
		/// <param name="trueBaseEliminations">The true base eliminations.</param>
		/// <param name="mirrorEliminations">The mirror eliminations.</param>
		/// <param name="compatibilityEliminations">The compatibility eliminations.</param>
		public SeniorExocetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Exocet exocet,
			IEnumerable<int> digits, int endoTargetCell, int[]? extraRegionsMask, TargetEliminations targetEliminations,
			TrueBaseEliminations trueBaseEliminations, MirrorEliminations mirrorEliminations,
			CompatibilityTestEliminations compatibilityEliminations)
			: base(
				  conclusions, views, exocet, digits, TechniqueCode.Se, null, null,
				  targetEliminations, mirrorEliminations, default, default, default,
				  trueBaseEliminations, compatibilityEliminations) =>
			(EndoTargetCell, ExtraRegionsMask) = (endoTargetCell, extraRegionsMask);


		/// <summary>
		/// Indicates whether the specified instance contains any extra regions.
		/// </summary>
		public bool ContainsExtraRegions => ExtraRegionsMask?.Any(m => m != 0) ?? false;

		/// <summary>
		/// Indicates the extra regions mask.
		/// </summary>
		public int[]? ExtraRegionsMask { get; }

		/// <summary>
		/// Indicates the target cell that lies on the cross-line.
		/// </summary>
		public int EndoTargetCell { get; }

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => ContainsExtraRegions ? TechniqueCode.ComplexSe : TechniqueCode.Se;

		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M + (ContainsExtraRegions ? 0 : .2M);


		/// <inheritdoc/>
		protected override string? GetAdditional()
		{
			const string separator = ", ";
			string endoTargetStr = $"Endo-target: {new CellCollection(EndoTargetCell).ToString()}";
			if (!(ExtraRegionsMask is null))
			{
				var sb = new StringBuilder();
				int count = 0;
				for (int digit = 0; digit < 9; digit++)
				{
					int mask = ExtraRegionsMask[digit];
					if (mask != 0)
					{
						sb.Append($"{digit + 1}{new RegionCollection(mask.GetAllSets()).ToString()}{separator}");
						count++;
					}
				}

				if (count != 0)
				{
					return $"{endoTargetStr}, and extra regions will be included: {sb.RemoveFromEnd(separator.Length)}";
				}
			}

			return endoTargetStr;
		}
	}
}
