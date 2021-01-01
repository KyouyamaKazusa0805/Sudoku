using System;
using System.Windows;
using Sudoku.DocComments;
using Sudoku.Generating;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>TechniqueViewWindow.xaml</c>.
	/// </summary>
	public partial class TechniqueViewWindow : Window
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public TechniqueViewWindow()
		{
			InitializeComponent();

			ChosenTechniques = _techniqueList.ChosenTechniques;
		}


		/// <summary>
		/// Indicates the techniques having chosen.
		/// </summary>
		public TechniqueCodeFilter ChosenTechniques { get; }


		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonSelect_Click(object sender, RoutedEventArgs e) => DialogResult = true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
	}
}
