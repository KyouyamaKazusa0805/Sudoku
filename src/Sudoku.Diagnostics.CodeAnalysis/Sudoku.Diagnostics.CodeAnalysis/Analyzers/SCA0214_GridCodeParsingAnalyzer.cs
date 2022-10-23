namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0214")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.InvocationExpression))]
public sealed partial class SCA0214_GridCodeParsingAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				Node: InvocationExpressionSyntax node,
				Compilation: var compilation,
				CancellationToken: var ct,
				SemanticModel: var semanticModel
			})
		{
			return;
		}

		if (semanticModel.GetOperation(node, ct) is not IInvocationOperation
			{
				TargetMethod:
				{
					ContainingType: var type,
					IsStatic: true
				} targetMethod,
				Arguments: [{ Value.ConstantValue: { HasValue: true, Value: string code } }, ..]
			})
		{
			return;
		}

		var location = node.GetLocation();
		if (compilation.GetTypeByMetadataName(SpecialFullTypeNames.Grid) is not { } gridType)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, location, messageArgs: new[] { SpecialFullTypeNames.Grid }));
			return;
		}

		switch (targetMethod)
		{
			case { Name: "Parse", Parameters: [{ Type.SpecialType: SpecialType.System_String }] }:
			{
				break;
			}
			case { Name: "TryParse", Parameters: [{ Type.SpecialType: SpecialType.System_String }, { Type: var tryParseType }] }
			when SymbolEqualityComparer.Default.Equals(tryParseType, gridType):
			{
				break;
			}
			default:
			{
				return;
			}
		}

		if (!SymbolEqualityComparer.Default.Equals(type, gridType))
		{
			return;
		}

		if (new SimpleGridCodeVerifier(code).Parse())
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0214, location));
	}
}

/// <summary>
/// Encapsulates a grid parser that can parse and verify a string value and convert it into a valid grid instance.
/// </summary>
file unsafe ref struct SimpleGridCodeVerifier
{
	/// <summary>
	/// The list of all methods to parse.
	/// </summary>
	private static readonly delegate*<ref SimpleGridCodeVerifier, bool>[] ParseFunctions;

	/// <summary>
	/// The list of all methods to parse multiple-line grid.
	/// </summary>
	private static readonly delegate*<ref SimpleGridCodeVerifier, bool>[] MultilineParseFunctions;


	/// <summary>
	/// Initializes an instance with parsing data.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SimpleGridCodeVerifier(string parsingValue) => ParsingValue = parsingValue;


	/// <include file='../../../global-doc-comments.xml' path='g/static-constructor' />
	static SimpleGridCodeVerifier()
	{
		ParseFunctions = new delegate*<ref SimpleGridCodeVerifier, bool>[]
		{
			&OnParsingSimpleTable,
			&OnParsingSimpleMultilineGrid,
			&OnParsingPencilMarked,
			&onParsingSusser_1,
			&onParsingSusser_2,
			&OnParsingExcel,
			&OnParsingOpenSudoku,
			&onParsingSukaku
		};

#if GITHUB_ISSUE_216
		// Cannot apply Range syntax '1..3' onto pointer-typed array.
		// Array slicing on pointer type cannot be available for AnyCPU.
		MultilineParseFunctions = ParseFunctions[1..3];
#else
		MultilineParseFunctions = new delegate*<ref SimpleGridCodeVerifier, bool>[] { &OnParsingSimpleMultilineGrid, &OnParsingPencilMarked };
#endif

		static bool onParsingSukaku(ref SimpleGridCodeVerifier @this) => OnParsingSukaku(ref @this, false);
		static bool onParsingSusser_1(ref SimpleGridCodeVerifier @this) => OnParsingSusser(ref @this, false);
		static bool onParsingSusser_2(ref SimpleGridCodeVerifier @this) => OnParsingSusser(ref @this, true);
	}


	/// <summary>
	/// The string value to parse.
	/// </summary>
	public string ParsingValue { get; private set; }

	/// <summary>
	/// Indicates whether the parser will change the execution order of PM grid.
	/// If the value is <see langword="true"/>, the parser will check compatible one
	/// first, and then check recommended parsing plan ('<c><![CDATA[<d>]]></c>' and '<c>*d*</c>').
	/// </summary>
	public bool CompatibleFirst => false;

	/// <summary>
	/// Indicates whether the parser will use shorten mode to parse a susser format grid.
	/// If the value is <see langword="true"/>, the parser will omit the continuous empty notation
	/// <c>.</c>s or <c>0</c>s to a <c>*</c>.
	/// </summary>
	public bool ShortenSusserFormat => false;


	/// <summary>
	/// To parse the value.
	/// </summary>
	/// <returns>The grid.</returns>
	public bool Parse()
	{
		if (ParsingValue.Length == 729)
		{
			if (OnParsingExcel(ref this))
			{
				return true;
			}
		}
		else if (ParsingValue.Contains("-+-"))
		{
			foreach (var parseMethod in MultilineParseFunctions)
			{
				if (parseMethod(ref this))
				{
					return true;
				}
			}
		}
		else if (ParsingValue.IndexOf('\t') != -1)
		{
			if (OnParsingExcel(ref this))
			{
				return true;
			}
		}
		else
		{
			for (int trial = 0, length = ParseFunctions.Length; trial < length; trial++)
			{
				if (ParseFunctions[trial](ref this))
				{
					return true;
				}
			}
		}

		return false;
	}


	/// <summary>
	/// Parse the value using multi-line simple grid (without any candidates).
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static bool OnParsingSimpleMultilineGrid(ref SimpleGridCodeVerifier parser)
	{
		var matches = parser.ParsingValue.MatchAll("""(\+?\d|\.)""");
		var length = matches.Length;
		if (length is not (81 or 85))
		{
			// Subtle grid outline will bring 2 '.'s on first line of the grid.
			return false;
		}

		for (var i = 0; i < 81; i++)
		{
			var currentMatch = matches[length - 81 + i];
			switch (currentMatch)
			{
				case [not ('.' or '0')]:
				{
					break;
				}
				case { Length: 1 }:
				{
					continue;
				}
				case [_, var match]:
				{
					if (match is '.' or '0')
					{
						// '+0' or '+.'? Invalid combination.
						return false;
					}

					break;
				}
				default:
				{
					// The sub-match contains more than 2 characters or empty string,
					// which is invalid.
					return false;
				}
			}
		}

		return true;
	}

	/// <summary>
	/// Parse the Excel format.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static bool OnParsingExcel(ref SimpleGridCodeVerifier parser)
	{
		var parsingValue = parser.ParsingValue;
		if (!parsingValue.Contains('\t'))
		{
			return false;
		}

		var values = parsingValue.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		if (values.Length != 9)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// Parse the open sudoku format grid.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static bool OnParsingOpenSudoku(ref SimpleGridCodeVerifier parser)
	{
		if (parser.ParsingValue.Match("""\d(\|\d){242}""") is not { } match)
		{
			return false;
		}

		for (var i = 0; i < 81; i++)
		{
			switch (match[i * 6])
			{
				case '0' when whenClause(i * 6, match, "|0|1", "|0|1|"):
				{
					continue;
				}
				case not '0' when whenClause(i * 6, match, "|0|0", "|0|0|"):
				{
					break;
				}
				default:
				{
					// Invalid string status.
					return false;
				}
			}
		}

		return true;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool whenClause(int i, string match, string pattern1, string pattern2)
			=> i == 80 * 6 ? match[(i + 1)..(i + 5)] == pattern1 : match[(i + 1)..(i + 6)] == pattern2;
	}

	/// <summary>
	/// Parse the PM grid.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The result.</returns>
	private static bool OnParsingPencilMarked(ref SimpleGridCodeVerifier parser)
	{
		// Older regular expression pattern:
		if (parser.ParsingValue.MatchAll("""(\<\d\>|\*\d\*|\d*[\+\-]?\d+)""") is not { Length: 81 } matches)
		{
			return false;
		}

		for (var cell = 0; cell < 81; cell++)
		{
			if (matches[cell] is not { Length: var length and <= 9 } s)
			{
				// More than 9 characters.
				return false;
			}

			if (s.Contains('<'))
			{
				// All values will be treated as normal characters:
				// '<digit>', '*digit*' and 'candidates'.

				// Givens.
				if (length == 3)
				{
					if (s[1] is not (>= '1' and <= '9'))
					{
						// Illegal characters found.
						return false;
					}
				}
				else
				{
					// The length is not 3.
					return false;
				}
			}
			else if (s.Contains('*'))
			{
				// Modifiables.
				if (length == 3)
				{
					if (s[1] is not (>= '1' and <= '9'))
					{
						// Illegal characters found.
						return false;
					}
				}
				else
				{
					// The length is not 3.
					return false;
				}
			}
			else if (s.SatisfyPattern("""[1-9]{1,9}"""))
			{
				// Candidates.
				// Here don't need to check the length of the string,
				// and also all characters are digit characters.
				short mask = 0;
				foreach (var c in s)
				{
					mask |= (short)(1 << c - '1');
				}

				if (mask == 0)
				{
					return false;
				}
			}
			else
			{
				// All conditions can't match.
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Parse the simple table format string (Sudoku explainer format).
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <returns>The grid.</returns>
	private static bool OnParsingSimpleTable(ref SimpleGridCodeVerifier parser)
		=> parser.ParsingValue.Match("""([\d\.\+]{9}(\r|\n|\r\n)){8}[\d\.\+]{9}""") is not null;

	/// <summary>
	/// Parse the susser format string.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <param name="shortenSusser">Indicates whether the parser will shorten the susser format.</param>
	/// <returns>The result.</returns>
	private static bool OnParsingSusser(ref SimpleGridCodeVerifier parser, bool shortenSusser)
	{
		var match = shortenSusser
			? parser.ParsingValue.Match("""[\d\.\*]{1,9}(,[\d\.\*]{1,9}){8}""")
			: parser.ParsingValue.Match("""[\d\.\+]{80,}(\:(\d{3}\s+)*\d{3})?""");

		switch (shortenSusser)
		{
			case false when match is not { Length: <= 405 }:
			case true when match is not { Length: <= 81 } || !expandCode(match, out match):
			{
				return false;
			}
		}

		int i = 0, length = match!.Length;
		for (var realPos = 0; i < length && match[i] != ':'; realPos++)
		{
			var c = match[i];
			switch (c)
			{
				case '+':
				{
					// Plus sign means the character after it is a digit,
					// which is modifiable value in the grid in its corresponding position.
					if (i < length - 1)
					{
						if (match[i + 1] is var nextChar and >= '1' and <= '9')
						{
							// Add 2 on iteration variable to skip 2 characters
							// (A plus sign '+' and a digit).
							i += 2;
						}
						else
						{
							// Why isn't the character a digit character?
							return false;
						}
					}
					else
					{
						return false;
					}

					break;
				}
				case '.':
				case '0':
				{
					// A placeholder.
					// Do nothing but only move 1 step forward.
					i++;

					break;
				}
				case >= '1' and <= '9':
				{
					// Finally moves 1 step forward.
					i++;

					break;
				}
				default:
				{
					// Other invalid characters. Throws an exception.
					//throw Throwing.ParsingError<Grid>(nameof(ParsingValue));
					return false;
				}
			}
		}

		return true;


		static bool expandCode(string? original, /*[NotNullWhen(true)]*/ out string? result)
		{
			// We must the string code holds 8 ','s and is with no ':' or '+'.
			if (original is null || original.Contains(':') || original.Contains('+') || original.CountOf(',') != 8)
			{
				result = null;
				return false;
			}

			scoped var resultSpan = (stackalloc char[81]);
			var lines = original.Split(',');
			if (lines.Length != 9)
			{
				result = null;
				return false;
			}

			// Check per line, and expand it.
			var placeholder = original.IndexOf('0') == -1 ? '.' : '0';
			for (var i = 0; i < 9; i++)
			{
				var line = lines[i];
				switch (line.CountOf('*'))
				{
					case 1 when (9 + 1 - line.Length, 0, 0) is var (empties, j, k):
					{
						foreach (var c in line)
						{
							if (c == '*')
							{
								resultSpan.Slice(i * 9 + k, empties).Fill(placeholder);

								j++;
								k += empties;
							}
							else
							{
								resultSpan[i * 9 + k] = line[j];

								j++;
								k++;
							}
						}

						break;
					}

					case var n when (9 + n - line.Length, 0, 0) is var (empties, j, k):
					{
						var emptiesPerStar = empties / n;
						foreach (var c in line)
						{
							if (c == '*')
							{
								resultSpan.Slice(i * 9 + k, emptiesPerStar).Fill(placeholder);

								j++;
								k += emptiesPerStar;
							}
							else
							{
								resultSpan[i * 9 + k] = line[j];

								j++;
								k++;
							}
						}

						break;
					}
				}
			}

			result = resultSpan.ToString();
			return true;
		}
	}

	/// <summary>
	/// Parse the sukaku format string.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the algorithm uses compatibility mode to check and parse sudoku grid.
	/// </param>
	/// <returns>The result.</returns>
	private static bool OnParsingSukaku(ref SimpleGridCodeVerifier parser, bool compatibleFirst)
	{
		const int candidatesCount = 729;
		if (compatibleFirst)
		{
			var parsingValue = parser.ParsingValue;
			if (parsingValue.Length < candidatesCount)
			{
				return false;
			}

			for (var i = 0; i < candidatesCount; i++)
			{
				var c = parsingValue[i];
				if (c is not (>= '0' and <= '9' or '.'))
				{
					return false;
				}
			}

			return true;
		}
		else
		{
			var matches = parser.ParsingValue.MatchAll("""\d*[\-\+]?\d+""");
			if (matches is { Length: not 81 })
			{
				return false;
			}

			for (var offset = 0; offset < 81; offset++)
			{
				var s = matches[offset].Reserve(@"\d");
				if (s.Length > 9)
				{
					// More than 9 characters.
					return false;
				}

				short mask = 0;
				foreach (var c in s)
				{
					mask |= (short)(1 << c - '1');
				}

				if (mask == 0)
				{
					return false;
				}
			}

			return true;
		}
	}
}
