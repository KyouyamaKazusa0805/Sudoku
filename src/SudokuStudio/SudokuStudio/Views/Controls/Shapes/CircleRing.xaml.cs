namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a circle ring.
/// </summary>
[DependencyProperty<double>("StrokeThickness", DefaultValue = 10, DocSummary = "Indicates the stroke thickness.")]
[DependencyProperty<Brush>("Background", DocSummary = "Indicates the color of the background.")]
public sealed partial class CircleRing : UserControl
{
	[DefaultValue]
	private static readonly Brush BackgroundDefaultValue = new SolidColorBrush(Colors.DimGray with { A = 64 });


	/// <summary>
	/// Initializes a <see cref="CircleRing"/> instance.
	/// </summary>
	public CircleRing() => InitializeComponent();


	[Callback]
	private static void StrokeThicknessPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (CircleRing inst, { NewValue: double value }))
		{
			inst.BackingEllipse.StrokeThickness = value;
		}
	}
}
