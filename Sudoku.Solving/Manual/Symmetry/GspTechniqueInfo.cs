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
	public sealed class GspTechniqueInfo : SymmetryTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="symmetryType">The symmetry type.</param>
		/// <param name="mappingTable">The mapping table.</param>
		public GspTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			SymmetryType symmetryType, int?[] mappingTable) : base(conclusions, views) =>
			(SymmetryType, MappingTable) = (symmetryType, mappingTable);


		/// <summary>
		/// Indicates the symmetry type.
		/// </summary>
		public SymmetryType SymmetryType { get; }

		/// <summary>
		/// Indicates the mapping table.
		/// </summary>
		public int?[] MappingTable { get; }

		/// <inheritdoc/>
		public override bool ShowDifficulty => false;

		/// <inheritdoc/>
		public override decimal Difficulty => 7.0M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.VeryHard;

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
				sb.Append($"{i + 1}{(value is null || value == i ? "" : $" -> {(int)value + 1}")}{separator}");
			}

			string customName = NameAttribute.GetName(SymmetryType)!.ToLower();
			string mapping = sb.RemoveFromEnd(separator.Length).ToString();
			return $"{Name}: Symmetry type: {customName}, mapping relations: {mapping} => {conclusions}";
		}
	}
}
