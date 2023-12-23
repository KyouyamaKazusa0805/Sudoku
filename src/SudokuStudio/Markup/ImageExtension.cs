namespace SudokuStudio.Markup;

/// <summary>
/// Represents a bitmap markup that can be used for creating a <see cref="BitmapImage"/> instance without nesting declaration in XAML.
/// </summary>
[ContentProperty(Name = nameof(UriSource))]
[MarkupExtensionReturnType(ReturnType = typeof(BitmapImage))]
public sealed class ImageExtension : MarkupExtension
{
	/// <summary>
	/// Indicates whether the image is a GIF and should be automatically played.
	/// </summary>
	public bool AutoPlay { get; set; }

	/// <summary>
	/// Indicates the URI source.
	/// </summary>
	public Uri? UriSource { get; set; }


	/// <inheritdoc/>
	protected override object ProvideValue() => new BitmapImage { AutoPlay = AutoPlay, UriSource = UriSource };
}
