using System.ComponentModel;

namespace Windows.Foundation;

/// <summary>
/// Provides extension methods on <see cref="Point"/>.
/// </summary>
/// <seealso cref="Point"/>
public static class PointExtensions
{
	/// <summary>
	/// Deconstruct the instance to multiple values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Deconstruct(this in Point @this, out double x, out double y)
	{
		x = @this.X;
		y = @this.Y;
	}
}
