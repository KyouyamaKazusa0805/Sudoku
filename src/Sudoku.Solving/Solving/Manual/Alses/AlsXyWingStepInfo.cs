using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>almost locked sets XY-Wing</b> (ALS-XY-Wing) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Als1">The ALS 1.</param>
	/// <param name="Als2">The ALS 2.</param>
	/// <param name="Bridge">The bridge ALS.</param>
	/// <param name="XDigitsMask">The X digits mask.</param>
	/// <param name="YDigitsMask">The Y digits mask.</param>
	/// <param name="ZDigitsMask">The Z digits mask.</param>
	public sealed record AlsXyWingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Als Als1, in Als Als2,
		in Als Bridge, short XDigitsMask, short YDigitsMask, short ZDigitsMask
	) : AlsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 6.0M;

		/// <inheritdoc/>
		public override string? Acronym => "ALS-XY-Wing";

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlsChainingLike;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.AlsXyWing;

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
		private string BridgeStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Bridge.ToString();
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
		private string XStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(XDigitsMask).ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string YStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(YDigitsMask).ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string ZStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(ZDigitsMask).ToString();
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
			$"{Name}: {Als1Str} -> {BridgeStr} -> {Als2Str}, X = {XStr}, Y = {YStr}, Z = {ZStr} => {ElimStr}";
	}
}
