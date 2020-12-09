using System.Collections.Generic;
using System.Extensions;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Provides a usage of <b>Gurth's symmetrical placement</b> (GSP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="SymmetryType">The symmetry type used.</param>
	/// <param name="MappingTable">The mapping table.</param>
	public sealed record GspStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, SymmetryType SymmetryType,
		int?[] MappingTable) : SymmetryStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override bool ShowDifficulty => false;

		/// <inheritdoc/>
		public override decimal Difficulty => 7.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Gsp;


		/// <inheritdoc/>
		public override string ToString()
		{
			const string separator = ", ";

			string conclusions = new ConclusionCollection(Conclusions).ToString();
			var sb = new StringBuilder();
			for (int i = 0; i < 9; i++)
			{
				int? value = MappingTable[i];
				sb
					.Append(i + 1)
					.Append(value.HasValue && value != i ? $" -> {value.Value + 1}" : string.Empty)
					.Append(separator);
			}

			string customName = SymmetryType.GetName().ToLower();
			string mapping = sb.RemoveFromEnd(separator.Length).ToString();
			return $"{Name}: Symmetry type: {customName}, mapping relations: {mapping} => {conclusions}";
		}
	}
}
