namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Index"/> instances.
/// </summary>
/// <seealso cref="Index"/>
public static class IndexExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Index @this, out int value, out bool isFromEnd) => (value, isFromEnd) = (@this.Value, @this.IsFromEnd);
}
