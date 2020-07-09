using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>cell forcing chains</b> technique.
	/// </summary>
	public sealed class CellChainingTechniqueInfo : ChainingTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="sourceCell">The source cell.</param>
		/// <param name="chains">All branches.</param>
		public CellChainingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int sourceCell, IReadOnlyDictionary<int, Node> chains)
			: base(conclusions, views, default, default, default, isMultiple: true, default, default) =>
			(SourceCell, Chains) = (sourceCell, chains);


		/// <summary>
		/// Indicates the source cell.
		/// </summary>
		public int SourceCell { get; }

		/// <summary>
		/// Indicates the branches.
		/// </summary>
		public IReadOnlyDictionary<int, Node> Chains { get; }

		/// <inheritdoc/>
		public override string Name => Resources.GetValue(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override int SortKey => 5;

		/// <inheritdoc/>
		public override int FlatComplexity
		{
			get
			{
				int result = 0;
				foreach (var node in Chains.Values)
				{
					result += node.AncestorsCount;
				}

				return result;
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + LengthDifficulty;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;


		/// <inheritdoc/>
		public override string ToString()
		{
			string srcCellStr = new CellCollection(SourceCell).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: It can be proved using chains that all digits are false from {srcCellStr} => {elimStr}";
		}
	}
}
