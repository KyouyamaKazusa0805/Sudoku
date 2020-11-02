using System.Collections.Generic;
using System.IO;
using NPOI.XWPF.UserModel;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving;

namespace Sudoku.IO
{
	/// <summary>
	/// Encapsulates a instance for saving analysis result.
	/// </summary>
	public sealed class AnalysisResultFileOutput
	{
		/// <summary>
		/// The converter from pixels to real output size.
		/// </summary>
		private const int Emu = 9525;


		/// <summary>
		/// Initializes an instance with the analysis result instance.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="settings">The settings.</param>
		public AnalysisResultFileOutput(AnalysisResult result, Settings settings) =>
			(AnalysisResult, Settings) = (result, settings);


		/// <summary>
		/// Indicates the analysis result.
		/// </summary>
		public AnalysisResult AnalysisResult { get; }

		/// <summary>
		/// Indicates the settings.
		/// </summary>
		public Settings Settings { get; }


		/// <summary>
		/// Export the analysis result.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="size">The size of pixels.</param>
		/// <param name="saveStepGridPictures">Indicates whether we also save step grid pictures.</param>
		/// <param name="format">The format.</param>
		/// <param name="outputType">The output type.</param>
		/// <param name="alignment">The alignment.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public bool Export(
			string path, int size, bool saveStepGridPictures,
			string format, AnalysisResultOutputType outputType = AnalysisResultOutputType.Text,
			Alignment alignment = Alignment.Middle)
		{
			try
			{
				bool chooseTxt = outputType == AnalysisResultOutputType.Text;
				bool chooseDocx = outputType == AnalysisResultOutputType.WordDocument;
				XWPFDocument? doc = chooseDocx ? new() : null;
#nullable disable warnings
				if (AnalysisResult.SolvingSteps is IReadOnlyList<TechniqueInfo> steps
					&& AnalysisResult.StepGrids is IReadOnlyList<SudokuGrid> stepGrids
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
						using var image = new GridPainter(new(size, size), Settings, new(grid))
						{
							View = step.Views[0],
							Conclusions = step.Conclusions
						}.Draw();

						image.Save(curPicturePath);

						using var picStream = new FileStream(curPicturePath, FileMode.Open, FileAccess.Read);

						var para = doc.CreateParagraph();
						para.Alignment = (ParagraphAlignment)(int)(alignment + 1);
						var r = para.CreateRun();
						r.AddPicture(picStream, (int)PictureType.PNG, curPictureName, size * Emu, size * Emu);
						r.SetText(step.ToString());

						// Bug fix: The document cannot be opened due to NPOI inserting pictures.
						r.GetCTR().GetDrawingList()[0].inline[0].docPr.id = 1;
					}

					// Output the document.
					using var resultDocumentStream = new FileStream(path, FileMode.Create);
					doc.Write(resultDocumentStream);

					// If we don't need to save pictures, just delete them.
					if (!saveStepGridPictures)
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
					File.WriteAllText(path, AnalysisResult.ToString(format));
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
