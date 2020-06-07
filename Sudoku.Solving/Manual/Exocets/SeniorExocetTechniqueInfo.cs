using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

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
		/// <param name="targetEliminations">The target eliminations.</param>
		/// <param name="trueBaseEliminations">The true base eliminations.</param>
		/// <param name="mirrorEliminations">The mirror eliminations.</param>
		/// <param name="compatibilityEliminations">The compatibility eliminations.</param>
		public SeniorExocetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Exocet exocet,
			IEnumerable<int> digits, int endoTargetCell, TargetEliminations targetEliminations,
			TrueBaseEliminations trueBaseEliminations, MirrorEliminations mirrorEliminations,
			CompatibilityTestEliminations compatibilityEliminations)
			: base(
				  conclusions, views, exocet, digits, TechniqueCode.Se, null, null,
				  targetEliminations, mirrorEliminations, default, default, default,
				  trueBaseEliminations, compatibilityEliminations) =>
			EndoTargetCell = endoTargetCell;


		/// <summary>
		/// Indicates the target cell that lies on the cross-line.
		/// </summary>
		public int EndoTargetCell { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M;


		/// <inheritdoc/>
		protected override string? GetAdditional() =>
			$"Endo-target: {new CellCollection(stackalloc[] { EndoTargetCell }).ToString()}";
	}
}
