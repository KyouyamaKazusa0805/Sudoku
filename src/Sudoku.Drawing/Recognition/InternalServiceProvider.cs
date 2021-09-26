namespace Sudoku.Recognition;

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
	/// <exception cref="FailedToFillValueException">
	/// Throws when the processing is wrong or unhandleable.
	/// </exception>
	public Grid RecognizeDigits(Field field)
	{
		var result = Grid.Empty;
		int w = field.Width / 9, o = w / 6;
		for (int x = 0; x < 9; x++)
		{
			for (int y = 0; y < 9; y++)
			{
				// Recognize digit from cell.
				int recogizedResult = RecognizeCellNumber(
					field.GetSubRect(new(o + w * x, o + w * y, w - o * 2, w - o * 2))
				);
				if (recogizedResult != -1)
				{
					int cell = x * 9 + y, digit = recogizedResult - 1;
					if (!result[cell, digit])
					{
						throw new FailedToFillValueException(cell, digit);
					}

					result[cell] = digit;
				}
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
	/// <exception cref="ArgumentNullException">Throws when the inner tool isn't been initialized.</exception>
	/// <exception cref="TesseractException">Throws when the OCR engine error.</exception>
	private int RecognizeCellNumber(Field cellImg)
	{
		ArgumentNullException.ThrowIfNull(_ocr);

		// Convert the image to gray-scale and filter out the noisy points.
		var imgGray = new Mat();
		Cv.CvtColor(cellImg, imgGray, ColorConversion.Bgr2Gray);

		// TODO: Problematic for some images.
		var imgThresholds = new Mat();
		Cv.Threshold(imgGray, imgThresholds, ThOcrMin, ThOcrMax, ThresholdType.Binary);

		_ocr.SetImage(imgThresholds);
		if (_ocr.Recognize() != 0)
		{
			throw new TesseractException("can't recognize any cell image");
		}

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

	/// <summary>
	/// Initializes <see cref="Tesseract"/> instance.
	/// </summary>
	/// <param name="dir">The directory.</param>
	/// <param name="lang">The language. The default value is <c>"eng"</c>.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	/// <exception cref="FileNotFoundException">Throws when the file doesn't found.</exception>
	[MemberNotNullWhen(true, nameof(_ocr))]
	public bool InitTesseract(string dir, string lang = "eng")
	{
		try
		{
			string filePath = $@"{dir}\{lang}.traineddata";
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(
					"The training file can't be found in the folder 'traineddata'.",
					filePath
				);
			}

			_ocr = new(dir, lang, OcrEngineMode.TesseractOnly, "123456789");
			return true;
		}
		catch
		{
			return false;
		}
	}
}
