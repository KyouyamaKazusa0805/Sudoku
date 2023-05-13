namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a circle ring.
/// </summary>
[DependencyProperty<Brush>("Background", DocSummary = "Indicates the color of the background.")]
public sealed partial class CircleRing : UserControl
{
	[DefaultValue]
	private static readonly Brush BackgroundDefaultValue = new SolidColorBrush(Colors.DimGray with { A = 64 });


	/// <summary>
	/// Initializes a <see cref="CircleRing"/> instance.
	/// </summary>
	public CircleRing() => InitializeComponent();
}
