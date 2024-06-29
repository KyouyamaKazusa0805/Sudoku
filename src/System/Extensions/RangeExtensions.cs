namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Range"/> instances.
/// </summary>
/// <seealso cref="Range"/>
public static class RangeExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Range @this, out Index start, out Index end) => (start, end) = (@this.Start, @this.End);
}
