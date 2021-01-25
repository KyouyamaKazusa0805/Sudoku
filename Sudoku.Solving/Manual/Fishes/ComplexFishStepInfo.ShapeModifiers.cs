using System;

namespace Sudoku.Solving.Manual.Fishes
{
	public sealed partial record ComplexFishStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit,
		IReadOnlyList<int> BaseSets, IReadOnlyList<int> CoverSets, in Cells Exofins,
		in Cells Endofins, bool IsFranken, bool? IsSashimi)
	{
		/// <summary>
		/// Indicates the shape modifiers.
		/// </summary>
		[Flags]
		private enum ShapeModifiers
		{
			/// <summary>
			/// Indicates the basic fish.
			/// </summary>
			Basic = 1,

			/// <summary>
			/// Indicates the franken fish.
			/// </summary>
			Franken = 2,

			/// <summary>
			/// Indicates the mutant fish.
			/// </summary>
			Mutant = 4
		}
	}
}
