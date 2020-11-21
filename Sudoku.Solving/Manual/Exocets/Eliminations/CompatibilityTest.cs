namespace Sudoku.Solving.Manual.Exocets.Eliminations
{
	/// <summary>
	/// Indicates the eliminations provided by compatibility test.
	/// </summary>
	public sealed class CompatibilityTest : SpecialConclusions
	{
		/// <inheritdoc/>
		public override string EliminationTypeName => "Compatibility test";
	}
}
