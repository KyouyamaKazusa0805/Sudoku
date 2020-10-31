using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Sudoku.DocComments;
using Sudoku.Solving;
using Sudoku.Data;
#if DEBUG
using System.IO;
using NPOI.XWPF.UserModel;
using Sudoku.Drawing;
using PointConverter = Sudoku.Drawing.PointConverter;
using Image = System.Drawing.Image;
#endif

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
		/// The settings.
		/// </summary>
		private readonly Settings _settings;

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


#if DEBUG
		/// <summary>
		/// Initializes an instance with the specified analysis result.
		/// </summary>
		/// <param name="analysisResult">The analysis result.</param>
		/// <param name="settings">The settings.</param>
#else
		/// <summary>
		/// Initializes an instance with the specified analysis result.
		/// </summary>
		/// <param name="analysisResult">The analysis result.</param>
#endif
		public ExportAnalysisResultWindow(AnalysisResult analysisResult
#if DEBUG
			, Settings settings
#endif
			)
		{
			InitializeComponent();

			// Initialize controls.
			foreach (var control in _gridMain.Children.OfType<CheckBox>())
			{
				control.IsChecked = _dic[control.Tag.ToString()![0]];
			}

			_analysisResult = analysisResult;
			_settings = settings;
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

#if DEBUG
			exportWordFile($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Output.docx");

			void exportWordFile(string path)
			{
				const int size = 600;
				string directoryPath = $@"{path[..path.LastIndexOf('\\')]}\Assets";
				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}

				var doc = new XWPFDocument();
				var stepGrids = _analysisResult.StepGrids;
				var steps = _analysisResult.SolvingSteps;
				if ((steps, stepGrids) is (not null, not null))
				{
#nullable disable warnings
					for (int i = 0; i < steps.Count; i++)
					{
						var (step, grid) = (steps[i], stepGrids[i]);

						string curPictureName = $"{i + 1}.png";
						string curPicturePath = $@"{directoryPath}\{curPictureName}";
						saveTo(curPicturePath, getImage(grid, step));

						using var curPictureStream = new FileStream(curPicturePath, FileMode.Open, FileAccess.Read);

						var p1 = doc.CreateParagraph();
						p1.Alignment = ParagraphAlignment.CENTER;
						var graphRun = p1.CreateRun();
						graphRun.AddPicture(curPictureStream, (int)PictureType.PNG, curPictureName, size * 9525, size * 9525);
						graphRun.SetText(step.ToString());

						var inline = graphRun.GetCTR().GetDrawingList()[0].inline[0];
						inline.docPr.id = 1;
					}
#nullable restore warnings
				}

				using var resultDocumentStream = new FileStream(path, FileMode.Create);
				doc.Write(resultDocumentStream);

				static void saveTo(string path, Image image) => image.Save(path);
				Image getImage(in SudokuGrid grid, TechniqueInfo info) =>
					new GridPainter(new PointConverter(size, size), _settings, new(grid))
					{
						View = info.Views[0],
						Conclusions = info.Conclusions
					}.Draw();
			}
#endif
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
