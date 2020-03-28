using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Solving;

namespace Sudoku.Forms
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
			['d'] = false // Show difficulty details.
		};


		public ExportAnalysisResultWindow(AnalysisResult analysisResult)
		{
			InitializeComponent();
			InitializeControls();

			_analysisResult = analysisResult;
		}


		/// <summary>
		/// Initialize controls.
		/// </summary>
		private void InitializeControls()
		{
			foreach (var control in _gridMain.Children)
			{
				if (control is CheckBox checkBox)
				{
					checkBox.IsChecked = _dic[checkBox.Tag!.ToString()![0]];
				}
			}
		}


		private void ButtonExport_Click(object sender, RoutedEventArgs e)
		{
			var format = new StringBuilder();
			foreach (char key in from pair in _dic where pair.Value select pair.Key)
			{
				format.Append(key);
			}

			_textBoxAnalysisResult.Text = _analysisResult.ToString(format.ToString());
		}

		private void CheckBoxShowSeparators_Click(object sender, RoutedEventArgs e) =>
			_dic['-'] ^= true;

		private void CheckBoxShowStepIndices_Click(object sender, RoutedEventArgs e) =>
			_dic['#'] ^= true;

		private void CheckBoxShowLogic_Click(object sender, RoutedEventArgs e) =>
			_dic['@'] ^= true;

		private void CheckBoxShowBottleneck_Click(object sender, RoutedEventArgs e) =>
			_dic['?'] ^= true;

		private void CheckBoxShowDifficulty_Click(object sender, RoutedEventArgs e) =>
			_dic['!'] ^= true;

		private void CheckboxShowStepsAfterBottleneck_Click(object sender, RoutedEventArgs e) =>
			_dic['.'] ^= true;

		private void CheckBoxShowAttributesOfPuzzle_Click(object sender, RoutedEventArgs e) =>
			_dic['a'] ^= true;

		private void CheckBoxShowMagicCells_Click(object sender, RoutedEventArgs e) =>
			_dic['b'] ^= true;

		private void CheckBoxShowDifficultyDetail_Click(object sender, RoutedEventArgs e) =>
			_dic['d'] ^= true;
	}
}
