namespace System.Drawing;

/// <summary>
/// Provides with extension methods on <see cref="Color"/>.
/// </summary>
/// <seealso cref="Color"/>
internal static partial class ColorExtensions
{
	/// <summary>
	/// Gets a target <see cref="Color"/> whose <see cref="Color.A"/> value is a quarter of the original one.
	/// </summary>
	/// <param name="this">The original color value.</param>
	/// <returns>The target result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color QuarterAlpha(this Color @this) => Color.FromArgb(@this.A >> 2, @this);

	[GeneratedDeconstruction]
	public static partial void Deconstruct(this Color @this, out byte a, out byte r, out byte g, out byte b);
}
