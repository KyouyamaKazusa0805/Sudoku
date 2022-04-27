using System.ComponentModel;
using System.Runtime.CompilerServices;
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
		=> (start, end) = (new(@this.X1, @this.Y1), new(@this.X2, @this.Y2));
}
