using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SudokuStudio.ComponentModel;

namespace SudokuStudio.Views.Controls.Shapes;

/// <summary>
/// Represents a circle ring.
/// </summary>
[DependencyProperty<double>("StrokeThickness", DocSummary = "Indicates the stroke thickness.")]
public sealed partial class CircleRing : UserControl
{
	[Default]
	private static readonly double StrokeThicknessDefaultValue = 6;


	/// <summary>
	/// Initializes a <see cref="CircleRing"/> instance.
	/// </summary>
	public CircleRing()
	{
		InitializeComponent();

		Background = new SolidColorBrush(Colors.DimGray with { A = 64 });
	}
}
