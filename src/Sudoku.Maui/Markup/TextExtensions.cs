namespace Sudoku.Maui.Markup;

/// <summary>
/// Defines a text resource fetcher.
/// </summary>
[ContentProperty(nameof(Key))]
public sealed class TextExtension : IMarkupExtension<string>, IMarkupExtension
{
	/// <summary>
	/// Initializes a <see cref="TextExtension"/> instance.
	/// </summary>
	public TextExtension() => Key = string.Empty;


	/// <summary>
	/// The resource key.
	/// </summary>
	public string Key { get; set; }


	/// <inheritdoc/>
	public string ProvideValue(IServiceProvider serviceProvider) => App.ResourceTextFetcherCommon(Key);

	/// <inheritdoc/>
	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}
