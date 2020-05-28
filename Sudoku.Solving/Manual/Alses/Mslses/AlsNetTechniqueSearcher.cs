using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Encapsulates a <b>multi-sector locked sets</b> (MSLS) technique. This searcher is
	/// the real technique, different with the abstract class <see cref="MslsTechniqueSearcher"/>.
	/// </summary>
	/// <seealso cref="MslsTechniqueSearcher"/>
	[TechniqueDisplay("Multi-sector Locked Sets")]
	public sealed partial class AlsNetTechniqueSearcher : MslsTechniqueSearcher
	{
		/// <summary>
		/// Indicates the list initialized with the static constructor.
		/// </summary>
		private static readonly GridMap[] Patterns = new GridMap[78975];


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 50;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{

		}
	}
}
