using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Sudoku.DocComments;
using Sudoku.Solving;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for ExportAnalysisResultWindow.xaml
	/// </summary>
	public partial class ExportAnalysisResultWindow : Window
	{
		/// <summary>
		/// The analysis result.
		/// </summary>
		private readonly AnalysisResult _analysisResult;

		/// <summary>
		/// The internal dictionary of all format characters.
		/// </summary>
		private readonly IDictionary<char, bool> _dic = new Dictionary<char, bool>
		{
			['-'] = false, // Show separators.
			['#'] = false, // Show step indices.
			['@'] = true, // Don't show eliminations.
			['?'] = false, // Show bottleneck.
			['!'] = false, // Show difficulty rating of each step.
			['.'] = true, // Don't show steps after bottleneck.
			['a'] = false, // Show attributes of this puzzle (if exists).
			['b'] = false, // Show magic cells.
			['d'] = false, // Show difficulty details.
			['l'] = true // Show technique steps.
		};


		/// <summary>
		/// Initializes an instance with the specified analysis result.
		/// </summary>
		/// <param name="analysisResult">The analysis result.</param>
		public ExportAnalysisResultWindow(AnalysisResult analysisResult)
		{
			InitializeComponent();

			// Initialize controls.
			foreach (var control in _gridMain.Children.OfType<CheckBox>())
			{
				control.IsChecked = _dic[control.Tag.ToString()![0]];
			}

			_analysisResult = analysisResult;
		}


		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonExport_Click(object sender, RoutedEventArgs e)
		{
			var format = new StringBuilder();
			foreach (char key in from pair in _dic where pair.Value select pair.Key)
			{
				format.Append(key);
			}

			_textBoxAnalysisResult.Text = _analysisResult.ToString(format.ToString());
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowSeparators_Click(object sender, RoutedEventArgs e) => _dic['-'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowStepIndices_Click(object sender, RoutedEventArgs e) => _dic['#'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowLogic_Click(object sender, RoutedEventArgs e) => _dic['@'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowBottleneck_Click(object sender, RoutedEventArgs e) => _dic['?'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowDifficulty_Click(object sender, RoutedEventArgs e) => _dic['!'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckboxShowStepsAfterBottleneck_Click(object sender, RoutedEventArgs e) => _dic['.'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowAttributesOfPuzzle_Click(object sender, RoutedEventArgs e) => _dic['a'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowMagicCells_Click(object sender, RoutedEventArgs e) => _dic['b'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowDifficultyDetail_Click(object sender, RoutedEventArgs e) => _dic['d'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowTechniqueSteps_Click(object sender, RoutedEventArgs e) => _dic['l'] ^= true;
	}
}
