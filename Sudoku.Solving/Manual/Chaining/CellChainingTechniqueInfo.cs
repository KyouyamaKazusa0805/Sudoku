using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>cell forcing chains</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="SourceCell">The source cell.</param>
	/// <param name="Chains">All branches.</param>
	public sealed record CellChainingTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int SourceCell, IReadOnlyDictionary<int, Node> Chains)
		: ChainingTechniqueInfo(Conclusions, Views, default, default, default, true, default, default)
	{
		/// <inheritdoc/>
		public override string Name => Resources.GetValue(TechniqueCode.ToString());

		/// <inheritdoc/>
		public override ChainingTypeCode SortKey => ChainingTypeCode.CellFc;

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
			string srcCellStr = new GridMap { SourceCell }.ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: It can be proved using chains that all digits are false from {srcCellStr} => {elimStr}";
		}
	}
}
