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


	/// <summary>
	/// Creates a <see cref="ContentDialog"/> instance.
	/// </summary>
	/// <param name="title">The title.</param>
	/// <param name="message">The message.</param>
	/// <returns>The result instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ContentDialog CreateErrorDialog(string title, string message)
		=> new()
		{
			XamlRoot = XamlRoot,
			Title = title,
			Content = message,
			CloseButtonText = R["Close"],
			DefaultButton = ContentDialogButton.Close
		};

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
			await CreateErrorDialog(
				R["DocumentationPage_FailedToLoadFile"]!,
				$"""
				{R["DocumentationPage_FailedToLoadFileDialogContent"]!}
				
				{ex.Message}
				"""
			).ShowAsync();
		}
	}


#if ESCAPE_NESTED_INLINE_CODE_BLOCK
	/// <summary>
	/// Escapes the markdown text to avoid the exception thrown in runtime for the markdown text block control.
	/// </summary>
	/// <param name="markdownText">The markdown text.</param>
	/// <returns>The escaped text.</returns>
	private static string EscapeMarkdown(string markdownText)
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
		if ((_cMarkdownTextBlock, e) is not ({ UriPrefix: var uriPrefix }, { Link: var link }))
		{
			return;
		}

		if (Uri.TryCreate(link, UriKind.Absolute, out var uri))
		{
			await Launcher.LaunchUriAsync(uri);
			return;
		}

		if (await tryDownloadStringAsync($"{uriPrefix}{link}.md", _cMarkdownTextBlock))
		{
			return;
		}

		if (await tryDownloadStringAsync($"{uriPrefix}{link}/index.md", _cMarkdownTextBlock))
		{
			return;
		}

		await CreateErrorDialog(
			R["DocumentationPage_FailedToLoadFile"]!,
			R["DocumentationPage_MarkdownFileNotFound"]!
		).ShowAsync();


		static async Task<bool> tryDownloadStringAsync(string link, MarkdownTextBlock control)
		{
			using var httpClient = new HttpClient();
			if (Uri.TryCreate(link, UriKind.Absolute, out var uri))
			{
				string? text = null;
				try
				{
					text = await httpClient.GetStringAsync(uri);
				}
				catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
				{
				}

				if (text is not null)
				{
					control.Text =
#if ESCAPE_NESTED_INLINE_CODE_BLOCK
						EscapeMarkdown(text);
#else
						text;
#endif

					return true;
				}
			}

			control.Text = string.Empty;
			return false;
		}
	}
}
