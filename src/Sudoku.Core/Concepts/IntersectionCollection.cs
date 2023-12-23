namespace Sudoku.Concepts;

/// <summary>
/// Represents a collection that stores for a list of <see cref="IntersectionBase"/> and <see cref="IntersectionResult"/> instances.
/// </summary>
/// <seealso cref="IntersectionBase"/>
/// <seealso cref="IntersectionResult"/>
[CollectionBuilder(typeof(IntersectionCollection), nameof(Create))]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
[method: DebuggerStepThrough]
public sealed partial class IntersectionCollection([Data(DataMemberKinds.Field, Accessibility = "private readonly")] IntersectionPair[] values) : IEnumerable<IntersectionPair>
{
	/// <summary>
	/// Try to get the enumerator that iterates on the elements of the current collection.
	/// </summary>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<IntersectionPair>.Enumerator GetEnumerator() => _values.AsReadOnlySpan().GetEnumerator();

	/// <summary>
	/// Projects each element into a new transform using the specified method to transform.
	/// </summary>
	/// <typeparam name="TResult">The type of result elements.</typeparam>
	/// <param name="selector">The transform method.</param>
	/// <returns>An array of <typeparamref name="TResult"/> results.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TResult[] Select<TResult>(Func<IntersectionPair, TResult> selector) => from e in _values select selector(e);

	/// <summary>
	/// Copies the internal array into a new instance, and return it.
	/// </summary>
	/// <returns>An array of <see cref="IntersectionPair"/> instances.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IntersectionPair[] ToArray() => _values[..];

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<IntersectionPair> IEnumerable<IntersectionPair>.GetEnumerator() => ((IEnumerable<IntersectionPair>)_values).GetEnumerator();


	/// <summary>
	/// Creates an <see cref="IntersectionCollection"/> instance via the specified values using collection expression.
	/// </summary>
	/// <param name="values">The values.</param>
	/// <returns>An <see cref="IntersectionCollection"/> result.</returns>
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static IntersectionCollection Create(scoped ReadOnlySpan<IntersectionPair> values) => new([.. values]);
}
