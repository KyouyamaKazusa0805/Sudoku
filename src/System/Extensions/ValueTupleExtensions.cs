namespace System;

/// <summary>
/// Provides with extension methods on <see cref="ValueTuple"/> type set.
/// </summary>
/// <seealso cref="ValueTuple"/>
public static class ValueTupleExtensions
{
	/// <summary>
	/// Invokes the tuple.
	/// </summary>
	/// <param name="this">The pair elements.</param>
	public static void Invoke(this (Action First, Action Second) @this)
	{
		@this.First();
		@this.Second();
	}
}
