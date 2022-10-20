namespace System;

/// <summary>
/// <inheritdoc cref="ICloneable" path="/summary"/>
/// </summary>
/// <typeparam name="TSelf">The type of the instance.</typeparam>
public interface ICloneable<out TSelf> : ICloneable where TSelf : class?, ICloneable<TSelf>?
{
	/// <inheritdoc cref="ICloneable.Clone"/>
	public new abstract TSelf Clone();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	object ICloneable.Clone() => Clone() ?? throw new InvalidOperationException("Target cloneation is invalid.");
}
