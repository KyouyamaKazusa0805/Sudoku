namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a cross-sign shape.
/// </summary>
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


	/// <summary>
	/// Indicates the stroke thickness.
	/// </summary>
	[AutoDependencyProperty]
	public partial double StrokeThickness { get; set; }

	/// <summary>
	/// Indicates whether the forward line is shown.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = Visibility.Visible)]
	public partial Visibility ForwardLineVisibility { get; set; }

	/// <summary>
	/// Indicates whether the backward line is shown.
	/// </summary>
	[AutoDependencyProperty(DefaultValue = Visibility.Visible)]
	public partial Visibility BackwardLineVisibility { get; set; }
}
