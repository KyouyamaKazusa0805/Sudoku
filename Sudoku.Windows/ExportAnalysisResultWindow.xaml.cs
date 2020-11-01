using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using NPOI.XWPF.UserModel;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving;
using Sudoku.Windows.Constants;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for ExportAnalysisResultWindow.xaml
	/// </summary>
	public partial class ExportAnalysisResultWindow : Window
	{
		/// <summary>
		/// The converter from pixels to real output size.
		/// </summary>
		private const int Emu = 9525;


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
		private static readonly IDictionary<char, bool> FormatCharacterList = new Dictionary<char, bool>
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
		/// <param name="settings">The settings.</param>
		public ExportAnalysisResultWindow(AnalysisResult analysisResult, Settings settings)
		{
			InitializeComponent();

			// Initialize controls.
			foreach (var control in _gridMain.Children.OfType<CheckBox>())
			{
				control.IsChecked = FormatCharacterList[control.Tag.ToString()![0]];
			}

			_analysisResult = analysisResult;
			_settings = settings;
		}


		/// <summary>
		/// Create format string.
		/// </summary>
		/// <returns>The result format string.</returns>
		private string CreateFormatString()
		{
			var format = new StringBuilder();
			foreach (char key in from pair in FormatCharacterList where pair.Value select pair.Key)
			{
				format.Append(key);
			}

			return format.ToString();
		}


		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonExport_Click(object sender, RoutedEventArgs e) =>
			_textBoxAnalysisResult.Text = _analysisResult.ToString(CreateFormatString());

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowSeparators_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['-'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowStepIndices_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['#'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowLogic_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['@'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowBottleneck_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['?'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowDifficulty_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['!'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckboxShowStepsAfterBottleneck_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['.'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowAttributesOfPuzzle_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['a'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowMagicCells_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['b'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowDifficultyDetail_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['d'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowTechniqueSteps_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['l'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonOutputAnalysisResult_Click(object sender, RoutedEventArgs e)
		{
			var sfd = new SaveFileDialog
			{
				Filter = "Word document|*.docx|Text file|*.txt",
				FilterIndex = 0,
				Title = (string)Application.Current.Resources["TitleSavingPuzzles"]
			};

			if (sfd.ShowDialog() is true)
			{
				exportFile(sfd.FileName, 500);
			}

			void exportFile(string path, int size)
			{
				bool chooseDocx = sfd.FilterIndex == 1, chooseTxt = sfd.FilterIndex == 2;

				XWPFDocument? doc = chooseDocx ? new() : null;
#nullable disable warnings
				if (_analysisResult.SolvingSteps is IReadOnlyList<TechniqueInfo> steps
					&& _analysisResult.StepGrids is IReadOnlyList<SudokuGrid> stepGrids
					&& chooseDocx)
				{
					// If the directory cannot be found, create it.
					string directoryPath = $@"{path[..path.LastIndexOf('\\')]}\Assets";
					if (!Directory.Exists(directoryPath))
					{
						Directory.CreateDirectory(directoryPath);
					}

					// Get all pictures, and input into the document.
					for (int i = 0; i < steps.Count; i++)
					{
						var (step, grid) = (steps[i], stepGrids[i]);

						string curPictureName = $"{i + 1}.png";
						string curPicturePath = $@"{directoryPath}\{curPictureName}";
						using var image = new GridPainter(new(size, size), _settings, new(grid))
						{
							View = step.Views[0],
							Conclusions = step.Conclusions
						}.Draw();

						image.Save(curPicturePath);

						using var picStream = new FileStream(curPicturePath, FileMode.Open, FileAccess.Read);

						var para = doc.CreateParagraph();
						para.Alignment = ParagraphAlignment.CENTER;
						var r = para.CreateRun();
						r.AddPicture(picStream, (int)PictureType.PNG, curPictureName, size * Emu, size * Emu);
						r.SetText(step.ToString());

						// Bug fix: The document cannot be opened due to NPOI inserts pictures.
						r.GetCTR().GetDrawingList()[0].inline[0].docPr.id = 1;
					}

					// Output the document.
					using var resultDocumentStream = new FileStream(path, FileMode.Create);
					doc.Write(resultDocumentStream);

					// If we don't need to save pictures, just delete them.
					if (!(_checkBoxOutputStepGrids.IsChecked ?? false))
					{
						bool dirExists = true;
						foreach (var file in Directory.GetFiles(directoryPath))
						{
							try
							{
								File.Delete(file);
							}
							catch when (!(dirExists = Directory.Exists(directoryPath)))
							{
								break;
							}
							catch
							{
							}
						}

						// If something is wrong occurred above, check it.
						if (dirExists && Directory.GetFiles(directoryPath).None())
						{
							Directory.Delete(directoryPath);
						}
					}
				}
#nullable restore warnings

				if (chooseTxt)
				{
					// Save text file.
					File.WriteAllText(sfd.FileName, _analysisResult.ToString(CreateFormatString()));
				}

				Messagings.SaveSuccess();
			}
		}
	}
}
