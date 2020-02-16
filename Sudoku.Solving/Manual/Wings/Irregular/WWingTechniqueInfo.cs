using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Provides a usage of W-Wing technique.
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
			(StartCellOffset, EndCellOffset, ConjugatePair) = (startCellOffset, endCellOffset, conjugatePair);


		/// <summary>
		/// Indicates the start cell offset.
		/// </summary>
		public int StartCellOffset { get; }

		/// <summary>
		/// Indicates the end cell offset.
		/// </summary>
		public int EndCellOffset { get; }

		/// <summary>
		/// Indicates the conjugate pair.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }

		/// <inheritdoc/>
		public override string Name => "W-Wing";

		/// <inheritdoc/>
		public override decimal Difficulty => 4.4m;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string startCellStr = CellUtils.ToString(StartCellOffset);
			string endCellStr = CellUtils.ToString(EndCellOffset);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {startCellStr} to {endCellStr} with conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
