using System.Collections.Generic;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Provides a usage of <b>domino loop</b> technique.
	/// </summary>
	public sealed class SkLoopTechniqueInfo : MslsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="cells">All cells.</param>
		public SkLoopTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, IReadOnlyList<int> cells)
			: base(conclusions, views) => Cells = cells;


		/// <summary>
		/// The cells.
		/// </summary>
		public IReadOnlyList<int> Cells { get; }

		/// <inheritdoc/>
		public override string Name => "Stephen Kurzhal's Loop";

		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {Cells.Count} Cells {cellsStr} => {elimStr}";
		}
	}
}
