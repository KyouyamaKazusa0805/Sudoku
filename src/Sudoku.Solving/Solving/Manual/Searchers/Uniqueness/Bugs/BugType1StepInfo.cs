namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 1 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public sealed record BugType1StepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
		: BugStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugType1;
	}
}
