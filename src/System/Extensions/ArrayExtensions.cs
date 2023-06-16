namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Array"/>.
/// </summary>
/// <seealso cref="Array"/>
public static class ArrayExtensions
{
#pragma warning disable CS1584, CS1658, IDE0001
	/// <inheritdoc cref="ImmutableArrayExtensions.CollectionElementEquals{T}(ImmutableArray{T}, ImmutableArray{T}, delegate*{T, T, bool})"/>
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
	/// <inheritdoc cref="ImmutableArrayExtensions.CollectionElementRefEquals{T}(ImmutableArray{T}, ImmutableArray{T}, delegate*{T, T, bool})"/>
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

	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond}(IEnumerable{TFirst}, IEnumerable{TSecond})"/>
	/// <param name="this">
	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond}(IEnumerable{TFirst}, IEnumerable{TSecond})" path="/param[@name='first']"/>
	/// </param>
	/// <param name="other">
	/// <inheritdoc cref="Enumerable.Zip{TFirst, TSecond}(IEnumerable{TFirst}, IEnumerable{TSecond})" path="/param[@name='second']"/>
	/// </param>
	public static (TFirst, TSecond)[] Zip<TFirst, TSecond>(this TFirst[] @this, TSecond[] other)
	{
		if (@this.Length != other.Length)
		{
			throw new InvalidOperationException("Two arrays should be of same length.");
		}

		var result = new (TFirst, TSecond)[@this.Length];
		for (var i = 0; i < @this.Length; i++)
		{
			result[i] = (@this[i], other[i]);
		}

		return result;
	}
}
