#define ESCAPE_NESTED_INLINE_CODE_BLOCK

namespace Sudoku.UI.Views.Pages;

/// <summary>
/// Indicates the documentation page.
/// </summary>
public sealed partial class DocumentationPage : Page
{
	/// <summary>
	/// Initializes a <see cref="DocumentationPage"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DocumentationPage() => InitializeComponent();


#if ESCAPE_NESTED_INLINE_CODE_BLOCK
	/// <summary>
	/// Escapes the markdown text to avoid the exception thrown in runtime for the markdown text block control.
	/// </summary>
	/// <param name="markdownText">The markdown text.</param>
	/// <returns>The escaped text.</returns>
	private string EscapeMarkdown(string markdownText)
		// At present, the control may cause exception on parsing a nested rendering.
		// For example, an inline code block nested in an image block (e.g. [`inline` text](link)) may reproduce a bug:
		// https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/2802
		=> NestedInlineCodeBlockInImageBlockPattern().Replace(
			markdownText,
			static m =>
				m.Groups is [_, { Value: var e }, { Value: var a }, { Value: var b }, { Value: var c }, { Value: var d }]
					? $"[{e}{a}{b}{c}{e}]({d})"
					: string.Empty
		);
#endif

	/// <summary>
	/// Loads the local documentation file.
	/// </summary>
	/// <returns>The task that operates the current method.</returns>
	private async Task LoadAssetFileAsync()
	{
		const string indexPageUrl = """https://raw.githubusercontent.com/SunnieShine/Sudoku/main/docs/index.md""";

		using var httpClient = new HttpClient();
		try
		{
			_cMarkdownTextBlock.Text =
#if ESCAPE_NESTED_INLINE_CODE_BLOCK
				EscapeMarkdown(await httpClient.GetStringAsync(indexPageUrl));
#else
				await httpClient.GetStringAsync(indexPageUrl);
#endif
		}
		catch (Exception ex)
		{
			await new ContentDialog
			{
				XamlRoot = XamlRoot,
				Title = Get("DocumentationPage_FailedToLoadFile"),
				Content =
					$"""
					{Get("DocumentationPage_FailedToLoadFileDialogContent")}
					
					{ex.Message}
					""",
				DefaultButton = ContentDialogButton.Close
			}.ShowAsync();
		}
	}


#if ESCAPE_NESTED_INLINE_CODE_BLOCK
	/// <summary>
	/// Defines a pattern that matches an inline code block nested in the image block. For example,
	/// <c>[`inline` block](link)</c>.
	/// </summary>
	/// <returns>The regular expression pattern created.</returns>
	[RegexGenerator("""\[(\*{2})?(.*)`(\w+)`(.*)\1\]\(([\w\-/]*)\)""")]
	private static partial Regex NestedInlineCodeBlockInImageBlockPattern();
#endif


	/// <summary>
	/// Triggers when the current page is loaded.
	/// </summary>
	/// <param name="sender">The object which triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void Page_LoadedAsync(object sender, RoutedEventArgs e) => await LoadAssetFileAsync();

	/// <summary>
	/// Triggers when the link in a markdown document is clicked.
	/// </summary>
	/// <param name="sender">The object which triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private async void MarkdownTextBlock_LinkClickedAsync(object sender, LinkClickedEventArgs e)
	{
		if (Uri.TryCreate($"{_cMarkdownTextBlock.UriPrefix}{e.Link}", UriKind.Absolute, out var uri))
		{
			await Launcher.LaunchUriAsync(uri);
		}
	}
}
