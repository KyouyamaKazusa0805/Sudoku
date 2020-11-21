namespace Sudoku.Solving.Manual.Exocets.Eliminations
{
	/// <summary>
	/// Indicates the special eliminations provided by mirror.
	/// </summary>
	public sealed class Mirror : SpecialConclusions
	{
		/// <inheritdoc/>
		public override string EliminationTypeName => "Mirror";
	}
}
