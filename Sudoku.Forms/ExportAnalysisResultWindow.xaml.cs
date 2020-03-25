using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

			_analysisResult = analysisResult;
		}


		private void CheckBoxShowSeparators_IsEnabledChanged(
			object sender, DependencyPropertyChangedEventArgs e) =>
			_dic['-'] ^= true;

		private void CheckBoxShowStepIndices_IsEnabledChanged(
			object sender, DependencyPropertyChangedEventArgs e) =>
			_dic['#'] ^= true;

		private void CheckBoxShowLogic_IsEnabledChanged(
			object sender, DependencyPropertyChangedEventArgs e) =>
			_dic['@'] ^= true;

		private void CheckBoxShowBottleneck_IsEnabledChanged(
			object sender, DependencyPropertyChangedEventArgs e) =>
			_dic['?'] ^= true;

		private void CheckBoxShowDifficulty_IsEnabledChanged(
			object sender, DependencyPropertyChangedEventArgs e) =>
			_dic['!'] ^= true;

		private void CheckboxShowStepsAfterBottleneck_IsEnabledChanged(
			object sender, DependencyPropertyChangedEventArgs e) =>
			_dic['.'] ^= true;

		private void CheckBoxShowAttributesOfPuzzle_IsEnabledChanged(
			object sender, DependencyPropertyChangedEventArgs e) =>
			_dic['a'] ^= true;

		private void CheckBoxShowMagicCells_IsEnabledChanged(
			object sender, DependencyPropertyChangedEventArgs e) =>
			_dic['b'] ^= true;

		private void CheckBoxShowDifficultyDetail_IsEnabledChanged(
			object sender, DependencyPropertyChangedEventArgs e) =>
			_dic['d'] ^= true;

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void ButtonExport_Click(object sender, RoutedEventArgs e)
		{
			var format = new StringBuilder();
			foreach (char key in from pair in _dic where pair.Value select pair.Key)
			{
				format.Append(key);
			}

			_textBoxAnalysisResult.Text = await Task.Run(
				() => _analysisResult.ToString(format.ToString()));
		}
	}
}
