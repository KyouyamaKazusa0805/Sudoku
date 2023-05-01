namespace SudokuStudio.Configuration;

/// <summary>
/// Defines the font serialization data.
/// </summary>
[DependencyProperty<string>("FontName", DefaultValue = "")]
[DependencyProperty<decimal>("FontScale")]
[DependencyProperty<Color>("FontColor")]
public sealed partial class FontSerializationData : DependencyObject
{
	[DefaultValue]
	private static readonly decimal FontScaleDefaultValue = .6M;

	[DefaultValue]
	private static readonly Color FontColorDefaultValue = Colors.Black;
}
