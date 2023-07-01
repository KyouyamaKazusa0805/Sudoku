namespace Sudoku.Gdip;

/// <summary>
/// Encapsulates a font data to be serialized.
/// </summary>
[Equals]
public sealed partial class FontData : IEquatable<FontData>
{
	/// <summary>
	/// Initializes a <see cref="FontData"/> instance.
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


	[DeconstructionMethod]
	public partial void Deconstruct(out string fontName, out float fontSize, out FontStyle fontStyle);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] FontData? other)
		=> other is not null && FontName == other.FontName && FontSize == other.FontSize && FontStyle == other.FontStyle;

	[GeneratedOverridingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(FontName), nameof(FontSize), nameof(FontStyle))]
	public override partial int GetHashCode();


	/// <summary>
	/// Creates a <see cref="Font"/> instance via the current <see cref="FontData"/> instance.
	/// </summary>
	/// <returns>
	/// The <see cref="Font"/> instance created. Please note that the created result should use <see langword="using"/> statement
	/// to limit the lifestyle:
	/// <code><![CDATA[
	/// using var font = data.CreateFont();
	/// ]]></code>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Font CreateFont()
	{
		var (fontName, fontSize, fontStyle) = this;
		return new(fontName, fontSize, fontStyle);
	}
}
