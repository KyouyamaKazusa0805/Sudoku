namespace Sudoku.Concepts;

/// <summary>
/// Represents a collection that stores for a list of <see cref="IntersectionBase"/> and <see cref="IntersectionResult"/> instances.
/// </summary>
/// <seealso cref="IntersectionBase"/>
/// <seealso cref="IntersectionResult"/>
[CollectionBuilder(typeof(IntersectionCollection), nameof(Create))]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
[method: DebuggerStepThrough]
public sealed partial class IntersectionCollection([PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "internal readonly")] Intersection[] values) : IEnumerable<Intersection>
{
	/// <summary>
	/// Try to get the enumerator that iterates on the elements of the current collection.
	/// </summary>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Intersection>.Enumerator GetEnumerator() => _values.AsReadOnlySpan().GetEnumerator();

	/// <summary>
	/// Copies the internal array into a new instance, and return it.
	/// </summary>
	/// <returns>An array of <see cref="Intersection"/> instances.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Intersection[] ToArray() => _values[..];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Intersection> IEnumerable<Intersection>.GetEnumerator() => ((IEnumerable<Intersection>)_values).GetEnumerator();


	/// <summary>
	/// Creates an <see cref="IntersectionCollection"/> instance via the specified values using collection expression.
	/// </summary>
	/// <param name="values">The values.</param>
	/// <returns>An <see cref="IntersectionCollection"/> result.</returns>
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static IntersectionCollection Create(scoped ReadOnlySpan<Intersection> values) => new([.. values]);
}
