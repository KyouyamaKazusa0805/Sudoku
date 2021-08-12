using System.Windows;
using Sudoku.Generating;

namespace Sudoku.Windows;

/// <summary>
/// Interaction logic for <c>TechniqueViewWindow.xaml</c>.
/// </summary>
public partial class TechniqueViewWindow : Window
{
	/// <summary>
	/// Initializes a default <see cref="TechniqueViewWindow"/> instance.
	/// </summary>
	public TechniqueViewWindow()
	{
		InitializeComponent();

		ChosenTechniques = _techniqueList.ChosenTechniques;
	}


	/// <summary>
	/// Indicates the techniques having chosen.
	/// </summary>
	public TechniqueCodeFilter ChosenTechniques { get; }


	private void ButtonSelect_Click(object sender, RoutedEventArgs e) => DialogResult = true;

	private void ButtonCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
