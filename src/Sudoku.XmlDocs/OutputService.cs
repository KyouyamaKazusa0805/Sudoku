using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Markdown;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.Diagnostics;
using Sudoku.XmlDocs.Extensions;

namespace Sudoku.XmlDocs
{
	/// <summary>
	/// Indicates the output service.
	/// </summary>
	public sealed class OutputService
	{
		/// <summary>
		/// Indicates the default options.
		/// </summary>
		private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;


		/// <summary>
		/// Indicates the new line characters.
		/// </summary>
		private static readonly char[] NewLineCharacters = new[] { '\r', '\n', ' ' };

		/// <summary>
		/// Indicates the default time span.
		/// </summary>
		private static readonly TimeSpan TimeSpan = TimeSpan.FromSeconds(5);

		/// <summary>
		/// Indicates the empty chars regular expression instance.
		/// </summary>
		private static readonly Regex EmptyChars = new(@"\s*\r\n\s*///\s*", Options, TimeSpan);

		/// <summary>
		/// Indicates the leading triple slash characters "<c>///</c>" regular expression instance.
		/// </summary>
		private static readonly Regex LeadingTripleSlashes = new(@"(?<=\r\n)\s*(///\s+?)", Options, TimeSpan);


		/// <summary>
		/// Initializes an <see cref="OutputService"/> with the default instantiation behavior.
		/// </summary>
		public OutputService() =>
#nullable disable warnings
			RootPath = new DirectoryInfo(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
			).Parent.Parent.Parent.Parent.FullName;
#nullable restore warnings


		/// <summary>
		/// Indicates the root path that stores all projects.
		/// </summary>
		public string RootPath { get; }


		/// <summary>
		/// Execute the service, and outputs the documentation files, asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The task of the execution.</returns>
		public async Task ExecuteAsync(CancellationToken cancellationToken = default)
		{
#if CONSOLE
			Console.WriteLine("Start execution...");
#endif

			// Try to get all possible files in this whole solution.
			string[] files = (await new FileCounter(RootPath, "cs", false).CountUpAsync()).FileList.ToArray();

			// Store all possible compilations.
			var compilations = new Dictionary<string, Compilation>();
			foreach (string file in files)
			{
				string dirName = Path.GetDirectoryName(file)!;
				string projectName = dirName[dirName.LastIndexOf('\\')..];
				var compilation = CSharpCompilation.Create(projectName);

				if (!compilations.ContainsKey(projectName))
				{
					compilations.Add(projectName, compilation);
				}
			}

			// Iterate on each file via the path.
			for (int i = 0; i < files.Length; i++)
			{
				string file = files[i];

#if CONSOLE
				double progress = (i + 1) * 100F / files.Length;
				Console.WriteLine($"Progress: {progress.ToString("0.00")}%");
#endif

				StringBuilder
					typeBuilder = new(),
					fieldBuilder = new(),
					primaryConstructorBuilder = new(),
					constructorBuilder = new(),
					propertyBuilder = new(),
					indexerBuilder = new(),
					eventBuilder = new(),
					methodBuilder = new(),
					operatorBuilder = new(),
					castBuilder = new();

				// Try to get the code.
				string text = await File.ReadAllTextAsync(file, cancellationToken);

				// Try to get the syntax tree.
				var tree = CSharpSyntaxTree.ParseText(text, cancellationToken: cancellationToken);

				// Try to get the semantic model.
				string dirName = Path.GetDirectoryName(file)!;
				string projectName = dirName[dirName.LastIndexOf('\\')..];

				// Try to get the syntax node of the root.
				var node = await tree.GetRootAsync(cancellationToken);

				// Try to get the type declarations.
				var typeDeclarationSyntaxes = node.DescendantNodes().OfType<TypeDeclarationSyntax>();

				// Iterate on each syntax node of declarations.
				foreach (var typeDeclaration in typeDeclarationSyntaxes)
				{
					// Gather all member information.
					// Check whether the type is a record. New we should extract its primary constructor.
					if (typeDeclaration is RecordDeclarationSyntax { ParameterList: var paramList } recordDeclaration)
					{
						// The record doesn't contain the primary constructor.
						if (paramList is not { Parameters: { Count: not 0 } parameters })
						{
							goto IterateOnMembers;
						}

						// The record contains the primary constructor. Now store them.
						typeDeclaration.VisitDocDescendants(
							summaryNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, typeBuilder),
							remarksNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, typeBuilder),
							returnsNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, typeBuilder),
							valueNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, typeBuilder),
							exampleNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, typeBuilder),
							paramNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, typeBuilder),
							typeParamNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, typeBuilder),
							seeAlsoNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, typeBuilder),
							exceptionNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlAttributeSyntax> attributes, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, typeBuilder)
						//,inheritDocNodeVisitor: (XmlEmptyElementSyntax node, in SyntaxList<XmlAttributeSyntax> attributes) => q(node, descendants, typeBuilder)
						);
					}

				IterateOnMembers:
					// Normal type (class, struct or interface). Now we should check its members.
					foreach (var memberDeclarationSyntax in typeDeclaration.GetMembers(checkNestedTypes: true))
					{
						getBuilder(memberDeclarationSyntax).AppendMarkdownHeader(3, "Member");

						memberDeclarationSyntax.VisitDocDescendants(
							summaryNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, getBuilder(memberDeclarationSyntax)),
							remarksNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, getBuilder(memberDeclarationSyntax)),
							returnsNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, getBuilder(memberDeclarationSyntax)),
							valueNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, getBuilder(memberDeclarationSyntax)),
							exampleNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, getBuilder(memberDeclarationSyntax)),
							paramNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, getBuilder(memberDeclarationSyntax)),
							typeParamNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, getBuilder(memberDeclarationSyntax)),
							seeAlsoNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, getBuilder(memberDeclarationSyntax)),
							exceptionNodeVisitor: (XmlElementSyntax node, in SyntaxList<XmlAttributeSyntax> attributes, in SyntaxList<XmlNodeSyntax> descendants) => q(node, descendants, getBuilder(memberDeclarationSyntax))
						//,inheritDocNodeVisitor: (XmlEmptyElementSyntax node, in SyntaxList<XmlAttributeSyntax> attributes) => q(node, descendants, getBuilder(memberDeclarationSyntax))
						);

						getBuilder(memberDeclarationSyntax).AppendMarkdownNewLine();
					}

#if CONSOLE
					Console.WriteLine("Generate file...");
#endif

					try
					{
						var document = Document.Create()
							.AppendHeader(1, $"Type `{typeDeclaration.Identifier.ValueText}`")
							.AppendPlainText(typeBuilder.ToString())
							.AppendNewLine()
							.AppendHeader(2, "Fields")
							.AppendPlainText(fieldBuilder.ToString())
							.AppendNewLine()
							.AppendHeader(2, "Constructors")
							.AppendPlainText(constructorBuilder.ToString())
							.AppendNewLine()
							.AppendHeader(2, "Properties")
							.AppendPlainText(propertyBuilder.ToString())
							.AppendNewLine()
							.AppendHeader(2, "Indexers")
							.AppendPlainText(indexerBuilder.ToString())
							.AppendNewLine()
							.AppendHeader(2, "Events")
							.AppendPlainText(eventBuilder.ToString())
							.AppendNewLine()
							.AppendHeader(2, "Methods")
							.AppendPlainText(methodBuilder.ToString())
							.AppendNewLine()
							.AppendHeader(2, "Operators")
							.AppendPlainText(operatorBuilder.ToString())
							.AppendNewLine()
							.AppendHeader(2, "Casts")
							.AppendPlainText(castBuilder.ToString())
							.AppendNewLine()
							.Format();

#if CONSOLE
						Console.WriteLine("Output...");
#endif

#if DEBUG
						await document.SaveAsync(
							$@"C:\Users\Howdy\Desktop\docs\{projectName}\{typeDeclaration.Identifier.ValueText}",
							cancellationToken
						);
#else
						await document.SaveAsync(
							$@"docs\{projectName}\{typeDeclaration.Identifier.ValueText}",
							cancellationToken
						);
#endif
					}
					catch (FormatException)
					{
#if CONSOLE
						Console.Clear();
#endif
						continue;
					}

#if CONSOLE
					Console.Clear();
#endif
				}


				StringBuilder getBuilder(MemberDeclarationSyntax memberDeclaration) => memberDeclaration switch
				{
					FieldDeclarationSyntax => fieldBuilder,
					ConstructorDeclarationSyntax => constructorBuilder,
					PropertyDeclarationSyntax => propertyBuilder,
					IndexerDeclarationSyntax => indexerBuilder,
					EventDeclarationSyntax => eventBuilder,
					MethodDeclarationSyntax => methodBuilder,
					OperatorDeclarationSyntax => operatorBuilder,
					ConversionOperatorDeclarationSyntax => castBuilder,
					_ => typeBuilder
				};
			}

			void q(XmlElementSyntax node, in SyntaxList<XmlNodeSyntax> descendants, StringBuilder sb)
			{
				foreach (var descendant in descendants)
				{
					traverse(descendant, sb);
				}
			}

			bool isWhiteOrTripleSlashOnly(XmlNodeSyntax node)
			{
				string s = node.ToString();
				var match = EmptyChars.Match(s);
				return match.Success && match.Value == s;
			}

			bool traverse(XmlNodeSyntax descendant, StringBuilder sb)
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
								sb.AppendMarkdownInlineCodeBlock(attributeValueText);
								break;
							}
							case DocCommentBlocks.ParamRef:
							{
								sb.AppendMarkdownInlineCodeBlock(attributeValueText);
								break;
							}
							case DocCommentBlocks.TypeParamRef:
							{
								sb.AppendMarkdownInlineCodeBlock(attributeValueText);
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
																		sb.AppendMarkdownInlineCodeBlock(
																			listHeaderNestedKeywordText
																		);

																		break;
																	}
																}
															}

															listHeaderBuilder.Append("</center>").AppendMarkdownNewLine();

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
																					descriptionBuilder.AppendMarkdownUnderlinedBlock(
																						descriptionContentInnerContent.ToString()
																					);

																					break;
																				}
																				case DocCommentBlocks.I
																				when descriptionContentAttributes.Count == 0:
																				{
																					descriptionBuilder.AppendMarkdownItalicBlock(
																						descriptionContentInnerContent.ToString()
																					);

																					break;
																				}
																				case DocCommentBlocks.B
																				when descriptionContentAttributes.Count == 0:
																				{
																					descriptionBuilder.AppendMarkdownBoldBlock(
																						descriptionContentInnerContent.ToString()
																					);

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
																					descriptionBuilder.AppendMarkdownHyperlink(
																						descriptionContentInnerContent.ToString(),
																						descriptionContentAttributesInnerTextTokens.ToString()
																					);

																					break;
																				}
																				case DocCommentBlocks.Del
																				when descriptionContentAttributes.Count == 0:
																				{
																					descriptionBuilder.AppendMarkdownDeleteBlock(
																						descriptionContentInnerContent.ToString()
																					);

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
																			descriptionBuilder.AppendMarkdownInlineCodeBlock(
																				langwordInDescriptionTextTokens.ToString()
																			);

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
													sb.Append(MarkdownSymbols.BulletListStart);

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
																					ValueText: var bulletContentTypeName
																				}
																			},
																			Attributes: var bulletContentAttributes
																		},
																		Content: var bulletContentInnerContent
																	}:
																	{
																		switch (bulletContentTypeName)
																		{
																			case DocCommentBlocks.U
																			when bulletContentAttributes.Count == 0:
																			{
																				bulletBuilder.AppendMarkdownUnderlinedBlock(
																					bulletContentInnerContent.ToString()
																				);

																				break;
																			}
																			case DocCommentBlocks.I
																			when bulletContentAttributes.Count == 0:
																			{
																				bulletBuilder.AppendMarkdownItalicBlock(
																					bulletContentInnerContent.ToString()
																				);

																				break;
																			}
																			case DocCommentBlocks.B
																			when bulletContentAttributes.Count == 0:
																			{
																				bulletBuilder.AppendMarkdownBoldBlock(
																					bulletContentInnerContent.ToString()
																				);

																				break;
																			}
																			case DocCommentBlocks.A
																			when bulletContentAttributes.Count == 1
																			&& bulletContentAttributes[0] is XmlTextAttributeSyntax
																			{
																				Name:
																				{
																					LocalName:
																					{
																						ValueText: DocCommentAttributes.Href
																					}
																				},
																				TextTokens: var bulletContentAttributesInnerTextTokens
																			}:
																			{
																				bulletBuilder.AppendMarkdownHyperlink(
																					bulletContentInnerContent.ToString(),
																					bulletContentAttributesInnerTextTokens.ToString()
																				);

																				break;
																			}
																			case DocCommentBlocks.Del
																			when bulletContentAttributes.Count == 0:
																			{
																				bulletBuilder.AppendMarkdownDeleteBlock(
																					bulletContentInnerContent.ToString()
																				);

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
																		bulletBuilder.AppendMarkdownInlineCodeBlock(
																			langwordInDescriptionTextTokens.ToString()
																		);

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

										sb.AppendMarkdownNewLine();

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
													sb.Append(MarkdownSymbols.NumberedListStart);

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
																					ValueText: var numberContentTypeName
																				}
																			},
																			Attributes: var numberContentAttributes
																		},
																		Content: var numberContentInnerContent
																	}:
																	{
																		switch (numberContentTypeName)
																		{
																			case DocCommentBlocks.U
																			when numberContentAttributes.Count == 0:
																			{
																				numberBuilder.AppendMarkdownUnderlinedBlock(
																					numberContentInnerContent.ToString()
																				);

																				break;
																			}
																			case DocCommentBlocks.I
																			when numberContentAttributes.Count == 0:
																			{
																				numberBuilder.AppendMarkdownItalicBlock(
																					numberContentInnerContent.ToString()
																				);

																				break;
																			}
																			case DocCommentBlocks.B
																			when numberContentAttributes.Count == 0:
																			{
																				numberBuilder.AppendMarkdownBoldBlock(
																					numberContentInnerContent.ToString()
																				);

																				break;
																			}
																			case DocCommentBlocks.A
																			when numberContentAttributes.Count == 1
																			&& numberContentAttributes[0] is XmlTextAttributeSyntax
																			{
																				Name:
																				{
																					LocalName:
																					{
																						ValueText: DocCommentAttributes.Href
																					}
																				},
																				TextTokens: var numberContentAttributesInnerTextTokens
																			}:
																			{
																				numberBuilder.AppendMarkdownHyperlink(
																					numberContentInnerContent.ToString(),
																					numberContentAttributesInnerTextTokens.ToString()
																				);

																				break;
																			}
																			case DocCommentBlocks.Del
																			when numberContentAttributes.Count == 0:
																			{
																				numberBuilder.AppendMarkdownDeleteBlock(
																					numberContentInnerContent.ToString()
																				);

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
																		numberBuilder.AppendMarkdownInlineCodeBlock(
																			langwordInDescriptionTextTokens.ToString()
																		);

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

										sb.AppendMarkdownNewLine();

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
									traverse(descendantInner, sb);
								}

								sb.AppendLine().AppendLine();

								break;
							}
							case DocCommentBlocks.C when attributes.Count == 0:
							{
								sb.AppendMarkdownInlineCodeBlock(contentText);
								break;
							}
							case DocCommentBlocks.U when attributes.Count == 0:
							{
								sb.AppendMarkdownUnderlinedBlock(contentText);
								break;
							}
							case DocCommentBlocks.I when attributes.Count == 0:
							{
								sb.AppendMarkdownItalicBlock(contentText);
								break;
							}
							case DocCommentBlocks.B when attributes.Count == 0:
							{
								sb.AppendMarkdownBoldBlock(contentText);
								break;
							}
							case DocCommentBlocks.Del when attributes.Count == 0:
							{
								sb.AppendMarkdownDeleteBlock(contentText);
								break;
							}
							case DocCommentBlocks.A
							when attributes is { Count: 1 } && attributes[0] is XmlTextAttributeSyntax
							{
								Name: { LocalName: { ValueText: DocCommentAttributes.Href } },
								TextTokens: var hyperLinkTextTokens
							}:
							{
								sb.AppendMarkdownHyperlink(hyperLinkTextTokens.ToString(), contentText);

								break;
							}
							case DocCommentBlocks.Code when attributes.Count == 0:
							{
								// Trimming. We should remove all unncessary text.
								contentText = LeadingTripleSlashes
									.Replace(contentText, removeMatchItems)
									.Trim(NewLineCharacters);

								if (sb.ToString() != string.Empty)
								{
									// If the context contains any characters, we should turn to a new line
									// to output the code block.
									sb.AppendMarkdownNewLine();
								}

								sb.AppendMarkdownCodeBlock(contentText, "csharp");

								break;

								static string removeMatchItems(Match _) => string.Empty;
							}
						}

						break;
					}
				}

				return true;
			}
		}
	}
}
