using System;

#if SOLUTION_WIDE_CODE_ANALYSIS
using System.Diagnostics.CodeAnalysis;
#endif

namespace Sudoku.Solving.Manual.Fishes
{
	partial record ComplexFishStepInfo
	{
		/// <summary>
		/// Indicates the shape modifiers.
		/// </summary>
		[Flags]
#if SOLUTION_WIDE_CODE_ANALYSIS
		[Closed]
#endif
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
