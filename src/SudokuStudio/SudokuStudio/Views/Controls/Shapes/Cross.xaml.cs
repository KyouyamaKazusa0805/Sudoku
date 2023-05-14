namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a cross-sign shape.
/// </summary>
[DependencyProperty<double>("StrokeThickness", DocSummary = "Indicates the stroke thickness.")]
public sealed partial class Cross : UserControl
{
	[DefaultValue]
	private static readonly double StrokeThicknessDefaultValue = 6;


	/// <summary>
	/// Initializes a <see cref="Cross"/> instance.
	/// </summary>
	public Cross()
	{
		InitializeComponent();

		Background = new SolidColorBrush(Colors.DimGray with { A = 64 });
	}
}
