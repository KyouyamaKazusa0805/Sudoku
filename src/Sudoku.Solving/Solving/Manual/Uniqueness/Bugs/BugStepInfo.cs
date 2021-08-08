using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract record BugStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
		: UniquenessStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override string? Acronym => "BUG";

		/// <inheritdoc/>
		public override decimal Difficulty => 5.6M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Bug;
	}
}
