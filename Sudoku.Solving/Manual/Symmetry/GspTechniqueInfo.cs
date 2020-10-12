#pragma warning disable IDE0060

using System.Collections.Generic;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Provides a usage of <b>Gurth's symmetrical placement</b> (GSP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="SymmetryType">The symmetry type used.</param>
	/// <param name="MappingTable">The mapping table.</param>
	public sealed record GspTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, SymmetryType SymmetryType, int?[] MappingTable)
		: SymmetryTechniqueInfo(Conclusions, Views)
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

			using var elims = new ConclusionCollection(Conclusions);
			string conclusions = elims.ToString();
			var sb = new StringBuilder();
			for (int i = 0; i < 9; i++)
			{
				int? value = MappingTable[i];
				sb.Append($"{i + 1}{(!value.HasValue || value == i ? "" : $" -> {value.Value + 1}")}{separator}");
			}

			string customName = NameAttribute.GetName(SymmetryType)!.ToLower();
			string mapping = sb.RemoveFromEnd(separator.Length).ToString();
			return $"{Name}: Symmetry type: {customName}, mapping relations: {mapping} => {conclusions}";
		}
	}
}
