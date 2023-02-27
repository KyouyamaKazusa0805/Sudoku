namespace System;

/// <summary>
/// <inheritdoc cref="ICloneable" path="/summary"/>
/// </summary>
/// <typeparam name="TSelf">The type of the instance.</typeparam>
public interface ICloneable<out TSelf> : ICloneable where TSelf : class?, ICloneable<TSelf>?
{
	/// <inheritdoc cref="ICloneable.Clone"/>
	new TSelf Clone();

	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Throws when method <see cref="Clone"/> returns <see langword="null"/>.</exception>
	/// <seealso cref="Clone"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	object ICloneable.Clone() => Clone() ?? throw new InvalidOperationException("Target cloneation is invalid.");
}
