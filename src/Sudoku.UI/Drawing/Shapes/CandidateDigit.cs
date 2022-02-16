namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that displays for a digit that is the candidate-levelled digit.
/// </summary>
#if DEBUG
#endif
public sealed class CandidateDigit : DrawingElement
{
	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) => throw new NotImplementedException();

	/// <inheritdoc/>
	public override int GetHashCode() => throw new NotImplementedException();

	/// <inheritdoc/>
	public override UIElement GetControl() => throw new NotImplementedException();
}
