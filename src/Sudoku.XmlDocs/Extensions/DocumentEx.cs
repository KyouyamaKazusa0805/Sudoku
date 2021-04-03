using System.Linq;
using System.Text.Markdown;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
				MemberKind.Property => "Property",
				MemberKind.Indexer => "Indexer",
				MemberKind.Event => "Event",
				MemberKind.Method => "Method",
				MemberKind.Operator => "Operator",
				MemberKind.ImplicitCast => "Implicit Cast",
				MemberKind.ExplicitCast => "Explicit Cast"
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
				@this
					.AppendHeaderText(3, DocComments.Summary)
					.AppendParagraph(text ?? string.Empty);
			}

			return @this;
		}

		/// <summary>
		/// Append "returns" section text. If the inner text is <see langword="null"/>,
		/// the specified node doesn't return any values (such as field, etc.), or
		/// the node returns <see langword="void"/>, this method will do nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="text">The inner text.</param>
		/// <param name="node">The node to check.</param>
		/// <returns>The document itself.</returns>
		public static Document AppendReturns(this Document @this, string? text, MemberDeclarationSyntax node)
		{
			switch (node)
			{
				case BasePropertyDeclarationSyntax { AccessorList: { Accessors: var accessors } }
				when accessors.Any(static node => node.Keyword.IsKeyword(Keywords.GetKeyword)):
				{
					break;
				}
				case MethodDeclarationSyntax { ReturnType: PredefinedTypeSyntax { Keyword: var keyword } }
				when keyword.IsKeyword(Keywords.VoidKeyword):
				{
					goto Returning;
				}
				default:
				{
					goto Returning;
				}
			}

			if (text is null)
			{
				goto Returning;
			}

			@this.AppendHeaderText(3, DocComments.Returns).AppendParagraph(text ?? string.Empty);

		Returning:
			return @this;
		}

		/// <summary>
		/// Append "remarks" section text. If the inner text is <see langword="null"/>,
		/// this method will do nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The document itself.</returns>
		public static Document AppendRemarks(this Document @this, string? text)
		{
			if (text is not null)
			{
				@this
					.AppendHeaderText(3, DocComments.Remarks)
					.AppendParagraph(text ?? string.Empty);
			}

			return @this;
		}

		/// <summary>
		/// Append "example" section text. If the inner text is <see langword="null"/>,
		/// this method will do nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="text">The inner text.</param>
		/// <returns>The document itself.</returns>
		public static Document AppendExample(this Document @this, string? text)
		{
			if (text is not null)
			{
				@this
					.AppendHeaderText(3, DocComments.Example)
					.AppendParagraph(text ?? string.Empty);
			}

			return @this;
		}

		/// <summary>
		/// Append "exception" section text. If the exceptions is <see langword="null"/>, it'll do nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="exceptions">The exceptions.</param>
		/// <returns>The document itself.</returns>
		public static Document AppendException(
			this Document @this, (string Exception, string? Description)[]? exceptions)
		{
			if (exceptions is null)
			{
				goto Returning;
			}

			@this.AppendHeaderText(3, DocComments.Exception);

			foreach (var (exceptionName, description) in exceptions)
			{
				@this
					.AppendHeaderText(4, exceptionName)
					.AppendParagraph(description ?? string.Empty);
			}

		Returning:
			return @this;
		}

		/// <summary>
		/// Append "value" section text. If the inner text is <see langword="null"/>, the member
		/// isn't neither a property nor a indexer, or the member doesn't contain
		/// neither <see langword="set"/>ter nor <see langword="init"/>er, this method will do
		/// nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="text">The inner text.</param>
		/// <param name="node">The property or indexer syntax node.</param>
		/// <returns>The document itself.</returns>
		public static Document AppendValue(this Document @this, string? text, BasePropertyDeclarationSyntax node)
		{
			// Check whether the text isn't null.
			if (text is null)
			{
				goto Returning;
			}

			// Check whether the accessor list isn't empty.
			if (node.AccessorList?.Accessors is not { } accessors)
			{
				goto Returning;
			}

			// Check whether the current member contains the setter or initer.
			if (accessors.All(static node => !node.Keyword.IsKeyword(Keywords.SetKeyword, Keywords.InitKeyword)))
			{
				goto Returning;
			}

			@this.AppendHeaderText(3, DocComments.Value).AppendParagraph(text ?? string.Empty);

		Returning:
			return @this;
		}

		/// <summary>
		/// Append "param" section text. If the parameter can't be found in the real parameter list,
		/// this method will do nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="parameters">The parameter list.</param>
		/// <param name="node">The node to check.</param>
		/// <returns>The current document.</returns>
		public static Document AppendParams(
			this Document @this, (string ParamName, string Type, string? Description)[]? parameters,
			BaseMethodDeclarationSyntax node)
		{
			if (node.ParameterList?.Parameters is { } parameterList)
			{
				if (parameters is null || parameterList.Count == 0)
				{
					goto Returning;
				}

				bool fastChecking = false;
				foreach (var (paramName, _, description) in parameters)
				{
					if (parameterList.Any(param => param.Identifier.ValueText == paramName))
					{
						fastChecking = true;
						break;
					}
				}
				if (!fastChecking)
				{
					goto Returning;
				}

				@this.AppendHeaderText(3, DocComments.Parameter);

				foreach (var (paramName, type, description) in parameters)
				{
					if (parameterList.Any(param => param.Identifier.ValueText == paramName))
					{
						@this
							.AppendHeaderText(4, paramName)
							.AppendBoldBlock("Type: ")
							.AppendText(type)
							.AppendBoldBlock("Description: ")
							.AppendNewLine()
							.AppendParagraph(description ?? string.Empty);
					}
				}
			}
			else if (parameters is not null)
			{
				goto Returning;
			}

		Returning:
			return @this;
		}

		/// <summary>
		/// Append "param" section text. If the parameter can't be found in the real parameter list,
		/// this method will do nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="parameters">The parameter list.</param>
		/// <param name="node">The node to check.</param>
		/// <returns>The current document.</returns>
		public static Document AppendParams(
			this Document @this, (string ParamName, string Type, string? Description)[]? parameters,
			RecordDeclarationSyntax node)
		{
			if (node.ParameterList?.Parameters is { } parameterList)
			{
				if (parameters is null || parameterList.Count == 0)
				{
					goto Returning;
				}

				bool fastChecking = false;
				foreach (var (paramName, _, _) in parameters)
				{
					if (parameterList.Any(param => param.Identifier.ValueText == paramName))
					{
						fastChecking = true;
						break;
					}
				}
				if (!fastChecking)
				{
					goto Returning;
				}

				@this.AppendHeaderText(3, DocComments.Parameter);

				foreach (var (paramName, type, description) in parameters)
				{
					if (parameterList.Any(param => param.Identifier.ValueText == paramName))
					{
						@this
							.AppendHeaderText(4, paramName)
							.AppendBoldBlock("Type: ")
							.AppendText(type)
							.AppendBoldBlock("Description: ")
							.AppendNewLine()
							.AppendParagraph(description ?? string.Empty);
					}
				}
			}
			else if (parameters is not null)
			{
				goto Returning;
			}

		Returning:
			return @this;
		}

		/// <summary>
		/// Append "typeparam" section text. If the type parameter can't be found
		/// in the real type parameter list, this method will do nothing.
		/// </summary>
		/// <param name="this">The document.</param>
		/// <param name="typeParameters">The type parameter list.</param>
		/// <param name="node">The node to check.</param>
		/// <returns>The current document.</returns>
		public static Document AppendTypeParams(
			this Document @this, (string ParamName, string? Description)[]? typeParameters,
			MethodDeclarationSyntax node)
		{
			if (node.TypeParameterList?.Parameters is { } parameterList)
			{
				if (typeParameters is null || parameterList.Count == 0)
				{
					goto Returning;
				}

				bool fastChecking = false;
				foreach (var (paramName, description) in typeParameters)
				{
					if (parameterList.Any(param => param.Identifier.ValueText == paramName))
					{
						fastChecking = true;
						break;
					}
				}
				if (!fastChecking)
				{
					goto Returning;
				}

				@this.AppendHeaderText(3, DocComments.Parameter);

				foreach (var (paramName, description) in typeParameters)
				{
					if (parameterList.Any(param => param.Identifier.ValueText == paramName))
					{
						@this.AppendHeaderText(4, paramName).AppendParagraph(description ?? string.Empty);
					}
				}
			}
			else if (typeParameters is not null)
			{
				goto Returning;
			}

		Returning:
			return @this;
		}
	}
}
