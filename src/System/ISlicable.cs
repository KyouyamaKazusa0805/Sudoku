namespace System;

/// <summary>
/// Provides with the mechanism to allow using slice syntax.
/// </summary>
/// <typeparam name="TSelf">The type of the instance.</typeparam>
/// <typeparam name="TElement">The type of each element.</typeparam>
public interface ISlicable<[Self] TSelf, TElement> where TSelf : ISlicable<TSelf, TElement>?
{
	/// <summary>
	/// Indicates the cardinality of the collection.
	/// </summary>
	int Length { get; }


	/// <inheritdoc cref="IReadOnlyList{T}.this[int]"/>
	TElement this[int index] { get; }


	/// <summary>
	/// Gets the <typeparamref name="TSelf"/> instance that starts with the specified index.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="count">The desired number of offsets.</param>
	/// <returns>The <typeparamref name="TSelf"/> result.</returns>
	TSelf Slice(int start, int count);
}
