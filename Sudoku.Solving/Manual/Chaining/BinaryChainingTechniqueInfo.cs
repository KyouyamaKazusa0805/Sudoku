using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

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
	public sealed record BinaryChainingTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Node SourceNode, in Node FromOnNode, in Node FromOffNode, bool IsAbsurd, bool IsMultiple,
		bool IsNishio, int Level)
		: ChainingTechniqueInfo(Conclusions, Views, true, true, IsNishio, IsMultiple, true, Level)
	{
		/// <inheritdoc/>
		public override ChainingTypeCode SortKey =>
			IsAbsurd ? ChainingTypeCode.DynamicContradictionFc : ChainingTypeCode.DynamicDoubleFc;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		public override Node[] ChainsTargets => new[] { FromOnNode, FromOffNode };
#endif

		/// <summary>
		/// Indicates the anchor.
		/// </summary>
		public Node Anchor =>
			IsNishio || IsAbsurd ? new(SourceNode.Cell, SourceNode.Digit, !SourceNode.IsOn) : FromOnNode;

		/// <inheritdoc/>
		public override int FlatComplexity => FromOnNode.AncestorsCount + FromOffNode.AncestorsCount;

		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + LengthDifficulty;


		/// <inheritdoc/>
		public override string ToString() =>
			$"{Name}: It can be proved to be a contradiction if " +
			$"{new SudokuMap { Anchor.Cell * 9 + Anchor.Digit }} is " +
			$"{(!Anchor.IsOn).ToString().ToLower()} => {new ConclusionCollection(Conclusions).ToString()}";
	}
}
