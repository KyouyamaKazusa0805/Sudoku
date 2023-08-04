namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a cross-sign shape.
/// </summary>
[DependencyProperty<double>("StrokeThickness", DocSummary = "Indicates the stroke thickness.")]
[DependencyProperty<Visibility>("BackwardLineVisibility", DefaultValue = Visibility.Visible, DocSummary = "Indicates whether the backward line is shown.")]
[DependencyProperty<Visibility>("ForwardLineVisibility", DefaultValue = Visibility.Visible, DocSummary = "Indicates whether the forward line is shown.")]
public sealed partial class Cross : UserControl
{
	[Default]
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
