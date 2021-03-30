namespace System.Text.Markdown
{
	/// <summary>
	/// Provides the symbols of the markdown symbols.
	/// </summary>
	public static class MarkdownSymbols
	{
		/// <summary>
		/// Indicates the inline code block start character.
		/// </summary>
		public const char InlineCodeBlockStartSingle = '`';

		/// <summary>
		/// Indicates the inline code block end character.
		/// </summary>
		public const char InlineCodeBlockEndSingle = '`';

		/// <summary>
		/// Indicates the inline LaTeX block start character.
		/// </summary>
		public const char InlineLatexBlockStartSingle = '$';

		/// <summary>
		/// Indicates the inline LaTeX block end character.
		/// </summary>
		public const char InlineLatexBlockEndSingle = '$';

		/// <summary>
		/// The header 1.
		/// </summary>
		public const string H1 = "# ";

		/// <summary>
		/// The header 2.
		/// </summary>
		public const string H2 = "## ";

		/// <summary>
		/// The header 3.
		/// </summary>
		public const string H3 = "### ";

		/// <summary>
		/// The header 4.
		/// </summary>
		public const string H4 = "#### ";

		/// <summary>
		/// The header 5.
		/// </summary>
		public const string H5 = "##### ";

		/// <summary>
		/// The header 6.
		/// </summary>
		public const string H6 = "###### ";

		/// <summary>
		/// Indicates the inline code block start.
		/// </summary>
		public const string InlineCodeBlockStart = "`";

		/// <summary>
		/// Indicates the inline code block start (double tildes).
		/// </summary>
		public const string InlineCodeBlockStartDouble = "``";

		/// <summary>
		/// Indicates the inline code block end.
		/// </summary>
		public const string InlineCodeBlockEnd = "`";

		/// <summary>
		/// Indicates the inline code block end (double tildes).
		/// </summary>
		public const string InlineCodeBlockEndDouble = "``";

		/// <summary>
		/// Indicates the code block start.
		/// </summary>
		public const string CodeBlockStart = "```";

		/// <summary>
		/// Indicates the code block end.
		/// </summary>
		public const string CodeBlockEnd = "```";

		/// <summary>
		/// Indicates the bold block start.
		/// </summary>
		public const string BoldBlockStart = "**";

		/// <summary>
		/// Indicates the bold block end.
		/// </summary>
		public const string BoldBlockEnd = "**";

		/// <summary>
		/// Indicates the italic block start.
		/// </summary>
		public const string ItalicBlockStart = "*";

		/// <summary>
		/// Indicates the italic block end.
		/// </summary>
		public const string ItalicBlockEnd = "*";

		/// <summary>
		/// Indicates the LaTeX block start.
		/// </summary>
		public const string LatexBlockStart = "$$";

		/// <summary>
		/// Indicates the LaTeX block end.
		/// </summary>
		public const string LatexBlockEnd = "$$";

		/// <summary>
		/// Indicates the inline LaTeX block start.
		/// </summary>
		public const string InlineLatexBlockStart = "$";

		/// <summary>
		/// Indicates the inline LaTeX block end.
		/// </summary>
		public const string InlineLatexBlockEnd = "$";

		/// <summary>
		/// Indicates the delete block start.
		/// </summary>
		public const string DeleteBlockStart = "~~";

		/// <summary>
		/// Indicates the delete block end.
		/// </summary>
		public const string DeleteBlockEnd = "~~";

		/// <summary>
		/// Indicates the picture block start.
		/// </summary>
		public const string PictureBlockStart = "![";

		/// <summary>
		/// Indicates the picture block middle.
		/// </summary>
		public const string PictureBlockMiddle = "](";

		/// <summary>
		/// Indicates the picture block end.
		/// </summary>
		public const string PictureBlockEnd = ")";

		/// <summary>
		/// Indicates the hyperlink block start.
		/// </summary>
		public const string HyperlinkBlockStart = "[";

		/// <summary>
		/// Indicates the hyperlink block middle.
		/// </summary>
		public const string HyperlinkBlockMiddle = "](";

		/// <summary>
		/// Indicates the hyperlink block end.
		/// </summary>
		public const string HyperlinkBlockEnd = ")";
	}
}
