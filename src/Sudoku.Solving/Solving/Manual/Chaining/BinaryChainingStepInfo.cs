using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Resources;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>contradiction forcing chains</b> and <b>double forcing chains</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="SourceNode">Indicates the source node.</param>
	/// <param name="FromOnNode">Indicates the node that is the destination (on side).</param>
	/// <param name="FromOffNode">Indicates the node that is the destination (off side).</param>
	/// <param name="IsAbsurd">Indicates whether the chain is absurd.</param>
	/// <param name="IsMultiple">Indicates whether the chain is multiple.</param>
	/// <param name="IsNishio">Indicates whether the chain is nishio.</param>
	/// <param name="Level">Indicates the dynamic level of the chain.</param>
	public sealed record BinaryChainingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Node SourceNode, in Node FromOnNode, in Node FromOffNode,
		bool IsAbsurd, bool IsMultiple, bool IsNishio, int Level
	) : ChainingStepInfo(Conclusions, Views, true, true, IsNishio, IsMultiple, true, Level)
	{
		/// <inheritdoc/>
		public override int FlatComplexity => FromOnNode.AncestorsCount + FromOffNode.AncestorsCount;

		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + LengthDifficulty;

		/// <summary>
		/// Indicates the anchor.
		/// </summary>
		public Node Anchor =>
			IsNishio || IsAbsurd ? new(SourceNode.Cell, SourceNode.Digit, !SourceNode.IsOn) : FromOnNode;

		/// <inheritdoc/>
		public override ChainingTypeCode SortKey =>
			IsAbsurd ? ChainingTypeCode.DynamicContradictionFc : ChainingTypeCode.DynamicDoubleFc;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => TechniqueTags.LongChaining | TechniqueTags.ForcingChains;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.Fc;

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string AnchorCandidateStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Candidates { Anchor.Cell * 9 + Anchor.Digit }.ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string AnchorIsTrueOrFalseStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Anchor.IsOn ? TextResources.Current.TrueKeyword : TextResources.Current.FalseKeyword;
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
			$"{Name}: It can be proved to be a contradiction if {AnchorCandidateStr} is {AnchorIsTrueOrFalseStr} => {ElimStr}";
	}
}
