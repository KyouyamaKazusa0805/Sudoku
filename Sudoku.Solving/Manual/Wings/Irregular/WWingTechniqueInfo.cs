using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Provides a usage of <b>W-Wing</b> technique.
	/// </summary>
	public sealed class WWingTechniqueInfo : IrregularWingTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views.</param>
		/// <param name="startCellOffset">Start cell offset.</param>
		/// <param name="endCellOffset">End cell offsets.</param>
		/// <param name="conjugatePair">The conjugate pair.</param>
		public WWingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int startCellOffset, int endCellOffset, ConjugatePair conjugatePair)
			: base(conclusions, views) =>
			(StartCell, EndCell, ConjugatePair) = (startCellOffset, endCellOffset, conjugatePair);


		/// <summary>
		/// Indicates the start cell offset.
		/// </summary>
		public int StartCell { get; }

		/// <summary>
		/// Indicates the end cell offset.
		/// </summary>
		public int EndCell { get; }

		/// <summary>
		/// Indicates the conjugate pair.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 4.4M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.WWing;


		/// <inheritdoc/>
		public override string ToString()
		{
			string startCellStr = new CellCollection(stackalloc[] { StartCell }).ToString();
			string endCellStr = new CellCollection(stackalloc[] { EndCell }).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {startCellStr} to {endCellStr} with conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
