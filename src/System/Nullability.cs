using CAE = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

namespace System;

/// <summary>
/// Encapsulates a set of methods that checks the nullability of an object.
/// </summary>
public static class Nullability
{
	/// <summary>
	/// Checks the nullability of the specified instance of a <see cref="Nullable{T}"/>.
	/// If the instance is <see langword="null"/>, the method will throw <see cref="ArgumentNullException"/>.
	/// </summary>
	/// <typeparam name="TStruct">The type of the instance to be checked.</typeparam>
	/// <param name="obj">The instance to be checked.</param>
	/// <param name="paramName">
	/// The parameter name. The parameter is always set to <see langword="null"/> intentionally.
	/// </param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the specified object is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNull<TStruct>([NotNull] TStruct? obj, [CAE("obj")] string? paramName = null)
	where TStruct : struct
	{
		if (!obj.HasValue)
		{
			throw new ArgumentNullException(paramName);
		}
	}

	/// <summary>
	/// Checks the nullability of the specified instance.
	/// If the instance is <see langword="null"/>, the method will throw <see cref="ArgumentNullException"/>.
	/// </summary>
	/// <typeparam name="TClass">The type of the instance to be checked.</typeparam>
	/// <param name="obj">The instance to be checked.</param>
	/// <param name="paramName">
	/// The argument name. The parameter is always set to <see langword="null"/> intentionally.
	/// </param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the specified object is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNull<TClass>([NotNull] TClass? obj, [CAE("obj")] string? paramName = null)
	where TClass : class
	{
		if (obj is null)
		{
			throw new ArgumentNullException(paramName);
		}
	}
}
