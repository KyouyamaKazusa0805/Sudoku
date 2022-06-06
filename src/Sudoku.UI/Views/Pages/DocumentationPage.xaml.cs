#define ESCAPE_NESTED_INLINE_CODE_BLOCK

namespace Sudoku.UI.Views.Pages;

/// <summary>
/// Indicates the documentation page.
/// </summary>
[Page]
public sealed partial class DocumentationPage : Page
{
	/// <summary>
	/// Indicates the page prefixes to visit. This collection is used as a cached collection,
	/// providing with a way backing to the last page.
	/// </summary>
	private readonly Stack<string> _pagePrefixes = new();


	/// <summary>
	/// Initializes a <see cref="DocumentationPage"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DocumentationPage() => InitializeComponent();


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
#if ESCAPE_NESTED_INLINE_CODE_BLOCK
			_cMarkdownTextBlock.Text = EscapeMarkdown(await httpClient.GetStringAsync(indexPageUrl));
#else
			_cMarkdownTextBlock.Text = await httpClient.GetStringAsync(indexPageUrl);
#endif
		}
		catch (Exception ex)
		{
			await SimpleControlFactory.CreateErrorDialog(
				this,
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
	/// <remarks>
	/// At present, the control may cause exception on parsing a nested rendering.
	/// For example, an inline code block nested in an image block (e.g. [`inline` text](link)) may reproduce a bug:
	/// <see href="https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/2802">Issue #2802</see>.
	/// </remarks>
	private static string EscapeMarkdown(string markdownText)
		=> NestedInlineCodeBlockInImageBlockPattern().Replace(
			markdownText,
			static m =>
				m.Groups is [_, { Value: var e }, { Value: var a }, { Value: var b }, { Value: var c }, { Value: var d }]
					? MarkdownElementFactory.ImageBlock($"{e}{a}{b}{c}{e}", d)
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

		string popped = _pagePrefixes.TryPop(out string? u) ? u : uriPrefix;
		if (await tryDownloadStringAsync($"{popped}{link}.md", _cMarkdownTextBlock))
		{
			return;
		}

		if (await tryDownloadStringAsync($"{popped}{link}/index.md", _cMarkdownTextBlock))
		{
			return;
		}

		await SimpleControlFactory.CreateErrorDialog(
			this,
			R["DocumentationPage_FailedToLoadFile"]!,
			R["DocumentationPage_MarkdownFileNotFound"]!
		).ShowAsync();


		async Task<bool> tryDownloadStringAsync(string link, MarkdownTextBlock control)
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
#if ESCAPE_NESTED_INLINE_CODE_BLOCK
					control.Text = EscapeMarkdown(text);
#else
					control.Text = text;
#endif

					_pagePrefixes.Push(uri.ToString()[..(uri.ToString().LastIndexOf('/') + 1)]);
					return true;
				}
			}

			control.Text = string.Empty;
			return false;
		}
	}
}
