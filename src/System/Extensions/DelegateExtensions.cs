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
	public static IEnumerable<TDelegate> GetInvocations<TDelegate>(this TDelegate? @this)
		where TDelegate : Delegate =>
		(@this?.GetInvocationList() ?? Array.Empty<TDelegate>()).Cast<TDelegate>();
}
