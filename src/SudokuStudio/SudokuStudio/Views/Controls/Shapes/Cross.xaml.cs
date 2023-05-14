namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a cross-sign shape.
/// </summary>
[DependencyProperty<double>("StrokeThickness", DefaultValue = 10, DocSummary = "Indicates the stroke thickness.")]
public sealed partial class Cross : UserControl
{
	/// <summary>
	/// Initializes a <see cref="Cross"/> instance.
	/// </summary>
	public Cross() => InitializeComponent();
}
