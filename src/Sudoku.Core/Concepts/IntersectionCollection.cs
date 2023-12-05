using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.SourceGeneration;

namespace Sudoku.Concepts;

using Pair = (IntersectionBase Base, IntersectionResult Result);

/// <summary>
/// Represents a collection that stores for a list of <see cref="IntersectionBase"/> and <see cref="IntersectionResult"/> instances.
/// </summary>
/// <seealso cref="IntersectionBase"/>
/// <seealso cref="IntersectionResult"/>
[CollectionBuilder(typeof(IntersectionCollection), nameof(Create))]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
[method: DebuggerStepThrough]
public sealed partial class IntersectionCollection([Data(DataMemberKinds.Field, Accessibility = "private readonly")] Pair[] values) : IEnumerable<Pair>
{
	/// <summary>
	/// Try to get the enumerator that iterates on the elements of the current collection.
	/// </summary>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Pair>.Enumerator GetEnumerator() => _values.AsReadOnlySpan().GetEnumerator();

	/// <summary>
	/// Projects each element into a new transform using the specified method to transform.
	/// </summary>
	/// <typeparam name="TResult">The type of result elements.</typeparam>
	/// <param name="selector">The transform method.</param>
	/// <returns>An array of <typeparamref name="TResult"/> results.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TResult[] Select<TResult>(Func<Pair, TResult> selector) => from e in _values select selector(e);

	/// <summary>
	/// Copies the internal array into a new instance, and return it.
	/// </summary>
	/// <returns>An array of <see cref="Pair"/> instances.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Pair[] ToArray() => (Pair[])_values.Clone();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<Pair> IEnumerable<Pair>.GetEnumerator() => ((IEnumerable<Pair>)_values).GetEnumerator();


	/// <summary>
	/// Creates an <see cref="IntersectionCollection"/> instance via the specified values using collection expression.
	/// </summary>
	/// <param name="values">The values.</param>
	/// <returns>An <see cref="IntersectionCollection"/> result.</returns>
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static IntersectionCollection Create(scoped ReadOnlySpan<Pair> values) => new([.. values]);
}
