namespace SudokuStudio.Models;

/// <summary>
/// Defines the font serialization data.
/// </summary>
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
[DependencyProperty<string>("FontName", DefaultValue = "")]
[DependencyProperty<double>("FontScale", DefaultValue = .6)]
[DependencyProperty<Color>("FontColor", DefaultValueGeneratingMemberName = nameof(FontColorDefaultValue))]
public sealed partial class FontSerializationData :
	DependencyObject,
	IEquatable<FontSerializationData>,
	IEqualityOperators<FontSerializationData, FontSerializationData, bool>
{
	private static readonly Color FontColorDefaultValue = Colors.Black;


	/// <include file="../../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	public void Deconstruct(out string name, out double scale, out Color color)
	{
		name = FontName;
		scale = FontScale;
		color = FontColor;
	}

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] FontSerializationData? other)
		=> other is not null && FontName == other.FontName && FontScale == other.FontScale && FontColor == other.FontColor;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, "FontName", "FontScale", "FontColor")]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, "FontName", "FontScale", "FontColor")]
	public override partial string ToString();
}
