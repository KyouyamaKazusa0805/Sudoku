using System.Collections.Generic;
using Sudoku.Data;
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
	/// <param name="IsNishio">Indicates whether the chain is nishio.</param>
	public sealed record BinaryChainingTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Node SourceNode, in Node FromOnNode, in Node FromOffNode, bool IsAbsurd, bool IsNishio)
		: ChainingTechniqueInfo(Conclusions, Views, true, true, default, default, default, default)
	{
		/// <inheritdoc/>
		public override ChainingTypeCode SortKey =>
			IsAbsurd ? ChainingTypeCode.ContradictionFc : ChainingTypeCode.DoubleFc;

		/// <summary>
		/// Indicates the anchor.
		/// </summary>
		public Node Anchor =>
			IsNishio || IsAbsurd ? new(SourceNode.Cell, SourceNode.Digit, !SourceNode.IsOn) : FromOnNode;

		/// <inheritdoc/>
		public override int FlatComplexity => FromOnNode.AncestorsCount + FromOffNode.AncestorsCount;

		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + LengthDifficulty;
	}
}
