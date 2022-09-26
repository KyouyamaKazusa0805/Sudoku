namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Regular Wing</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>XY-Wing</item>
/// <item>XYZ-Wing</item>
/// <item>WXYZ-Wing</item>
/// <item>VWXYZ-Wing</item>
/// <item>UVWXYZ-Wing</item>
/// <item>TUVWXYZ-Wing</item>
/// <item>STUVWXYZ-Wing</item>
/// <item>RSTUVWXYZ-Wing</item>
/// </list>
/// </summary>
public interface IRegularWingStepSearcher : IWingStepSearcher
{
	/// <summary>
	/// Indicates the maximum size the searcher will search for. The maximum possible value is 9.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Throws when <c>value</c> is greater than 9.</exception>
	public abstract int MaxSize { get; set; }
}
