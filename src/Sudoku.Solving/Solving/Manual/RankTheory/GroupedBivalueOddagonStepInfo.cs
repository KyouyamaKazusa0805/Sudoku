using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Provides a usage of <b>grouped bi-value oddagon</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Loop">The loop used.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="ExtraDigits">The extra digits.</param>
	public sealed record GroupedBivalueOddagonStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Cells Loop, int Digit1, int Digit2, short ExtraDigits
	) : RankTheoryStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.3M;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => TechniqueTags.RankTheory;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.BivalueOddagon;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.GroupedBivalueOddagon;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string LoopStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Loop.ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string Digit1Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit1 + 1).ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string Digit2Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit2 + 1).ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string ExtraDigitsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(ExtraDigits).ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string ElimStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ConclusionCollection(Conclusions).ToString();
		}


		/// <inheritdoc/>
		public override string ToString() =>
			$"The cells {LoopStr} about digits {Digit1Str} and {Digit2Str} will form an invalid bi-value oddagon structure to make the puzzle no solution when all positions about a digit in digits {ExtraDigitsStr} in this structure is removed => {ElimStr}";
	}
}
