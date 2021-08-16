namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="Rectangle"/>.
/// </summary>
/// <seealso cref="Rectangle"/>
internal static class RectangleExtensions
{
	/// <summary>
	/// Zoom in or out the rectangle by the specified offset.
	/// If the offset is positive, the rectangle will be larger; otherwise, smaller.
	/// </summary>
	/// <param name="this">The rectangle.</param>
	/// <param name="offset">The offset to zoom in or out.</param>
	/// <returns>The new rectangle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle Zoom(this in Rectangle @this, int offset)
	{
		var result = @this;
		result.X -= offset;
		result.Y -= offset;
		result.Width += offset * 2;
		result.Height += offset * 2;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this in Rectangle @this, out Point point, out Size size)
	{
		point = new(@this.X, @this.Y);
		size = @this.Size;
	}
}
