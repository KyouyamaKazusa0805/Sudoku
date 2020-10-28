#if SUDOKU_RECOGNITION

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Field = Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>;
#if MUST_DOWNLOAD_TRAINED_DATA
using System.Net.Http;
using System.Threading.Tasks;
#endif

namespace Sudoku.Recognitions
{
	/// <summary>
	/// Define a recognizer.
	/// </summary>
	/// <remarks>
	/// During the recognizing, the <b>field</b> indicates the whole outline of a grid.
	/// </remarks>
	internal sealed class InternalServiceProvider : IDisposable
	{
		/// <summary>
		/// The internal <see cref="Tesseract"/> instance.
		/// </summary>
		private Tesseract? _ocr;


		/// <summary>
		/// Indicates whether the current recognizer has already initialized.
		/// </summary>
		public bool Initialized => _ocr is not null;


		/// <inheritdoc/>
		public void Dispose() => _ocr?.Dispose();

		/// <summary>
		/// Recognizes digits.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns>The grid.</returns>
		/// <exception cref="RecognitionException">
		/// Throws when the processing is wrong or unhandleable.
		/// </exception>
		public SudokuGrid RecognizeDigits(Field field)
		{
			var result = SudokuGrid.Empty;
			int w = field.Width / 9, o = w / 6;
			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 9; y++)
				{
					// Recognize digit from cell.
					int recognition = RecognizeCellNumber(field.GetSubRect(new(o + w * x, o + w * y, w - o * 2, w - o * 2)));
					if (recognition == 0)
					{
						continue;
					}

					int cell = x * 9 + y, digit = recognition - 1;
					_ = result[cell, digit] ? throw new RecognitionException($"Recognition error. Cannot fill the cell r{x + 1}c{y + 1} with the digit {digit + 1}.") : 0;

					result[cell] = digit;
				}
			}

			// The result will be transposed.
			return result.Transpose();
		}

		/// <summary>
		/// Recognize the number of a cell.
		/// </summary>
		/// <param name="cellImg">The image of a cell.</param>
		/// <returns>
		/// The result value (must be between 1 and 9). If the recognition is failed,
		/// the value will be <c>0</c>.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Throws when the OCR engine error.
		/// </exception>
		private int RecognizeCellNumber(Field cellImg)
		{
			_ = _ocr is null ? throw new NullReferenceException($"{nameof(_ocr)} cannot be null here.") : 0;

			// Convert the image to gray-scale and filter out the noise
			var imgGray = new Mat();
			CvInvoke.CvtColor(cellImg, imgGray, ColorConversion.Bgr2Gray);

			// TODO: Can be problem with values for some image.
			// Another methods to process image, but worse. Use only one!
			var imgThresholded = new Mat();
			CvInvoke.Threshold(
				imgGray, imgThresholded, InternalSettings.ThOcrMin, InternalSettings.ThOcrMax,
				ThresholdType.Binary);

			_ocr.SetImage(imgThresholded);
			_ = _ocr.Recognize() != 0
				? throw new InvalidOperationException("Tessaract Error. Cannot to recognize cell image.")
				: 0;

			var characters = _ocr.GetCharacters();
			string numberText = string.Empty;
			foreach (var c in characters)
			{
				if (c.Text is var t and not " ")
				{
					numberText += t;
				}
			}

			return numberText.Length > 1 ? -1 : int.TryParse(numberText, out int resultValue) ? resultValue : -1;
		}

#if MUST_DOWNLOAD_TRAINED_DATA
		/// <summary>
		/// Initializes <see cref="Tesseract"/> instance.
		/// </summary>
		/// <param name="dir">The directory.</param>
		/// <param name="lang">The language. The default value is <c>"eng"</c>.</param>
		/// <returns>The task.</returns>
		[MemberNotNullWhen(true, nameof(_ocr))]
		public async Task<bool> InitTesseractAsync(string dir, string lang = "eng")
#else
		/// <summary>
		/// Initializes <see cref="Tesseract"/> instance.
		/// </summary>
		/// <param name="dir">The directory.</param>
		/// <param name="lang">The language. The default value is <c>"eng"</c>.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		/// <exception cref="FileNotFoundException">Throws when the file doesn't found.</exception>
		[MemberNotNullWhen(true, nameof(_ocr))]
		public bool InitTesseract(string dir, string lang = "eng")
#endif
		{
			try
			{
				string filePath = $@"{dir}\{lang}.traineddata";
				if (!File.Exists(filePath))
				{
#if MUST_DOWNLOAD_TRAINED_DATA
					return await TesseractDownloadLangFileAsync(dir, lang);
#else
					throw new FileNotFoundException("The specified file path can't be found.", filePath);
#endif
				}

				_ocr = new(dir, lang, OcrEngineMode.TesseractOnly, "123456789");
				return true;
			}
			catch
			{
				return false;
			}
		}

#if MUST_DOWNLOAD_TRAINED_DATA
		/// <summary>
		/// When the trained data is failed to find in the local machine, this method will download
		/// the file online.
		/// </summary>
		/// <param name="dir">The directory to find.</param>
		/// <param name="lang">The language.</param>
		/// <returns>The result.</returns>
		private async Task<bool> TesseractDownloadLangFileAsync(string dir, string lang)
		{
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			HttpClient? client = null;
			try
			{
				client = new();
				File.WriteAllText(
					$@"{dir}\{lang}.traineddata",
					await client.GetStringAsync(
						new Uri($"https://github.com/tesseract-ocr/tessdata/raw/master/{lang}.traineddata")
					)
				);

				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				client?.Dispose();
			}
		}
#endif
	}
}
#endif