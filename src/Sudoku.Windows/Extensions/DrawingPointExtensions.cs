namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="DPoint"/>.
/// </summary>
/// <seealso cref="DPoint"/>
public static partial class DrawingPointExtensions
{
	/// <summary>
	/// Convert a <see cref="DPoint"/> to <see cref="WPoint"/>.
	/// </summary>
	/// <param name="this">The point to convert.</param>
	/// <returns>The result of conversion.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static WPoint ToWPoint(this in DPoint @this) => new(@this.X, @this.Y);
}