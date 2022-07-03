namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that represents a sudoku information.
/// </summary>
public abstract class DrawingElement : IEquatable<DrawingElement>, IEqualityOperators<DrawingElement, DrawingElement>
{
	/// <summary>
	/// Indicates the type identifier. The value implemented can be used for the hashing.
	/// For example, if the containing type is <c>T</c>, the property can return <c>nameof(T)</c>.
	/// </summary>
	protected abstract string TypeIdentifier { get; }


	/// <summary>
	/// Provides a way to assign the inner properties using the reflection via the specified parameters.
	/// </summary>
	/// <param name="objectHandler">The handler that checks and changes the inner value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DynamicAssign(Action<dynamic> objectHandler) => objectHandler(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as DrawingElement);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	public abstract bool Equals([NotNullWhen(true)] DrawingElement? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <summary>
	/// To get the <see cref="UIElement"/> that is used for displaying the data structure,
	/// on the <see cref="Canvas"/>.
	/// </summary>
	/// <returns>The <see cref="UIElement"/> control instance.</returns>
	public abstract UIElement GetControl();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(DrawingElement? left, DrawingElement? right) =>
		(left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(DrawingElement? left, DrawingElement? right) => !(left == right);
}
