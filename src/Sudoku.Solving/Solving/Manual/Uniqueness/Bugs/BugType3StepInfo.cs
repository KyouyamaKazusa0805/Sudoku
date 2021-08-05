using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Resources;
using Sudoku.Techniques;
using static Sudoku.Solving.Manual.Constants;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 3 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="TrueCandidates">All true candidates.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="IsNaked">Indicates whether the subset is naked.</param>
	public sealed record BugType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		IReadOnlyList<int> TrueCandidates, IReadOnlyList<int> Digits,
		IReadOnlyList<int> Cells, bool IsNaked
	) : BugStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + Digits.Count * .1M + (IsNaked ? 0 : .1M);

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugType3;

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string TrueCandidatesStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Candidates(TrueCandidates).ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string SubsetTypeStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => IsNaked ? TextResources.Current.NakedKeyword : TextResources.Current.HiddenKeyword;
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string SizeStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => SubsetNames[Digits.Count].ToLower(null);
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string SizeStrZhCn
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => TextResources.Current[$"SubsetNamesSize{Digits.Count.ToString()}"];
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string ExtraDigitsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(Digits).ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string CellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells(Cells).ToString();
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
			$"{Name}: True candidates {TrueCandidatesStr} with {SubsetTypeStr} {SizeStr} {ExtraDigitsStr} in cells {CellsStr} => {ElimStr}";
	}
}
