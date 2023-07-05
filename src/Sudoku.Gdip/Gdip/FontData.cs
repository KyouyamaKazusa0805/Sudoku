namespace Sudoku.Gdip;

/// <summary>
/// Encapsulates a font data to be serialized.
/// </summary>
/// <param name="fontName">Indicates the name of the font.</param>
/// <param name="fontSize">Indicates the font size.</param>
/// <param name="fontStyle">Indicates the font style.</param>
[Equals]
[GetHashCode]
[method: SetsRequiredMembers]
public sealed partial class FontData(
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set"), HashCodeMember] string fontName,
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set"), HashCodeMember] float fontSize,
	[PrimaryConstructorParameter(Accessibility = "public required", SetterExpression = "set"), HashCodeMember] FontStyle fontStyle
) : IEquatable<FontData>
{
	[DeconstructionMethod]
	public partial void Deconstruct(out string fontName, out float fontSize, out FontStyle fontStyle);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] FontData? other)
		=> other is not null && FontName == other.FontName && FontSize == other.FontSize && FontStyle == other.FontStyle;


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
