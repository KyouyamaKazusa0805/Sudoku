namespace SudokuStudio.Models;

/// <summary>
/// Defines the font serialization data.
/// </summary>
[DependencyProperty<string>("FontName", DefaultValue = "")]
[DependencyProperty<double>("FontScale", DefaultValue = .6)]
[DependencyProperty<Color>("FontColor")]
public sealed partial class FontSerializationData : DependencyObject
{
	[DefaultValue]
	private static readonly Color FontColorDefaultValue = Colors.Black;
}
