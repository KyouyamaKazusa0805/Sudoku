namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Array"/>.
/// </summary>
/// <seealso cref="Array"/>
public static class ArrayExtensions
{
#pragma warning disable CS1584, CS1658, IDE0001
	/// <inheritdoc cref="Collections.Immutable.ImmutableArrayExtensions.CollectionElementEquals{T}(ImmutableArray{T}, ImmutableArray{T}, delegate*{T, T, bool})"/>
#pragma warning restore CS1584, CS1658, IDE0001
	public static unsafe bool CollectionElementEquals<T>(this T[] @this, T[] other, delegate*<T, T, bool> comparison)
	{
		if (@this.Length != other.Length)
		{
			return false;
		}

		for (var i = 0; i < @this.Length; i++)
		{
			if (!comparison(@this[i], other[i]))
			{
				return false;
			}
		}

		return true;
	}

#pragma warning disable CS1584, CS1658, IDE0001
	/// <inheritdoc cref="Collections.Immutable.ImmutableArrayExtensions.CollectionElementRefEquals{T}(ImmutableArray{T}, ImmutableArray{T}, delegate*{T, T, bool})"/>
#pragma warning restore CS1584, CS1658, IDE0001
	public static unsafe bool CollectionElementRefEquals<T>(this T[] @this, T[] other, delegate*<in T, in T, bool> comparison)
	{
		if (@this.Length != other.Length)
		{
			return false;
		}

		for (var i = 0; i < @this.Length; i++)
		{
			if (!comparison(@this[i], other[i]))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Creates a new <see cref="Array"/> instance of type <typeparamref name="T"/>,
	/// with all elements in the current instance, except the element at the specified index.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The list.</param>
	/// <param name="index">The desired index.</param>
	/// <returns>The target list.</returns>
	public static T[] CopyExcept<T>(this T[] @this, int index)
	{
		var result = new T[@this.Length - 1];
		for (var i = 0; i < index; i++)
		{
			result[i] = @this[i];
		}
		for (var i = index + 1; i < @this.Length; i++)
		{
			result[i - 1] = @this[i];
		}

		return result;
	}
}
