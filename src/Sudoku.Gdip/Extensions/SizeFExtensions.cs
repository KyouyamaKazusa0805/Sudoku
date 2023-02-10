namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="SizeF"/>.
/// </summary>
/// <seealso cref="SizeF"/>
internal static partial class SizeFExtensions
{
	/// <summary>
	/// To truncate the size.
	/// </summary>
	/// <param name="this">The size.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size Truncate(this SizeF @this) => new((int)@this.Width, (int)@this.Height);

	[DeconstructionMethod]
	public static partial void Deconstruct(this SizeF @this, out float width, out float height);
}
