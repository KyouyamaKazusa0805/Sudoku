using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Manual.Exocets.Eliminations;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>junior exocet</b> (JE) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Exocet">The exocet.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="LockedMemberQ">The locked member Q.</param>
	/// <param name="LockedMemberR">The locked member R.</param>
	/// <param name="TargetEliminations">The target eliminations.</param>
	/// <param name="MirrorEliminations">The mirror eliminations.</param>
	/// <param name="BibiEliminations">The Bi-bi pattern eliminations.</param>
	/// <param name="TargetPairEliminations">The target pair eliminations.</param>
	/// <param name="SwordfishEliminations">The swordfish pattern eliminations.</param>
	public sealed record JuniorExocetTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, Pattern Exocet,
		IEnumerable<int> Digits, IEnumerable<int>? LockedMemberQ, IEnumerable<int>? LockedMemberR,
		TargetEliminations TargetEliminations, MirrorEliminations MirrorEliminations,
		BibiPatternEliminations BibiEliminations, TargetPairEliminations TargetPairEliminations,
		SwordfishEliminations SwordfishEliminations)
		: ExocetTechniqueInfo(
			Conclusions, Views, Exocet, Digits, TechniqueCode.Je,
			LockedMemberQ, LockedMemberR, TargetEliminations, MirrorEliminations,
			BibiEliminations, TargetPairEliminations, SwordfishEliminations, default, default)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			9.4M +
			(MirrorEliminations.Conclusions is null ? 0 : .1M) +
			(BibiEliminations.Conclusions is null ? 0 : .3M) +
			(TargetPairEliminations.Conclusions is null ? 0 : .1M) +
			(SwordfishEliminations.Conclusions is null ? 0 : .2M);


		/// <inheritdoc/>
		protected override string? GetAdditional() => null;
	}
}
