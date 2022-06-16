using Cae = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

namespace System;

/// <summary>
/// Represents a type that holds argument checking operations.
/// </summary>
public static class Argument
{
	/// <summary>
	/// Checks whether the specified value is equivalent to the specified value. Otherwise,
	/// an <see cref="ArgumentException"/> will be thrown.
	/// </summary>
	/// <typeparam name="TEquatable">The type of the instance.</typeparam>
	/// <param name="instance">The instance.</param>
	/// <param name="value">The value that the argument <paramref name="argName"/> must be.</param>
	/// <param name="argName">The argument name.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="argName"/> is not equal to <paramref name="value"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNotEqual<TEquatable>(
		TEquatable instance, TEquatable value, [Cae(nameof(instance))] string? argName = null)
		where TEquatable : notnull, IEquatable<TEquatable>
	{
		if (!instance.Equals(value))
		{
			throw new ArgumentException($"The argument '{argName}' must be '{value}'.", argName);
		}
	}

	/// <summary>
	/// Checks whether the specified condition is <see langword="true"/>. Otherwise,
	/// an <see cref="ArgumentException"/> will be thrown.
	/// </summary>
	/// <param name="condition">The condition.</param>
	/// <param name="message">The error message.</param>
	/// <param name="conditionStr">The string representation of the condition expression.</param>
	/// <exception cref="ArgumentException">
	/// Throws when checking failed on argument <paramref name="condition"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfFalse(
		bool condition, string? message = null, [Cae(nameof(condition))] string? conditionStr = null)
	{
		if (!condition)
		{
			throw new ArgumentException(message ?? $"The condition is failed to be checked: '{conditionStr}'.");
		}
	}

	/// <summary>
	/// Checks whether the specified condition is <see langword="true"/>. Otherwise,
	/// an <see cref="InvalidOperationException"/> will be thrown.
	/// </summary>
	/// <param name="condition">The condition.</param>
	/// <param name="message">The error message.</param>
	/// <param name="conditionStr">The string representation of the condition expression.</param>
	/// <exception cref="InvalidOperationException">
	/// Throws when checking failed on argument <paramref name="condition"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfInvalid(
		bool condition, string? message = null, [Cae(nameof(condition))] string? conditionStr = null)
	{
		if (!condition)
		{
			throw new InvalidOperationException(
				message ?? $"Cannot operate due to the condition failed to be checked: '{conditionStr}'.");
		}
	}

	/// <summary>
	/// Checks whether the specified pointer value is not <see langword="null"/>. Otherwise,
	/// an <see cref="ArgumentNullException"/> will be thrown.
	/// </summary>
	/// <param name="pointer">The pointer value.</param>
	/// <param name="argName">The argument name.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the <paramref name="argName"/> is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe void ThrowIfNull(void* pointer, [Cae(nameof(pointer))] string? argName = null)
	{
		if (pointer == null)
		{
			throw new ArgumentNullException(argName);
		}
	}

	/// <summary>
	/// Checks whether the specified reference value is not <see langword="null"/>. Otherwise,
	/// an <see cref="ArgumentNullException"/> will be thrown.
	/// </summary>
	/// <typeparam name="TStruct">The type of the real instance.</typeparam>
	/// <param name="ref">The pointer value.</param>
	/// <param name="argName">The argument name.</param>
	/// <exception cref="ArgumentNullException"></exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullRef<TStruct>(in TStruct @ref, [Cae(nameof(@ref))] string? argName = null)
		where TStruct : struct
	{
		if (Unsafe.IsNullRef(ref Unsafe.AsRef(@ref)))
		{
			throw new ArgumentNullException(argName);
		}
	}
}
