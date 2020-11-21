namespace Sudoku.Solving.Manual.Exocets.Eliminations
{
	/// <summary>
	/// Indicates the special eliminations provided by target pair.
	/// </summary>
	public sealed class TargetPair : SpecialConclusions
	{
		/// <inheritdoc/>
		public override string EliminationTypeName => "Target pair";
	}
}
