namespace System.Text.Markdown;

/// <summary>
/// Provides with a factory that can create a markdown element.
/// </summary>
public static class MarkdownElementFactory
{
	/// <summary>
	/// Creates an image block <c>[path](url)</c>.
	/// </summary>
	/// <param name="path">
	/// The path of the image. The value can be <see langword="null"/> if you don't want to set any value.
	/// </param>
	/// <param name="imageUrl">The link to the image.</param>
	/// <returns>The markdown code describing an image.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ImageBlock(string? path, string imageUrl) => $"[{path}]({imageUrl})";

	/// <summary>
	/// Creates an inlined code block <c>`code`</c>.
	/// </summary>
	/// <param name="code">The code.</param>
	/// <returns>The markdown code describing an inlined code snippet.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string InliningCodeBlock(string code) => InliningCodeBlock(code, 1);

	/// <summary>
	/// Creates an inlined code block, with the specified number of backticks <c>"`"</c>
	/// appeared in both delimiters.
	/// </summary>
	/// <param name="code">The code.</param>
	/// <param name="backticksCount">The number of backticks <c>"`"</c> appeared in both delimiters.</param>
	/// <returns>The markdown code describing an inlined code snippet.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string InliningCodeBlock(string code, int backticksCount)
	{
		string backtickDelimiter = new('`', backticksCount);
		if (code.Trim() == "`")
		{
			code = @"\` ";
		}

		return $"{backtickDelimiter}{code}{backtickDelimiter}";
	}
}
