using System.Text.Markdown;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Document"/>.
	/// </summary>
	/// <seealso cref="Document"/>
	public static class DocumentEx
	{
		/// <summary>
		/// Append the title with the specified member kind.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="memberKind">The member kind.</param>
		/// <param name="title">The title.</param>
		/// <returns>The document itself.</returns>
		public static Document AppendTitle(this Document @this, MemberKind memberKind, string title)
		{
			@this.AppendHeaderText(2, $@"{memberKind switch
			{
				MemberKind.Field => "Field ",
				MemberKind.Constructor => "Constructor",
				MemberKind.PrimaryConstructor => "Primary Constructor",
				MemberKind.StaticConstructor => "Static Constructor",
				MemberKind.Property => "Property",
				MemberKind.Indexer => "Indexer",
				MemberKind.Event => "Event",
				MemberKind.Method => "Method",
				MemberKind.Operator => "Operator",
				MemberKind.ImplicitCast => "Implicit cast",
				MemberKind.ExplicitCast => "Explicit cast"
			}} {title}");

			return @this;
		}

		/// <summary>
		/// Append "summary" section text. If the inner text is <see langword="null"/>, this method
		/// will do nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The document itself.</returns>
		public static Document AppendSummary(this Document @this, string? text)
		{
			if (text is not null)
			{
				@this.AppendHeaderText(3, "Summary").AppendParagraph(text ?? string.Empty);
			}

			return @this;
		}

		/// <summary>
		/// Append "remarks" section text. If the inner text is <see langword="null"/>, this method
		/// will do nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The document itself.</returns>
		public static Document AppendRemarks(this Document @this, string? text)
		{
			if (text is not null)
			{
				@this.AppendHeaderText(3, "Remarks").AppendParagraph(text ?? string.Empty);
			}

			return @this;
		}
	}
}
