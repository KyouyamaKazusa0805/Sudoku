namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="RectangleF"/>.
/// </summary>
/// <seealso cref="RectangleF"/>
internal static class RectangleFExtensions
{
	/// <summary>
	/// Zoom in or out the rectangle by the specified offset.
	/// If the offset is positive, the rectangle will be larger; otherwise, smaller.
	/// </summary>
	/// <param name="this">The rectangle.</param>
	/// <param name="offset">The offset to zoom in or out.</param>
	/// <returns>The new rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectangleF Zoom(this RectangleF @this, float offset)
	{
		var result = @this;
		result.X -= offset;
		result.Y -= offset;
		result.Width += offset * 2;
		result.Height += offset * 2;
		return result;
	}

	/// <summary>
	/// Truncate the specified rectangle.
	/// </summary>
	/// <param name="this">The rectangle.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle Truncate(this RectangleF @this) => new((int)@this.X, (int)@this.Y, (int)@this.Width, (int)@this.Height);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this RectangleF @this, out PointF point, out SizeF size) => (point, size) = (@this.Location, @this.Size);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this RectangleF @this, out float x, out float y, out float width, out float height)
		=> (x, y, width, height) = (@this.X, @this.Y, @this.Width, @this.Height);
}
