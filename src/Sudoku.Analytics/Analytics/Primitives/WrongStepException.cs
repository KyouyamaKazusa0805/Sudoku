namespace Sudoku.Analytics.Primitives;

/// <summary>
/// Indicates an error that throws when a solving step is wrong (may be due to wrong algorithm, bug, etc.).
/// </summary>
/// <param name="grid"><inheritdoc/></param>
/// <param name="wrongStep">Indicates the wrong step.</param>
public sealed partial class WrongStepException(ref readonly Grid grid, [PrimaryConstructorParameter] Step wrongStep) : RuntimeAnalysisException(in grid)
{
	/// <inheritdoc/>
	public override string Message => string.Format(SR.Get("Message_WrongStepException"), [InvalidGrid, WrongStep]);
}
