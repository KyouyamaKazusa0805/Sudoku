using System.Collections.Generic;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Provides a usage of Gurth's symmetrical placement.
	/// </summary>
	public sealed class GurthSymmetricalPlacementTechniqueInfo : SymmetryTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="symmetricalType">The symmetrical type.</param>
		/// <param name="mappingTable">The mapping table.</param>
		public GurthSymmetricalPlacementTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			SymmetricalType symmetricalType, int?[] mappingTable)
			: base(conclusions, views) =>
			(SymmetricalType, MappingTable) = (symmetricalType, mappingTable);


		/// <summary>
		/// Indicates the symmetrical type.
		/// </summary>
		public SymmetricalType SymmetricalType { get; }

		/// <summary>
		/// Indicates the mapping table.
		/// </summary>
		public int?[] MappingTable { get; }

		/// <inheritdoc/>
		public override bool ShowDifficulty => false;

		/// <inheritdoc/>
		public override string Name => "Gurth's Symmetrical Placement";

		/// <inheritdoc/>
		public override decimal Difficulty => 7m;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.VeryHard;


		/// <inheritdoc/>
		public override string ToString()
		{
			const string separator = ", ";

			string conclusions = ConclusionCollection.ToString(Conclusions);
			var sb = new StringBuilder();
			for (int i = 0; i < 9; i++)
			{
				int? value = MappingTable[i];
				sb.Append($"{i + 1}{(value is null || value == i ? "" : $" -> {(int)value + 1}")}{separator}");
			}

			string customName = SymmetricalType.GetCustomName()!.ToLower();
			string mapping = sb.RemoveFromEnd(separator.Length).ToString();
			return $"{Name}: Symmetrical type: {customName}, mapping relations: {mapping} => {conclusions}";
		}
	}
}
