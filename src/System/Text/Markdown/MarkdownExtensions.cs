using System.Runtime.CompilerServices;
using static System.Text.Markdown.MarkdownSymbols;

namespace System.Text.Markdown
{
	/// <summary>
	/// Provides extension methods for markdown utility on <see cref="StringBuilder"/> and
	/// <see cref="ValueStringBuilder"/>.
	/// </summary>
	/// <seealso cref="StringBuilder"/>
	/// <seealso cref="ValueStringBuilder"/>
	internal static class MarkdownExtensions
	{
		/// <summary>
		/// Append plain text.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty or whitespaces.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendText(this ref ValueStringBuilder @this, string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new FormatException("The text shouldn't be an empty string, whitespaces or null.");
			}

			@this.Append(text);
		}

		/// <summary>
		/// Append a paragraph with a new line.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty or whitespaces.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendParagraph(this ref ValueStringBuilder @this, string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new FormatException("The text shouldn't be an empty string, whitespaces or null.");
			}

			@this.Append(text);
			@this.AppendLine();
			@this.AppendLine();
		}

		/// <summary>
		/// Append inline code block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The inner text.</param>
		/// <param name="appendPaddingSpaces">
		/// Indicates whether the block will append padding spaces surrounded
		/// the block. If <see langword="true"/>,
		/// the method will append a trailing space and a leading space to the block.
		/// The default value is <see langword="false"/>.
		/// </param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> contains double tilde mark <c>"``"</c>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendInlineCodeBlock(
			this ref ValueStringBuilder @this, string text, bool appendPaddingSpaces)
		{
			if (text.Contains(InlineCodeBlockStartDouble))
			{
				throw new FormatException(
					$"The text can't contain double tilde mark \"{InlineCodeBlockStartDouble}\"."
				);
			}

			switch ((text.Contains(InlineCodeBlockStartSingle), appendPaddingSpaces))
			{
				case (true, true):
				{
					@this.Append(' ');
					@this.Append(InlineCodeBlockStartDouble);
					@this.Append(text);
					@this.Append(InlineCodeBlockEndDouble);
					@this.Append(' ');

					break;
				}
				case (true, false):
				{
					@this.Append(InlineCodeBlockStartDouble);
					@this.Append(text);
					@this.Append(InlineCodeBlockEndDouble);

					break;
				}
				case (false, true):
				{
					@this.Append(' ');
					@this.Append(InlineCodeBlockStartSingle);
					@this.Append(text);
					@this.Append(InlineCodeBlockEndSingle);
					@this.Append(' ');

					break;
				}
				default:
				{
					@this.Append(InlineCodeBlockStartSingle);
					@this.Append(text);
					@this.Append(InlineCodeBlockEndSingle);

					break;
				}
			}
		}

		/// <summary>
		/// Append the code block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="code">The inner code.</param>
		/// <param name="codelang">The code language.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendCodeBlock(this ref ValueStringBuilder @this, string code, string? codelang)
		{
			@this.Append(CodeBlockStart);
			@this.Append(codelang ?? string.Empty);
			@this.AppendLine();
			@this.Append(code);
			@this.AppendLine();
			@this.Append(CodeBlockEnd);
			@this.AppendLine();
			@this.AppendLine();
		}

		/// <summary>
		/// Append header text.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendHeaderText(this ref ValueStringBuilder @this, string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new FormatException("The text shouldn't be an empty string, whitespaces or null.");
			}

			@this.Append(H1);
			@this.Append(text);
			@this.AppendLine();
			@this.AppendLine();
		}

		/// <summary>
		/// Append header text, with the specified header level.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="level">The header level. The value should be between 1 and 6.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendHeaderText(this ref ValueStringBuilder @this, int level, string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new FormatException("The text shouldn't be an empty string, whitespaces or null.");
			}

			@this.Append(level switch { 1 => H1, 2 => H2, 3 => H3, 4 => H4, 5 => H5, 6 => H6 });
			@this.Append(text);
			@this.AppendLine();
			@this.AppendLine();
		}

		/// <summary>
		/// Append bold block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendBoldBlock(this ref ValueStringBuilder @this, string text)
		{
			@this.Append(BoldBlockStart);
			@this.Append(text);
			@this.Append(BoldBlockEnd);
		}

		/// <summary>
		/// Append italic block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendItalicBlock(this ref ValueStringBuilder @this, string text)
		{
			@this.Append(ItalicBlockStartSingle);
			@this.Append(text);
			@this.Append(ItalicBlockEndSingle);
		}

		/// <summary>
		/// Append delete block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendDeleteBlock(this ref ValueStringBuilder @this, string text)
		{
			@this.Append(DeleteBlockStart);
			@this.Append(text);
			@this.Append(DeleteBlockEnd);
		}

		/// <summary>
		/// Append hyperlink.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="description">
		/// The description of the hyperlink. If <see langword="null"/>, the description part will be replaced
		/// by <paramref name="uri"/>.
		/// </param>
		/// <param name="uri">Thr URI link.</param>
		/// <param name="withNewLine">
		/// A <see cref="bool"/> value indicates whether the end appends a new line. The default value
		/// is <see langword="false"/>.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendHyperlink(
			this ref ValueStringBuilder @this, string? description, string uri, bool withNewLine = false)
		{
			description ??= uri;

			@this.Append(HyperlinkBlockStart);
			@this.Append(description);
			@this.Append(HyperlinkBlockMiddle);
			@this.Append(uri);
			@this.Append(HyperlinkBlockEnd);

			if (withNewLine)
			{
				@this.AppendLine();
				@this.AppendLine();
			}
		}

		/// <summary>
		/// Append image block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="description">The description of the hyperlink.</param>
		/// <param name="uri">The URI link.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendImageBlock(this ref ValueStringBuilder @this, string? description, string uri)
		{
			@this.Append(ImageBlockStart);
			if (description is not null) @this.Append(description);
			@this.Append(ImageBlockMiddle);
			@this.Append(uri);
			@this.Append(ImageBlockEnd);
			@this.AppendLine();
			@this.AppendLine();
		}

		/// <summary>
		/// Append a new line.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AppendNewLine(this ref ValueStringBuilder @this)
		{
			@this.AppendLine();
			@this.AppendLine();
		}

		/// <summary>
		/// Append plain text.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty or whitespaces.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendText(this StringBuilder @this, string text) =>
			string.IsNullOrWhiteSpace(text)
			? throw new FormatException("The text shouldn't be an empty string, whitespaces or null.")
			: @this.Append(text);

		/// <summary>
		/// Append a paragraph with a new line.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty or whitespaces.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendParagraph(this StringBuilder @this, string text) =>
			string.IsNullOrWhiteSpace(text)
			? throw new FormatException("The text shouldn't be an empty string, whitespaces or null.")
			: @this.Append(text).AppendLine().AppendLine();

		/// <summary>
		/// Append inline code block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The inner text.</param>
		/// <param name="appendPaddingSpaces">
		/// Indicates whether the block will append padding spaces surrounded
		/// the block. If <see langword="true"/>,
		/// the method will append a trailing space and a leading space to the block.
		/// The default value is <see langword="false"/>.
		/// </param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> contains double tilde mark <c>"``"</c>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendInlineCodeBlock(
			this StringBuilder @this, string text, bool appendPaddingSpaces) =>
			text.Contains(InlineCodeBlockStartDouble)
			? throw new FormatException(
				$"The text can't contain double tilde mark \"{InlineCodeBlockStartDouble}\"."
			)
			: (text.Contains(InlineCodeBlockStartSingle), appendPaddingSpaces) switch
			{
				(true, true) => @this
					.Append(' ')
					.Append(InlineCodeBlockStartDouble)
					.Append(text)
					.Append(InlineCodeBlockEndDouble)
					.Append(' '),
				(true, false) => @this
					.Append(InlineCodeBlockStartDouble)
					.Append(text)
					.Append(InlineCodeBlockEndDouble),
				(false, true) => @this
					.Append(' ')
					.Append(InlineCodeBlockStartSingle)
					.Append(text)
					.Append(InlineCodeBlockEndSingle)
					.Append(' '),
				_ => @this
					.Append(InlineCodeBlockStartSingle)
					.Append(text)
					.Append(InlineCodeBlockEndSingle)
			};

		/// <summary>
		/// Append the code block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="code">The inner code.</param>
		/// <param name="codelang">The code language.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendCodeBlock(this StringBuilder @this, string code, string? codelang) =>
			@this
				.Append(CodeBlockStart).Append(codelang ?? string.Empty).AppendLine()
				.Append(code).AppendLine()
				.Append(CodeBlockEnd).AppendLine().AppendLine();

		/// <summary>
		/// Append header text.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendHeaderText(this StringBuilder @this, string text) =>
			string.IsNullOrWhiteSpace(text)
			? throw new FormatException("The text shouldn't be an empty string, whitespaces or null.")
			: @this.Append(H1).Append(text).AppendLine().AppendLine();

		/// <summary>
		/// Append header text, with the specified header level.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="level">The header level. The value should be between 1 and 6.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The current instance.</returns>
		/// <exception cref="FormatException">
		/// Throws when the <paramref name="text"/> is <see langword="null"/>, empty
		/// or only contains whitespace characters.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendHeaderText(this StringBuilder @this, int level, string text) =>
			string.IsNullOrWhiteSpace(text)
			? throw new FormatException("The text shouldn't be an empty string, whitespaces or null.")
			: @this
				.Append(level switch { 1 => H1, 2 => H2, 3 => H3, 4 => H4, 5 => H5, 6 => H6 })
				.Append(text)
				.AppendLine()
				.AppendLine();

		/// <summary>
		/// Append bold block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendBoldBlock(this StringBuilder @this, string text) =>
			@this
				.Append(BoldBlockStart)
				.Append(text)
				.Append(BoldBlockEnd);

		/// <summary>
		/// Append italic block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendItalicBlock(this StringBuilder @this, string text) =>
			@this
				.Append(ItalicBlockStartSingle)
				.Append(text)
				.Append(ItalicBlockEndSingle);

		/// <summary>
		/// Append delete block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="text">The text.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendDeleteBlock(this StringBuilder @this, string text) =>
			@this
				.Append(DeleteBlockStart)
				.Append(text)
				.Append(DeleteBlockEnd);

		/// <summary>
		/// Append hyperlink.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="description">
		/// The description of the hyperlink. If <see langword="null"/>, the description part will be replaced
		/// by <paramref name="uri"/>.
		/// </param>
		/// <param name="uri">Thr URI link.</param>
		/// <param name="appendNewLine">
		/// A <see cref="bool"/> value indicates whether the end appends a new line. The default value
		/// is <see langword="false"/>.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendHyperlink(
			this StringBuilder @this, string? description, string uri, bool appendNewLine = false)
		{
			description ??= uri;

			@this
				.Append(HyperlinkBlockStartSingle)
				.Append(description)
				.Append(HyperlinkBlockMiddle)
				.Append(uri)
				.Append(HyperlinkBlockEndSingle);

			return appendNewLine ? @this.AppendLine().AppendLine() : @this;
		}

		/// <summary>
		/// Append image block.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <param name="description">The description of the hyperlink.</param>
		/// <param name="uri">The URI link.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendImageBlock(this StringBuilder @this, string? description, string uri)
		{
			@this.Append(ImageBlockStart);

			return description is not null
				? @this.Append(description)
				: @this
					.Append(ImageBlockMiddle)
					.Append(uri)
					.Append(ImageBlockEndSingle)
					.AppendLine()
					.AppendLine();
		}

		/// <summary>
		/// Append a new line.
		/// </summary>
		/// <param name="this">The current instance.</param>
		/// <returns>The current instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder AppendNewLine(this StringBuilder @this) => @this.AppendLine().AppendLine();
	}
}
