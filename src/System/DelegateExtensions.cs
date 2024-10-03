namespace System;

/// <summary>
/// Provides extension methods on <see cref="Delegate"/>.
/// </summary>
/// <seealso cref="Delegate"/>
public static class DelegateExtensions
{
	/// <summary>
	/// Returns the invocation list of the delegate.
	/// </summary>
	/// <typeparam name="TDelegate">The type of the delegate.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>An array of delegates representing the invocation list of the current delegate.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TDelegate[] GetInvocations<TDelegate>(this TDelegate @this) where TDelegate : Delegate
		=> from TDelegate element in @this.GetInvocationList() select element;

	/// <inheritdoc cref="Delegate.EnumerateInvocationList{TDelegate}(TDelegate)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DelegateEnumerator<TDelegate> GetEnumerator<TDelegate>(this TDelegate? @this) where TDelegate : Delegate
		=> new(@this);
}
