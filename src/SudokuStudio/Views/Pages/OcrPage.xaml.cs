using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.Storage;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents an OCR page.
/// </summary>
public sealed partial class OcrPage : Page
{
	/// <summary>
	/// Initializes an <see cref="OcrPage"/> instance.
	/// </summary>
	public OcrPage() => InitializeComponent();


	private async void ReadPictureButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (!App.EnsureUnsnapped())
		{
			return;
		}

		var fop = new FileOpenPicker();
		fop.Initialize(this);
		fop.ViewMode = PickerViewMode.Thumbnail;
		fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fop.AddFileFormat(FileFormats.PortablePicture);

		if (await fop.PickSingleFileAsync() is not { Path: var filePath })
		{
			return;
		}

		using var stream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);

		// Decode the picture.
		var decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.PngDecoderId, stream);
		var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

		// Create an engine to be used for the later recognition.
		if (OcrEngine.TryCreateFromLanguage(new("en-US")) is not { } engine)
		{
			ErrorDialog_LanguageIsNotSupportedForNow.IsOpen = true;
			return;
		}

		var result = await engine.RecognizeAsync(softwareBitmap);

		// Parses the OCR result.
		var sb = new StringBuilder();
		foreach (var line in result.Lines)
		{
			foreach (var word in line.Words)
			{
				// Recognition: Iterate the OCR result in order to fetch the core data.
				//var bounds = word.BoundingRect;
				var text = word.Text;

				// TODO: Use them.
			}
		}
	}
}
