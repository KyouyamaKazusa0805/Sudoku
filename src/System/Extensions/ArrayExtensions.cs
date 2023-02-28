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
}
