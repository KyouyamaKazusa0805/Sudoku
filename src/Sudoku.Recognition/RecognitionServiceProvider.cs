using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sudoku.Data;

namespace Sudoku.Recognition
{
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
#if MUST_DOWNLOAD_TRAINED_DATA
			_recognizingServiceProvider.InitTesseractAsync(folder).Wait();
#else
			_recognizingServiceProvider.InitTesseract(folder);
#endif
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
		public SudokuGrid Recognize(Bitmap image)
		{
			if (IsInitialized)
			{
				using var gridRecognizer = new GridRecognizer(image);
				return _recognizingServiceProvider.RecognizeDigits(gridRecognizer.Recognize());
			}

			throw new RecognizerNotInitializedException();
		}

		/// <summary>
		/// Recognize the image asynchronously.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <returns>The task.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async ValueTask<SudokuGrid> RecognizeAsync(Bitmap image) => await new ValueTask<SudokuGrid>(Recognize(image));
	}
}
