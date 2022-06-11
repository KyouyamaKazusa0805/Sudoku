namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines an element in the <see cref="View"/> to be displayed as a <see cref="DrawingElement"/>.
/// </summary>
/// <seealso cref="View"/>
/// <seealso cref="DrawingElement"/>
public abstract class ViewElement : DrawingElement
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] DrawingElement? other) => Equals(other as ViewElement);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	public abstract bool Equals([NotNullWhen(true)] ViewElement? other);
}
