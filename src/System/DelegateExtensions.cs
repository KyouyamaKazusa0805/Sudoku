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
	/// <typeparam name="T">The type of the delegate.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>An array of delegates representing the invocation list of the current delegate.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] GetInvocations<T>(this T @this) where T : Delegate => from T element in @this.GetInvocationList() select element;
}
