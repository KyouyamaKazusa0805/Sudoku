namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>nishio forcing chains</b> (<b>Nishio FCs</b>) technique searcher.
	/// </summary>
	public sealed class NishioFcStepSearcher : FcStepSearcher
	{
		/// <summary>
		/// Initializes a <see cref="NishioFcStepSearcher"/> instance.
		/// </summary>
		public NishioFcStepSearcher() : base(true, false, true)
		{
		}
	}
}
