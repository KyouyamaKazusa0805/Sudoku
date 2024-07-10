namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a diamond shape.
/// </summary>
public sealed partial class Diamond : UserControl
{
	[Default]
	private static readonly double StrokeThicknessDefaultValue = 6;


	/// <summary>
	/// Initializes a <see cref="Diamond"/> instance.
	/// </summary>
	public Diamond() => InitializeComponent();


	/// <summary>
	/// Indicates the stroke thickness for the star.
	/// </summary>
	[AutoDependencyProperty]
	public partial double StrokeThickness { get; set; }


	private void ParentViewBox_SizeChanged(object sender, SizeChangedEventArgs e)
		=> PathPresenter.StrokeThickness = StrokeThicknessDefaultValue * 16 / ParentViewBox.ActualWidth;


	[Callback]
	private static void StrokeThicknessPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not Path { Parent: Viewbox { ActualWidth: var aw } } pathControl)
		{
			return;
		}

		pathControl.StrokeThickness = (double)e.NewValue * 16 / aw;
	}
}
