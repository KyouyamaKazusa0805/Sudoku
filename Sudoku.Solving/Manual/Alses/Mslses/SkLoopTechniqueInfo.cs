using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Provides a usage of <b>domino loop</b> technique.
	/// </summary>
	public sealed class SkLoopTechniqueInfo : MslsTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cells">All cells.</param>
		public SkLoopTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, IReadOnlyList<int> cells)
			: base(conclusions, views) => Cells = cells;


		/// <summary>
		/// The cells.
		/// </summary>
		public IReadOnlyList<int> Cells { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.SkLoop;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {Cells.Count} Cells {cellsStr} => {elimStr}";
		}
	}
}
