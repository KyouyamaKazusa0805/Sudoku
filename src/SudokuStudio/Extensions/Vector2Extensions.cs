namespace System.Numerics;

/// <summary>
/// Provides with extension methods on <see cref="Vector2"/>.
/// </summary>
/// <seealso cref="Vector2"/>
public static class Vector2Extensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Vector2 @this, out float width, out float height) => (width, height) = (@this.X, @this.Y);
}
