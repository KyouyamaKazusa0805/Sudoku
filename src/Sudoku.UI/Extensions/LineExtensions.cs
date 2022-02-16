using System.ComponentModel;
using Windows.Foundation;

namespace Microsoft.UI.Xaml.Shapes;

/// <summary>
/// Provides extension methods on <see cref="Line"/>.
/// </summary>
/// <seealso cref="Line"/>
public static class LineExtensions
{
	/// <summary>
	/// Deconstruct the instance to multiple values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Deconstruct(this Line @this, out Point start, out Point end)
	{
		start = new(@this.X1, @this.Y1);
		end = new(@this.X2, @this.Y2);
	}

	/// <summary>
	/// Deconstruct the instance to multiple values.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Deconstruct(this Line @this, out double x1, out double x2, out double y1, out double y2)
	{
		x1 = @this.X1;
		x2 = @this.X2;
		y1 = @this.Y1;
		y2 = @this.Y2;
	}
}
