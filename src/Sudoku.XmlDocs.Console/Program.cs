//using Sudoku.XmlDocs;

//await new OutputService().ExecuteAsync();

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.XmlDocs;
using Sudoku.XmlDocs.Extensions;

const string testCode = @"
/// <summary>
/// <para>
/// This is an xml doc comment. <see cref=""T(ref int)""/>
/// <code>
/// foreach (var item in list)
/// {
///     Console.WriteLine(item.ToString());
/// }
/// </code>
/// This is a new line.
/// </para>
/// <list type=""bullet"">
/// <item>1</item>
/// <item>2</item>
/// <item>3</item>
/// </list>
/// </summary>
/// <remarks>
/// Remarks.
/// </remarks>
class C
{
    /// <summary>
    /// <para><paramref name=""p""/> is a.</para>
    /// <para><paramref name=""p""/> is b.</para>
    /// <para><paramref name=""p""/> is c.</para>
    /// </summary>
	/// <typeparam name=""TArg"">The type argument.</typeparam>
	/// <example>Examples.</example>
    public void T<TArg>(ref int p) where TArg : struct
    {
        p += 2;
    }
}";
char[] newLineCharacters = new[] { '\r', '\n', ' ' };
var root = CSharpSyntaxTree.ParseText(testCode).GetRoot();
var sb = new StringBuilder();
var emptyCharsRegex = new Regex(
	pattern: @"\s*\r\n\s*///\s*",
	options: RegexOptions.Compiled | RegexOptions.ExplicitCapture,
	matchTimeout: TimeSpan.FromSeconds(5)
);
var leadingTripleSlashRegex = new Regex(
	pattern: @"(?<=\r\n)\s*(///\s+?)",
	options: RegexOptions.Compiled | RegexOptions.ExplicitCapture,
	matchTimeout: TimeSpan.FromSeconds(5)
);

bool isWhiteOrTripleSlashOnly(XmlNodeSyntax node)
{
	string s = node.ToString();
	var match = emptyCharsRegex.Match(s);
	return match.Success && match.Value == s;
}

foreach (var decl in root.DescendantNodes().OfType<TypeDeclarationSyntax>())
{
	// Get its docs.
	decl.VisitDocDescendants(
		summaryNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) =>
		{
			foreach (var descendant in descendants)
			{
				traverse(descendant);
			}
		}
	);

	// Get all member docs.
	foreach (var member in decl.GetMembers(checkNestedTypes: true))
	{
		Console.Write("Member ");
		Console.WriteLine();

		member.VisitDocDescendants(
			summaryNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) =>
			{
				foreach (var descendant in descendants)
				{
					traverse(descendant);
				}
			}
		);
	}
}

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(sb.ToString());
Console.ResetColor();

bool traverse(XmlNodeSyntax descendant)
{
	if (isWhiteOrTripleSlashOnly(descendant))
	{
		return false;
	}

	switch (descendant)
	{
		case XmlTextSyntax { TextTokens: var tokens }:
		{
			foreach (var token in tokens)
			{
				string text = token.ValueText;
				if (!string.IsNullOrWhiteSpace(text) && text != Environment.NewLine)
				{
					sb.Append(text.TrimStart());
				}
			}

			break;
		}
		case XmlEmptyElementSyntax
		{
			Name: { LocalName: { ValueText: var markup } },
			Attributes: var attributes
		}
		when attributes[0] is { Name: { LocalName: { ValueText: var xmlPrefixName } } } firstAttribute:
		{
			string attributeValueText = (SyntaxKind)firstAttribute.RawKind switch
			{
				SyntaxKind.XmlCrefAttribute when firstAttribute is XmlCrefAttributeSyntax
				{
					Cref: var crefNode
				} => crefNode.ToString(),
				SyntaxKind.XmlNameAttribute when firstAttribute is XmlNameAttributeSyntax
				{
					Identifier: { Identifier: { ValueText: var identifier } }
				} => identifier,
				SyntaxKind.XmlTextAttribute when firstAttribute is XmlTextAttributeSyntax
				{
					Name: { LocalName: { ValueText: DocCommentAttributes.LangWord } },
					TextTokens: { Count: not 0 } tokenList
				} && tokenList[0] is { ValueText: var firstTokenText } => firstTokenText
			};

			switch (markup)
			{
				case DocCommentBlocks.See:
				{
					sb.Append($" `{attributeValueText}` ");
					break;
				}
				case DocCommentBlocks.ParamRef:
				{
					sb.Append($" `{attributeValueText}` ");
					break;
				}
				case DocCommentBlocks.TypeParamRef:
				{
					sb.Append($" `{attributeValueText}` ");
					break;
				}
			}

			break;
		}
		case XmlElementSyntax
		{
			StartTag:
			{
				Name: { LocalName: { ValueText: var markup } },
				Attributes: var attributes
			},
			Content: var content
		}
		when content.ToString() is var contentText:
		{
			switch (markup)
			{
				case DocCommentBlocks.List
				when attributes.Count != 0 && attributes[0] is XmlTextAttributeSyntax
				{
					Name: { LocalName: { ValueText: DocCommentAttributes.Type } },
					TextTokens: { Count: not 0 } listTypeNameTextTokens
				} && listTypeNameTextTokens[0] is { ValueText: var listTypeName }:
				{
					StringBuilder? listHeaderBuilder = null;
					switch (listTypeName)
					{
						case DocCommentValues.Table:
						{
							// Items:
							//   Allow <listheader> markup.
							//   Allow <item> markup.
							//   Allow nested <term> and <description> markup in the <item>.
							//   Allow <item> markup only. (But don't consider)

							// Primt title.
							sb.AppendLine("|---|---|").AppendLine("| Term | Description |");
							foreach (var node in content)
							{
								// Leading syntax nodes shouldn't match.
								switch (node)
								{
									case XmlTextSyntax: { continue; }
									case XmlElementSyntax
									{
										StartTag: { Name: { LocalName: { ValueText: var tagName } } },
										Content: var listHeaderContents
									}:
									{
										switch (tagName)
										{
											case DocCommentBlocks.ListHeader:
											{
												(listHeaderBuilder ??= new()).Append("<center>");

												foreach (var listHeaderContent in listHeaderContents)
												{
													switch (listHeaderContent)
													{
														case XmlTextSyntax:
														{
															listHeaderBuilder.Append(listHeaderContent.ToString());

															break;
														}
														case XmlEmptyElementSyntax
														{
															Name:
															{
																LocalName: { ValueText: DocCommentBlocks.See }
															},
															Attributes: { Count: 1 } langwordAttributes
														}
														when langwordAttributes[0] is XmlTextAttributeSyntax
														{
															Name:
															{
																LocalName:
																{
																	ValueText: DocCommentAttributes.LangWord
																}
															},
															TextTokens: { Count: not 0 } langAttributeTextTokens
														} && langAttributeTextTokens[0] is
														{
															ValueText: var listHeaderNestedKeywordText
														}:
														{
															sb.Append($" `{listHeaderNestedKeywordText}` ");

															break;
														}
													}
												}

												listHeaderBuilder.AppendLine("</center>").AppendLine();

												break;
											}
											case DocCommentBlocks.Item:
											{
												var itemDescendants = node.DescendantNodes();
												if (
													itemDescendants.OfType<XmlElementSyntax>().ToArray() is
													{
														Length: 2
													} termAndDescriptionPair
													&& termAndDescriptionPair[0] is XmlElementSyntax
													{
														StartTag:
														{
															Name:
															{
																LocalName: { ValueText: DocCommentBlocks.Term }
															}
														},
														Content: { Count: 1 } termContents
													}
													&& termContents[0] is XmlTextSyntax
													{
														TextTokens: var termTextTokens
													}
													&& termAndDescriptionPair[1] is XmlElementSyntax
													{
														StartTag:
														{
															Name:
															{
																LocalName:
																{
																	ValueText: DocCommentBlocks.Description
																}
															}
														},
														Content: var descriptionContents
													}
												)
												{
													// Item block contains both term and description markups.
													// Now integrate term and description part, and print them.
													var descriptionBuilder = new StringBuilder();
													foreach (var descriptionContent in descriptionContents)
													{
														switch (descriptionContent)
														{
															case XmlTextSyntax:
															{
																descriptionBuilder.Append(descriptionContent);
																break;
															}
															case XmlElementSyntax
															{
																StartTag:
																{
																	Name:
																	{
																		LocalName:
																		{
																			ValueText: var descriptionContentTypeName
																		}
																	},
																	Attributes: var descriptionContentAttributes
																},
																Content: var descriptionContentInnerContent
															}:
															{
																switch (descriptionContentTypeName)
																{
																	case DocCommentBlocks.U
																	when descriptionContentAttributes.Count == 0:
																	{
																		descriptionBuilder
																			.Append(" <u>")
																			.Append(descriptionContentInnerContent)
																			.Append("</u> ");

																		break;
																	}
																	case DocCommentBlocks.I
																	when descriptionContentAttributes.Count == 0:
																	{
																		descriptionBuilder
																			.Append(" *")
																			.Append(descriptionContentInnerContent)
																			.Append("* ");

																		break;
																	}
																	case DocCommentBlocks.B
																	when descriptionContentAttributes.Count == 0:
																	{
																		descriptionBuilder
																			.Append(" **")
																			.Append(descriptionContentInnerContent)
																			.Append("** ");

																		break;
																	}
																	case DocCommentBlocks.A
																	when descriptionContentAttributes.Count == 1
																	&& descriptionContentAttributes[0] is XmlTextAttributeSyntax
																	{
																		Name:
																		{
																			LocalName:
																			{
																				ValueText:
																					DocCommentAttributes.Href
																			}
																		},
																		TextTokens: var descriptionContentAttributesInnerTextTokens
																	}:
																	{
																		descriptionBuilder
																			.Append(" [")
																			.Append(descriptionContentInnerContent)
																			.Append("](")
																			.Append(descriptionContentAttributesInnerTextTokens.ToString())
																			.Append(") ");

																		break;
																	}
																	case DocCommentBlocks.Del
																	when descriptionContentAttributes.Count == 0:
																	{
																		descriptionBuilder
																			.Append(" ~~")
																			.Append(descriptionContentInnerContent)
																			.Append("~~ ");

																		break;
																	}
																}

																break;
															}
															case XmlEmptyElementSyntax
															{
																Name:
																{
																	LocalName:
																	{
																		ValueText: DocCommentBlocks.See
																	}
																},
																Attributes:
																{
																	Count: 1
																} langWordAttributeInDescription
															}
															when langWordAttributeInDescription[0] is XmlTextAttributeSyntax
															{
																Name:
																{
																	LocalName:
																	{
																		ValueText: DocCommentAttributes.LangWord
																	}
																},
																TextTokens: var langwordInDescriptionTextTokens
															}:
															{
																descriptionBuilder
																	.Append(" `")
																	.Append(langwordInDescriptionTextTokens)
																	.Append("` ");

																break;
															}
														}
													}

													sb
														.Append("| ")
														.Append(termTextTokens.ToString())
														.Append(" | ")
														.Append(descriptionBuilder)
														.AppendLine(" |");
												}
												//else if (
												//	itemDescendants.OfType<XmlTextSyntax>().ToArray() is
												//	{
												//		Length: 1
												//	} plainItems
												//	&& plainItems[0] is
												//	{
												//		TextTokens: var plainItemTextTokens
												//	} plainItem
												//)
												//{
												//	// Item block only contains the plain text.
												//	// This case we don't consider.
												//}

												break;
											}
										}

										break;
									}
								}
							}

							sb.AppendLine();

							if (listHeaderBuilder is not null)
							{
								sb.AppendLine(listHeaderBuilder.ToString());
							}

							sb.AppendLine();

							break;
						}
						case DocCommentValues.Bullet:
						{
							// Items:
							//   Disallow <listheader> markup.
							//   Disallow nested <term> and <description> markup in the <item>.
							//   Allow nested bullet list. (But don't consider)
							foreach (var node in content)
							{
								var bulletBuilder = new StringBuilder();

								switch (node)
								{
									case XmlElementSyntax
									{
										StartTag: { Name: { LocalName: { ValueText: var tagName } } },
										Content: var bulletContents
									}:
									{
										sb.Append("* ");

										switch (tagName)
										{
											case DocCommentBlocks.Item:
											{
												foreach (var bulletContent in bulletContents)
												{
													switch (bulletContent)
													{
														case XmlTextSyntax:
														{
															bulletBuilder.Append(bulletContent);
															break;
														}
														case XmlElementSyntax
														{
															StartTag:
															{
																Name:
																{
																	LocalName:
																	{
																		ValueText: var descriptionContentTypeName
																	}
																},
																Attributes: var descriptionContentAttributes
															},
															Content: var descriptionContentInnerContent
														}:
														{
															switch (descriptionContentTypeName)
															{
																case DocCommentBlocks.U
																when descriptionContentAttributes.Count == 0:
																{
																	bulletBuilder
																		.Append(" <u>")
																		.Append(descriptionContentInnerContent)
																		.Append("</u> ");

																	break;
																}
																case DocCommentBlocks.I
																when descriptionContentAttributes.Count == 0:
																{
																	bulletBuilder
																		.Append(" *")
																		.Append(descriptionContentInnerContent)
																		.Append("* ");

																	break;
																}
																case DocCommentBlocks.B
																when descriptionContentAttributes.Count == 0:
																{
																	bulletBuilder
																		.Append(" **")
																		.Append(descriptionContentInnerContent)
																		.Append("** ");

																	break;
																}
																case DocCommentBlocks.A
																when descriptionContentAttributes.Count == 1
																&& descriptionContentAttributes[0] is XmlTextAttributeSyntax
																{
																	Name:
																	{
																		LocalName:
																		{
																			ValueText:
																				DocCommentAttributes.Href
																		}
																	},
																	TextTokens: var descriptionContentAttributesInnerTextTokens
																}:
																{
																	bulletBuilder
																		.Append(" [")
																		.Append(descriptionContentInnerContent)
																		.Append("](")
																		.Append(descriptionContentAttributesInnerTextTokens.ToString())
																		.Append(") ");

																	break;
																}
																case DocCommentBlocks.Del
																when descriptionContentAttributes.Count == 0:
																{
																	bulletBuilder
																		.Append(" ~~")
																		.Append(descriptionContentInnerContent)
																		.Append("~~ ");

																	break;
																}
															}

															break;
														}
														case XmlEmptyElementSyntax
														{
															Name:
															{
																LocalName:
																{
																	ValueText: DocCommentBlocks.See
																}
															},
															Attributes:
															{
																Count: 1
															} langWordAttributeInDescription
														}
														when langWordAttributeInDescription[0] is XmlTextAttributeSyntax
														{
															Name:
															{
																LocalName:
																{
																	ValueText: DocCommentAttributes.LangWord
																}
															},
															TextTokens: var langwordInDescriptionTextTokens
														}:
														{
															bulletBuilder
																.Append(" `")
																.Append(langwordInDescriptionTextTokens)
																.Append("` ");

															break;
														}
													}
												}

												break;
											}
										}

										break;
									}
								}

								if (bulletBuilder.ToString() is var bulletBuilderStr and not "")
								{
									sb.AppendLine(bulletBuilderStr);
								}
							}

							sb.AppendLine().AppendLine();

							break;
						}
						case DocCommentValues.Number:
						{
							// Items:
							//   Disallow <listheader> markup.
							//   Disallow nested <term> and <description> markup in the <item>.
							//   Allow nested numbered list. (But don't consider)
							foreach (var node in content)
							{
								var numberBuilder = new StringBuilder();

								switch (node)
								{
									case XmlElementSyntax
									{
										StartTag: { Name: { LocalName: { ValueText: var tagName } } },
										Content: var bulletContents
									}:
									{
										sb.Append("1. ");

										switch (tagName)
										{
											case DocCommentBlocks.Item:
											{
												foreach (var bulletContent in bulletContents)
												{
													switch (bulletContent)
													{
														case XmlTextSyntax:
														{
															numberBuilder.Append(bulletContent);
															break;
														}
														case XmlElementSyntax
														{
															StartTag:
															{
																Name:
																{
																	LocalName:
																	{
																		ValueText: var descriptionContentTypeName
																	}
																},
																Attributes: var descriptionContentAttributes
															},
															Content: var descriptionContentInnerContent
														}:
														{
															switch (descriptionContentTypeName)
															{
																case DocCommentBlocks.U
																when descriptionContentAttributes.Count == 0:
																{
																	numberBuilder
																		.Append(" <u>")
																		.Append(descriptionContentInnerContent)
																		.Append("</u> ");

																	break;
																}
																case DocCommentBlocks.I
																when descriptionContentAttributes.Count == 0:
																{
																	numberBuilder
																		.Append(" *")
																		.Append(descriptionContentInnerContent)
																		.Append("* ");

																	break;
																}
																case DocCommentBlocks.B
																when descriptionContentAttributes.Count == 0:
																{
																	numberBuilder
																		.Append(" **")
																		.Append(descriptionContentInnerContent)
																		.Append("** ");

																	break;
																}
																case DocCommentBlocks.A
																when descriptionContentAttributes.Count == 1
																&& descriptionContentAttributes[0] is XmlTextAttributeSyntax
																{
																	Name:
																	{
																		LocalName:
																		{
																			ValueText:
																				DocCommentAttributes.Href
																		}
																	},
																	TextTokens: var descriptionContentAttributesInnerTextTokens
																}:
																{
																	numberBuilder
																		.Append(" [")
																		.Append(descriptionContentInnerContent)
																		.Append("](")
																		.Append(descriptionContentAttributesInnerTextTokens.ToString())
																		.Append(") ");

																	break;
																}
																case DocCommentBlocks.Del
																when descriptionContentAttributes.Count == 0:
																{
																	numberBuilder
																		.Append(" ~~")
																		.Append(descriptionContentInnerContent)
																		.Append("~~ ");

																	break;
																}
															}

															break;
														}
														case XmlEmptyElementSyntax
														{
															Name:
															{
																LocalName:
																{
																	ValueText: DocCommentBlocks.See
																}
															},
															Attributes:
															{
																Count: 1
															} langWordAttributeInDescription
														}
														when langWordAttributeInDescription[0] is XmlTextAttributeSyntax
														{
															Name:
															{
																LocalName:
																{
																	ValueText: DocCommentAttributes.LangWord
																}
															},
															TextTokens: var langwordInDescriptionTextTokens
														}:
														{
															numberBuilder
																.Append(" `")
																.Append(langwordInDescriptionTextTokens)
																.Append("` ");

															break;
														}
													}
												}

												break;
											}
										}

										break;
									}
								}

								if (numberBuilder.ToString() is var numberBuilderStr and not "")
								{
									sb.AppendLine(numberBuilderStr);
								}
							}

							sb.AppendLine().AppendLine();

							break;
						}
					}

					break;
				}
				case DocCommentBlocks.Para when attributes.Count == 0:
				{
					foreach (var descendantInner in content)
					{
						// Handle it recursively.
						traverse(descendantInner);
					}

					sb.AppendLine().AppendLine();

					break;
				}
				case DocCommentBlocks.C when attributes.Count == 0:
				{
					sb.Append($" `{contentText}` ");

					break;
				}
				case DocCommentBlocks.U when attributes.Count == 0:
				{
					sb.Append($" <u>{contentText}</u> ");

					break;
				}
				case DocCommentBlocks.I when attributes.Count == 0:
				{
					sb.Append($" *{contentText}* ");

					break;
				}
				case DocCommentBlocks.B when attributes.Count == 0:
				{
					sb.Append($" **{contentText}** ");

					break;
				}
				case DocCommentBlocks.Del when attributes.Count == 0:
				{
					sb.Append($" ~~{contentText}~~ ");

					break;
				}
				case DocCommentBlocks.A
				when attributes is { Count: 1 } && attributes[0] is XmlTextAttributeSyntax
				{
					Name: { LocalName: { ValueText: DocCommentAttributes.Href } },
					TextTokens: var hyperLinkTextTokens
				}:
				{
					sb.Append($" [{hyperLinkTextTokens.ToString()}]({contentText}) ");

					break;
				}
				case DocCommentBlocks.Code when attributes.Count == 0:
				{
					// Trimming. We should remove all unncessary text.
					contentText = leadingTripleSlashRegex
						.Replace(contentText, removeMatchItems)
						.Trim(newLineCharacters);

					if (sb.ToString() != string.Empty)
					{
						// If the context contains any characters, we should turn to a new line
						// to output the code block.
						sb.AppendLine().AppendLine();
					}

					sb
						.AppendLine("```csharp")    // Code block start
						.AppendLine(contentText)    // Code content
						.AppendLine("```")          // Code block end
						.AppendLine().AppendLine(); // New line

					break;

					static string removeMatchItems(Match _) => string.Empty;
				}
			}

			break;
		}
	}

#if DEBUG
	Console.ForegroundColor = ConsoleColor.Red;
	Console.Write(descendant.GetType().Name);
	Console.ResetColor();
	Console.Write(": \"");
	Console.ForegroundColor = ConsoleColor.Blue;
	Console.Write(descendant);
	Console.ResetColor();
	Console.WriteLine("\"");
	Console.WriteLine();
#endif

	return true;
}