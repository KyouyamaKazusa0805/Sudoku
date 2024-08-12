namespace System.Collections.Generic;

/// <summary>
/// Represents a type that creates a <see cref="LinkedList{T}"/> instance.
/// </summary>
/// <seealso cref="LinkedList{T}"/>
public static class LinkedList
{
	/// <summary>
	/// Creates a <see cref="LinkedList{T}"/> with only one element.
	/// </summary>
	/// <typeparam name="T">The type of each element of the return value.</typeparam>
	/// <param name="value">The value to be created.</param>
	/// <returns>A <see cref="LinkedList{T}"/> instance, with only one element inside it.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LinkedList<T> Singleton<T>(T value)
	{
		var result = new LinkedList<T>();
		result.AddLast(value);
		return result;
	}

	/// <summary>
	/// Creates a <see cref="LinkedList{T}"/> with the original values and a new value.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="original">The original collection.</param>
	/// <param name="newValue">The new value to be added.</param>
	/// <returns>A <see cref="LinkedList{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LinkedList<T> Create<T>(LinkedList<T> original, T newValue)
	{
		var result = new LinkedList<T>(original);
		result.AddLast(newValue);
		return result;
	}
}
