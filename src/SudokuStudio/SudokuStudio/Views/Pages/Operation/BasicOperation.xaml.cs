namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Indicates the basic operation command bar.
/// </summary>
public sealed partial class BasicOperation : Page, INotifyPropertyChanged
{
	/// <summary>
	/// Defines a default puzzle generator.
	/// </summary>
	private static readonly PatternBasedPuzzleGenerator Generator = new();


	/// <summary>
	/// Indicates the path of the saved file.
	/// </summary>
	[NotifyBackingField(Accessibility = GeneralizedAccessibility.Internal)]
	private string? _succeedFilePath;


	/// <summary>
	/// Initializes a <see cref="BasicOperation"/> instance.
	/// </summary>
	public BasicOperation() => InitializeComponent();


	/// <summary>
	/// Indicates the base page.
	/// </summary>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// To determine whether the current application view is in an unsnapped state.
	/// </summary>
	/// <returns>The <see cref="bool"/> value indicating that.</returns>
	private bool EnsureUnsnapped()
	{
		/// <see cref="FileOpenPicker"/> APIs will not work if the application is in a snapped state.
		/// If an app wants to show a <see cref="FileOpenPicker"/> while snapped, it must attempt to unsnap first.
		var unsnapped = ApplicationView.Value != ApplicationViewState.Snapped || ApplicationView.TryUnsnap();
		if (!unsnapped)
		{
			ErrorDialog_ProgramIsSnapped.IsOpen = true;
		}

		return unsnapped;
	}


	private async void OpenFileButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (!EnsureUnsnapped())
		{
			return;
		}

		var fop = new FileOpenPicker()
			.WithSuggestedStartLocation(PickerLocationId.DocumentsLibrary)
			.AddFileTypeFilter(CommonFileExtensions.Text)
			.AddFileTypeFilter(CommonFileExtensions.PlainText)
			.WithAwareHandleOnWin32();

		if (await fop.PickSingleFileAsync() is not { Path: var filePath } file)
		{
			return;
		}

		var fileInfo = new FileInfo(filePath);
		var (errorDialog, targetGrid) = fileInfo.Length switch
		{
			0 => (ErrorDialog_FileIsEmpty, null),
			> 1024 => (ErrorDialog_FileIsOversized, null),
			_ when await FileIO.ReadTextAsync(file) is var content => string.IsNullOrWhiteSpace(content) switch
			{
				true => (ErrorDialog_FileIsEmpty, null),
				false => Grid.TryParse(content, out var g) switch
				{
					false => (ErrorDialog_FileCannotBeParsed, null),
					true => g.IsValid() switch
					{
						false => (ErrorDialog_FileGridIsNotUnique, null),
						true => (null, g)
					}
				}
			},
			_ => default((TeachingTip?, Grid?))
		};

		if (errorDialog is not null)
		{
			errorDialog.IsOpen = true;

			return;
		}

		if (targetGrid is { } grid)
		{
			BasePage.SudokuPane.Puzzle = grid;
		}
	}

	private async void SaveFileButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (!EnsureUnsnapped())
		{
			return;
		}

		var fsp = new FileSavePicker()
			.WithSuggestedStartLocation(PickerLocationId.DocumentsLibrary)
			.WithSuggestedFileName(R["Sudoku"]!)
			.AddFileTypeChoice(GetString("FileExtension_TextDescription"), CommonFileExtensions.Text)
			.AddFileTypeChoice(GetString("FileExtension_PlainTextDescription"), CommonFileExtensions.PlainText)
			.AddFileTypeChoice(GetString("FileExtension_Picture"), CommonFileExtensions.PortablePicture)
			.WithAwareHandleOnWin32();

		_ = await fsp.PickSaveFileAsync() switch
		{
			{ Path: var filePath } file => Path.GetExtension(filePath) switch
			{
				CommonFileExtensions.Text or CommonFileExtensions.PlainText => await Helper.PlainTextSaveAsync(file, this),
				CommonFileExtensions.PortablePicture => await Helper.PictureSaveAsync(file, this),
				_ => false
			},
			_ => false
		};
	}

	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		BasePage.GeneratorIsNotRunning = false;

		var grid = await Generator.GenerateAsync();

		BasePage.GeneratorIsNotRunning = true;

		BasePage.SudokuPane.Puzzle = grid;
	}

	private void SaveAsButton_Click(object sender, RoutedEventArgs e) => Dialog_FormatChoosing.IsOpen = true;

	private async void Dialog_FormatChoosing_ActionButtonClickAsync(TeachingTip sender, object args)
	{
		if (!EnsureUnsnapped())
		{
			return;
		}

		var flags = (
			from children in FormatGroupPanel.Children
			where children is CheckBox { Tag: int and not 0, IsChecked: true }
			select (FormatFlags)(int)((CheckBox)children).Tag!
		).Aggregate(FormatFlags.None, static (interim, next) => interim | next);
		if (flags == FormatFlags.None)
		{
			return;
		}

		var puzzle = BasePage.SudokuPane.Puzzle;
		var newline = Environment.NewLine;
		var outputString = string.Join(
			$"{newline}{newline}",
			from object formatter in createFormatHandlers(flags)
			select ((IGridFormatter)formatter).ToString(puzzle)
		);

		var fsp = new FileSavePicker()
			.WithSuggestedStartLocation(PickerLocationId.DocumentsLibrary)
			.WithSuggestedFileName(R["Sudoku"]!)
			.AddFileTypeChoice(GetString("FileExtension_TextDescription"), CommonFileExtensions.Text)
			.AddFileTypeChoice(GetString("FileExtension_PlainTextDescription"), CommonFileExtensions.PlainText)
			.WithAwareHandleOnWin32();

		if (await fsp.PickSaveFileAsync() switch
		{
			{ Path: var filePath } file => Path.GetExtension(filePath) switch
			{
				CommonFileExtensions.Text or CommonFileExtensions.PlainText => await Helper.PlainTextSaveAsync(file, this, outputString),
				_ => false
			},
			_ => false
		})
		{
			Dialog_FormatChoosing.IsOpen = false;
		}


		static ArrayList createFormatHandlers(FormatFlags flags)
		{
			var formats = new ArrayList();
			foreach (var flag in flags.GetAllFlagsDistinct()!)
			{
				formats.Add(
					flag switch
					{
						FormatFlags.InitialFormat => SusserFormat.Default,
						FormatFlags.CurrentFormat => SusserFormat.Full
					}
				);
			}

			return formats;
		}
	}
}

/// <summary>
/// Represents a format flag.
/// </summary>
[Flags]
file enum FormatFlags : int
{
	/// <summary>
	/// Indicates the default format.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None = 0,

	/// <summary>
	/// Indicates the initial grid format.
	/// </summary>
	InitialFormat = 1,

	/// <summary>
	/// Indicates the current grid format.
	/// </summary>
	CurrentFormat = 1 << 1,

	/// <summary>
	/// Indicates the full formats. All format flags are included.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	Full = InitialFormat | CurrentFormat
}

/// <summary>
/// Represents a list of methods that handles and saves the files with supported sudoku formats.
/// </summary>
file static class Helper
{
	/// <summary>
	/// Saves with plain text format.
	/// </summary>
	/// <param name="file">The file.</param>
	/// <param name="page">The page control.</param>
	/// <returns>
	/// The asynchronous task that can return the <see cref="bool"/> result
	/// indicating whether the operation is succeeded.
	/// </returns>
	public static async Task<bool> PlainTextSaveAsync(StorageFile file, BasicOperation page)
	{
		if ((file, page) is not ({ Name: var fileName, Path: var filePath }, { BasePage.SudokuPane.Puzzle: var grid }))
		{
			return false;
		}

		var code = grid.ToString("#");

		await File.WriteAllTextAsync(filePath, code);

		var a = GetString("AnalyzePage_FileSaveSucceed_Segment1");
		var b = GetString("AnalyzePage_FileSaveSucceed_Segment2");
		page.SucceedFilePath = $"{a}{fileName}{b}";

		page.SuccessDialog_FileSavedSuccessfully.IsOpen = true;

		return true;
	}

	/// <summary>
	/// Saves with plain text format.
	/// </summary>
	/// <param name="file">The file.</param>
	/// <param name="page">The page control.</param>
	/// <param name="outputValue">The output value.</param>
	/// <returns>
	/// The asynchronous task that can return the <see cref="bool"/> result
	/// indicating whether the operation is succeeded.
	/// </returns>
	public static async Task<bool> PlainTextSaveAsync(StorageFile file, BasicOperation page, string outputValue)
	{
		if (file is not { Name: var fileName, Path: var filePath })
		{
			return false;
		}

		await File.WriteAllTextAsync(filePath, outputValue);

		var a = GetString("AnalyzePage_FileSaveSucceed_Segment1");
		var b = GetString("AnalyzePage_FileSaveSucceed_Segment2");
		page.SucceedFilePath = $"{a}{fileName}{b}";

		page.SuccessDialog_FileSavedSuccessfully.IsOpen = true;

		return true;
	}

	/// <summary>
	/// Saves with picture format.
	/// </summary>
	/// <param name="file">The file.</param>
	/// <param name="page">The page control.</param>
	/// <returns>
	/// The asynchronous task that can return the <see cref="bool"/> result
	/// indicating whether the operation is succeeded.
	/// </returns>
	public static async Task<bool> PictureSaveAsync(StorageFile file, BasicOperation page)
	{
		if ((file, page) is not ({ Name: var fileName }, { BasePage.SudokuPane: var pane }))
		{
			return false;
		}

		await pane.RenderToAsync(file);

		var a = GetString("AnalyzePage_FileSaveSucceed_Segment1");
		var b = GetString("AnalyzePage_FileSaveSucceed_Segment2");
		page.SucceedFilePath = $"{a}{fileName}{b}";

		page.SuccessDialog_FileSavedSuccessfully.IsOpen = true;

		return true;
	}
}

/// <include file='../../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Renders the specified UI control to the target file as a picture.
	/// </summary>
	/// <typeparam name="TUIElement">The type of the UI control.</typeparam>
	/// <param name="this">The control.</param>
	/// <param name="file">The target file.</param>
	public static async Task RenderToAsync<TUIElement>(this TUIElement @this, StorageFile file) where TUIElement : UIElement
	{
		// Render.
		var rtb = new RenderTargetBitmap();
		await rtb.RenderAsync(@this);

		// Creates the pixel buffer.
		var pixelBuffer = await rtb.GetPixelsAsync();

		// Gets the DPI value.
		var dpi = TryGetLogicalDpi();

		// Encodes the image to the selected file on disk.
		using var pictureFileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
		var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, pictureFileStream);
		encoder.SetPixelData(
			BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)rtb.PixelWidth, (uint)rtb.PixelHeight,
			dpi, dpi, pixelBuffer.ToArray());

		// Flushes the encoder.
		await encoder.FlushAsync();
	}

	/// <summary>
	/// Try to get the logical DPI value.
	/// </summary>
	/// <param name="default">The default DPI value. The default value is 96.</param>
	/// <returns>The DPI value to get.</returns>
	private static float TryGetLogicalDpi(float @default = 96)
	{
		float dpi;
		try
		{
			dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
		}
		catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x80070490))
		{
			// Cannot find the element.
			dpi = @default;
		}

		return dpi;
	}
}
