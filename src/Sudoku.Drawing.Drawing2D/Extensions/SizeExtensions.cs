namespace System.Drawing;

/// <summary>
/// Provides with extension methods on <see cref="Size"/>.
/// </summary>
/// <seealso cref="Size"/>
internal static class SizeExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Size @this, out int width, out int height) => (width, height) = (@this.Width, @this.Height);
}
