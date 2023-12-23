namespace Windows.Foundation;

/// <summary>
/// Provides with extension methods on <see cref="Size"/>.
/// </summary>
/// <seealso cref="Size"/>
public static class SizeExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Size @this, out float width, out float height) => (width, height) = (@this._width, @this._height);
}
