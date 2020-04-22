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
		/// <param name="mirrorEliminations">The mirror eliminations.</param>
		public JuniorExocetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Exocet exocet,
			IEnumerable<int> digits, MirrorEliminations? mirrorEliminations)
			: base(conclusions, views, exocet, digits, ExocetTypeCode.Junior, mirrorEliminations)
		{
		}


		/// <inheritdoc/>
		public override decimal Difficulty => 9.4M;


		/// <inheritdoc/>
		protected override string? GetAdditional() => null;
	}
}
