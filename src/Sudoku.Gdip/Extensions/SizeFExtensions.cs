using System.Runtime.CompilerServices;

namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="SizeF"/>.
/// </summary>
/// <seealso cref="SizeF"/>
public static class SizeFExtensions
{
	/// <summary>
	/// To truncate the size.
	/// </summary>
	/// <param name="this">The size.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size Truncate(this SizeF @this) => new((int)@this.Width, (int)@this.Height);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct(this SizeF @this, out float width, out float height) => (width, height) = (@this.Width, @this.Height);
}
