using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of guardian technique.
	/// </summary>
	public sealed class GuardianTechniqueInfo : SdpTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="guardians">The guardians.</param>
		public GuardianTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int digit,
			GridMap loop, GridMap guardians) : base(conclusions, views, digit) => (Loop, Guardians) = (loop, guardians);


		/// <summary>
		/// Indicates the loop.
		/// </summary>
		public GridMap Loop { get; }

		/// <summary>
		/// Indicates the guardians.
		/// </summary>
		public GridMap Guardians { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M + .1M * (Loop.Count >> 1);

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Guardian;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = new CellCollection(Loop).ToString();
			string guardians = new CellCollection(Guardians).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string guardianSingularOrPlural = Guardians.Count == 1 ? "a guardian" : "guardians";
			return $"{Name}: Cells {cellsStr} with {guardianSingularOrPlural} {guardians} => {elimStr}";
		}
	}
}
