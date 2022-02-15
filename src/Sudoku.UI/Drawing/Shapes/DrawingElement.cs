namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that represents a sudoku information.
/// </summary>
public abstract class DrawingElement :
	IEquatable<DrawingElement>,
	IEqualityOperators<DrawingElement, DrawingElement>
{
	/// <summary>
	/// Provides a way to assign the inner properties using the reflection via the specified parameters.
	/// </summary>
	/// <param name="objectHandler">The handler that checks and changes the inner value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DynamicAssign(Action<dynamic> objectHandler) => objectHandler(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is DrawingElement comparer && Equals(comparer);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	public abstract bool Equals([NotNullWhen(true)] DrawingElement? other);

	/// <inheritdoc/>
	public abstract override int GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => GetType().FullName!;

	/// <summary>
	/// Gets the inner control.
	/// </summary>
	/// <returns>The control.</returns>
	public abstract UIElement GetControl();


	/// <summary>
	/// Determines whether the two <see cref="DrawingElement"/>s are equal of both type and inner value. 
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(DrawingElement? left, DrawingElement? right) =>
		(A: left, B: right) switch
		{
			(A: null, B: null) => true,
			(A: not null, B: not null) => left.Equals(right),
			_ => false
		};

	/// <summary>
	/// Determines whether the two <see cref="DrawingElement"/>s aren't equal of both type and inner value. 
	/// </summary>
	/// <param name="left">The left-side instance to compare.</param>
	/// <param name="right">The right-side instance to compare.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(DrawingElement? left, DrawingElement? right) => !(left == right);
}
