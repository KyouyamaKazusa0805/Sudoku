namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a circle ring.
/// </summary>
[DependencyProperty<double>("StrokeThickness", DefaultValue = 10, DocSummary = "Indicates the stroke thickness.")]
public sealed partial class CircleRing : UserControl
{
	/// <summary>
	/// Initializes a <see cref="CircleRing"/> instance.
	/// </summary>
	public CircleRing()
	{
		InitializeComponent();

		Background = new SolidColorBrush(Colors.DimGray with { A = 64 });
	}


	[Callback]
	private static void StrokeThicknessPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (CircleRing inst, { NewValue: double value }))
		{
			inst.BackingEllipse.StrokeThickness = value;
		}
	}
}
