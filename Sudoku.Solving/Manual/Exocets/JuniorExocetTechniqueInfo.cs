using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>junior exocet</b> (JE) technique.
	/// </summary>
	public sealed class JuniorExocetTechniqueInfo : ExocetTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="exocet">The exocet.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="lockedMemberQ">The locked member Q.</param>
		/// <param name="lockedMemberR">The locked member R.</param>
		/// <param name="targetEliminations">The target eliminations.</param>
		/// <param name="mirrorEliminations">The mirror eliminations.</param>
		/// <param name="bibiEliminations">The Bi-bi pattern eliminations.</param>
		public JuniorExocetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Exocet exocet,
			IEnumerable<int> digits, IEnumerable<int>? lockedMemberQ, IEnumerable<int>? lockedMemberR,
			TargetEliminations targetEliminations,
			MirrorEliminations mirrorEliminations, BibiPatternEliminations bibiEliminations)
			: base(
				  conclusions, views, exocet, digits, ExocetTypeCode.Junior,
				  lockedMemberQ, lockedMemberR, targetEliminations, mirrorEliminations, bibiEliminations)
		{
		}


		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return 9.4M
					+ (MirrorEliminations.Conclusions is null ? 0 : .1M)
					+ (BibiEliminations.Conclusions is null ? 0 : .3M);
			}
		}


		/// <inheritdoc/>
		protected override string? GetAdditional() => null;
	}
}
