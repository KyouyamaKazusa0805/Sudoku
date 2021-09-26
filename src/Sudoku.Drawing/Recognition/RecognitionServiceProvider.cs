namespace Sudoku.Recognition;

/// <summary>
/// Define a sudoku recognition service provider.
/// </summary>
public sealed class RecognitionServiceProvider : IDisposable
{
	/// <summary>
	/// Indicates the internal recognition service provider.
	/// </summary>
	private readonly InternalServiceProvider _recognizingServiceProvider;


	/// <summary>
	/// Initializes a default <see cref="RecognitionServiceProvider"/> instance.
	/// </summary>
	public RecognitionServiceProvider()
	{
		string folder = $@"{Directory.GetCurrentDirectory()}\tessdata";
		_recognizingServiceProvider = new();
		_recognizingServiceProvider.InitTesseract(folder);
	}


	/// <summary>
	/// Indicates whether the OCR tool has already initialized.
	/// </summary>
	public bool IsInitialized => _recognizingServiceProvider.Initialized;


	/// <inheritdoc/>
	public void Dispose() => _recognizingServiceProvider.Dispose();

	/// <summary>
	/// Recognize the image.
	/// </summary>
	/// <param name="image">The image.</param>
	/// <returns>The grid.</returns>
	/// <exception cref="RecognizerNotInitializedException">
	/// Throws when the tool has not initialized yet.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Grid Recognize(Bitmap image)
	{
		if (IsInitialized)
		{
			using var gridRecognizer = new GridRecognizer(image);
			return _recognizingServiceProvider.RecognizeDigits(gridRecognizer.Recognize());
		}

		throw new RecognizerNotInitializedException();
	}
}
