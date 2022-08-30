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
		TEquatable instance,
		TEquatable value,
		[CallerArgumentExpression(nameof(instance))] string? argName = null)
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
		[DoesNotReturnIf(false)] bool condition,
		string? message = null,
		[CallerArgumentExpression(nameof(condition))] string? conditionStr = null)
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
		[DoesNotReturnIf(false)] bool condition,
		string? message = null,
		[CallerArgumentExpression(nameof(condition))] string? conditionStr = null)
	{
		if (!condition)
		{
			throw new InvalidOperationException(
				message ?? $"Cannot operate due to the condition failed to be checked: '{conditionStr}'.");
		}
	}
}
