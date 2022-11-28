namespace Sudoku.Gdip;

/// <summary>
/// Encapsulates a font data to be serialized.
/// </summary>
public sealed partial class FontData : IEquatable<FontData>
{
	/// <summary>
	/// Initalizes a <see cref="FontData"/> instance.
	/// </summary>
	/// <param name="fontName">The font name.</param>
	/// <param name="fontSize">The font size.</param>
	/// <param name="fontStyle">The font style.</param>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public FontData(string fontName, float fontSize, FontStyle fontStyle) => (FontName, FontSize, FontStyle) = (fontName, fontSize, fontStyle);


	/// <summary>
	/// Indicates the name of the font.
	/// </summary>
	public required string FontName { get; set; }

	/// <summary>
	/// Indicates the font size.
	/// </summary>
	public required float FontSize { get; set; }

	/// <summary>
	/// Indicates the font style.
	/// </summary>
	public required FontStyle FontStyle { get; set; }


	[GeneratedDeconstruction]
	public partial void Deconstruct(out string fontName, out float fontSize, out FontStyle fontStyle);

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] FontData? other)
		=> other is not null && FontName == other.FontName && FontSize == other.FontSize && FontStyle == other.FontStyle;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(FontName), nameof(FontSize), nameof(FontStyle))]
	public override partial int GetHashCode();
}
