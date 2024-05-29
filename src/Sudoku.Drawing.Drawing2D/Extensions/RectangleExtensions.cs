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
	public static Rectangle Zoom(this Rectangle @this, int offset)
		=> @this with { X = @this.X - offset, Y = @this.Y - offset, Width = @this.Width + offset * 2, Height = @this.Height + offset * 2 };

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this Rectangle @this, out Point point, out Size size)
		=> (point, size) = (new(@this.X, @this.Y), @this.Size);
}
