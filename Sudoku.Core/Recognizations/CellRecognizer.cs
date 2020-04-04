#if SUDOKU_RECOGNIZING
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Sudoku.Data;

namespace Sudoku.Recognizations
{
	/// <summary>
	/// Define a cell recognizer.
	/// </summary>
	public sealed class CellRecognizer
	{
		/// <summary>
		/// The internal <see cref="Tesseract"/> instance.
		/// </summary>
		private static Tesseract _ocr = null!;


		/// <summary>
		/// Initializes <see cref="Tesseract"/> instance.
		/// </summary>
		/// <param name="dir">The directory.</param>
		/// <param name="lang">The language. The default value is <c>"eng"</c>.</param>
		/// <returns>The task.</returns>
		/// <exception cref="InvalidOperationException">
		/// Throws when the specified trained data cannot be found and cannot be downloaded.
		/// </exception>
		public static async Task InitTesseractAsync(string dir, string lang = "eng")
		{
			try
			{
				if (!File.Exists($@"{dir}\{lang}.traineddata"))
				{
					await TesseractDownloadLangFileAsync(dir, lang);
				}
			}
			catch (Exception)
			{
				throw new InvalidOperationException(
					"Tessaract Error. Do not have a file and cannot to download it.");
			}

			_ocr = new Tesseract(dir, lang, OcrEngineMode.TesseractOnly, "123456789");
		}

		/// <summary>
		/// Recognizes digits.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="rectangles">(<see langword="out"/> parameter) The rectangles.</param>
		/// <returns>The grid.</returns>
		/// <exception cref="Exception">
		/// Throws when the processing is wrong or unhandleable.
		/// </exception>
		public static Grid RecognizeDigits(Image<Bgr, byte> field, out Rectangle[] rectangles)
		{
			var result = Grid.Empty.Clone();
			rectangles = new Rectangle[81];

			int width = field.Width / 9;
			int offset = width / 6;
			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 9; y++)
				{
					// Get image from center cell.
					rectangles[x * 9 + y] =
						new Rectangle(
							offset + width * x, offset + width * y,
							width - offset * 2, width - offset * 2);

					// Recognize digit from cell.
					int digit = RecognizeCellNumber(field.GetSubRect(rectangles[x * 9 + y]));
					if (digit == 0)
					{
						continue;
					}

					if (result[x * 9 + y, digit - 1])
					{
						throw new Exception($"Recognition error. Don't possible value at cell r{x + 1}c{y + 1}.");
					}

					result[x * 9 + y] = digit - 1;
				}
			}

			return result;
		}

		private static int RecognizeCellNumber(Image<Bgr, byte> cellImg)
		{
			// Convert the image to gray-scale and filter out the noise
			var imgGray = new Mat();
			CvInvoke.CvtColor(cellImg, imgGray, ColorConversion.Bgr2Gray);

			// TODO: can be problem with values for some image.
			// Another methods to process image, but worse. Use only one!
			var imgThresholded = new Mat();
			CvInvoke.Threshold(
				imgGray, imgThresholded, InternalSettings.ThOcrMin, InternalSettings.ThOcrMax,
				ThresholdType.Binary);
			//CvInvoke.AdaptiveThreshold(
			//	imgGrey, imgThresholded, 255, AdaptiveThresholdType.GaussianC,
			//	ThresholdType.Binary, 51, -1);

			_ocr.SetImage(imgThresholded);

			if (_ocr.Recognize() != 0)
			{
				throw new InvalidOperationException("Tessaract Error. Cannot to recognize cell image.");
			}

			var characters = _ocr.GetCharacters();
			string number = string.Empty;
			foreach (var c in characters)
			{
				if (c.Text != " ")
				{
					number += c.Text;
				}
			}

			if (number.Length > 1)
			{
				return 0;
			}

			int.TryParse(number, out int digit);
			return digit;
		}

		private static async Task TesseractDownloadLangFileAsync(string dir, string lang)
		{
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			using var httpClient = new HttpClient();
			string content = await httpClient.GetStringAsync(
				new Uri($"https://github.com/tesseract-ocr/tessdata/raw/master/{lang}.traineddata"));
			File.WriteAllText($@"{dir}\{lang}.traineddata", content);
		}
	}
}
#endif