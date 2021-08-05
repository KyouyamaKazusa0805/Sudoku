using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets W-Wing</b> (ALS-W-Wing) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Als1">The ALS 1.</param>
	/// <param name="Als2">The ALS 2.</param>
	/// <param name="ConjugatePair">The conjugate pair.</param>
	/// <param name="WDigitsMask">The W digit mask.</param>
	/// <param name="X">The digit X.</param>
	public sealed record AlsWWingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Als Als1, in Als Als2,
		in ConjugatePair ConjugatePair, short WDigitsMask, int X
	) : AlsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 6.2M;

		/// <inheritdoc/>
		public override string? Acronym => "ALS-W-Wing";

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlsChainingLike;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.AlsWWing;

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string Als1Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Als1.ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string Als2Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Als2.ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string ConjStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugatePair.ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string WStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(WDigitsMask).ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string XStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (X + 1).ToString();
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
			$"{Name}: Two ALSes {Als1Str}, {Als2Str} connected by {ConjStr}, W = {WStr}, X = {XStr} => {ElimStr}";
	}
}
