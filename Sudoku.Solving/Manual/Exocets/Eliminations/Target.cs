namespace Sudoku.Solving.Manual.Exocets.Eliminations
{
	/// <summary>
	/// Indicates the special eliminations provided by target, which is the basic eliminations.
	/// </summary>
	public sealed class Target : SpecialConclusions
	{
		/// <inheritdoc/>
		public override string EliminationTypeName => "Target";
	}
}
