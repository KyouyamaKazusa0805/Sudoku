namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="RectangleF"/>.
/// </summary>
/// <seealso cref="RectangleF"/>
internal static partial class RectangleFExtensions
{
	/// <summary>
	/// Zoom in or out the rectangle by the specified offset.
	/// If the offset is positive, the rectangle will be larger; otherwise, smaller.
	/// </summary>
	/// <param name="this">The rectangle.</param>
	/// <param name="offset">The offset to zoom in or out.</param>
	/// <returns>The new rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectangleF Zoom(this scoped in RectangleF @this, float offset)
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
	public static Rectangle Truncate(this scoped in RectangleF @this) => new((int)@this.X, (int)@this.Y, (int)@this.Width, (int)@this.Height);

	[GeneratedDeconstruction]
	public static partial void Deconstruct(this scoped in RectangleF @this, [GeneratedDeconstructionArgument(nameof(RectangleF.Location))] out PointF point, out SizeF size);

	[GeneratedDeconstruction]
	public static partial void Deconstruct(this scoped in RectangleF @this, out float x, out float y, out float width, out float height);
}
