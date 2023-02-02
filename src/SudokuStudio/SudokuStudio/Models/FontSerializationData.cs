namespace SudokuStudio.Models;

/// <summary>
/// Defines the font serialization data.
/// </summary>
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
public sealed partial class FontSerializationData :
	IEquatable<FontSerializationData>,
	IEqualityOperators<FontSerializationData, FontSerializationData, bool>
{
	/// <summary>
	/// Indicates the font name.
	/// </summary>
	public string FontName { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the font scale.
	/// </summary>
	public decimal FontScale { get; set; } = .6M;

	/// <summary>
	/// Indicates the font color.
	/// </summary>
	public Color FontColor { get; set; } = Colors.Black;


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] FontSerializationData? other)
		=> other is not null && FontName == other.FontName && FontScale == other.FontScale && FontColor == other.FontColor;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(FontName), nameof(FontScale), nameof(FontColor))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(FontName), nameof(FontScale), nameof(FontColor))]
	public override partial string ToString();
}
