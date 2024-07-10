namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a circle ring.
/// </summary>
public sealed partial class CircleRing : UserControl
{
	/// <summary>
	/// Indicates the stroke thickness.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = 6D)]
	public partial double StrokeThickness { get; set; }


	/// <summary>
	/// Initializes a <see cref="CircleRing"/> instance.
	/// </summary>
	public CircleRing()
	{
		InitializeComponent();

		Background = new SolidColorBrush(Colors.DimGray with { A = 64 });
	}
}
