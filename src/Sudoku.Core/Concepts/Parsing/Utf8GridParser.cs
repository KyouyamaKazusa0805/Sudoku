namespace Sudoku.Concepts.Parsing;

/// <summary>
/// Encapsulates a grid parser that can parse a <see cref="Utf8String"/> value and convert it
/// into a valid <see cref="Grid"/> instance as the result.
/// </summary>
public unsafe ref partial struct Utf8GridParser
{
	/// <summary>
	/// The list of all methods to parse.
	/// </summary>
	private static readonly delegate*<ref Utf8GridParser, Grid>[] ParseFunctions;

	/// <summary>
	/// The list of all methods to parse multiple-line grid.
	/// </summary>
	private static readonly delegate*<ref Utf8GridParser, Grid>[] MultilineParseFunctions;


	/// <summary>
	/// Initializes an instance with parsing data.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	public Utf8GridParser(Utf8String parsingValue) : this(parsingValue, false, false)
	{
	}

	/// <summary>
	/// Initializes an instance with parsing data and a bool value
	/// indicating whether the parsing operation should use compatible mode.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check
	/// PM grid. See <see cref="CompatibleFirst"/> to learn more.
	/// </param>
	/// <seealso cref="CompatibleFirst"/>
	public Utf8GridParser(Utf8String parsingValue, bool compatibleFirst) : this(parsingValue, compatibleFirst, false)
	{
	}

	/// <summary>
	/// Initializes an instance with parsing data and a bool value
	/// indicating whether the parsing operation should use compatible mode.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check
	/// PM grid. See <see cref="CompatibleFirst"/> to learn more.
	/// </param>
	/// <param name="shortenSusser">Indicates the parser will shorten the susser format result.</param>
	/// <seealso cref="CompatibleFirst"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8GridParser(Utf8String parsingValue, bool compatibleFirst, bool shortenSusser)
		=> (ParsingValue, CompatibleFirst, ShortenSusserFormat) = (parsingValue, compatibleFirst, shortenSusser);


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static Utf8GridParser()
	{
		ParseFunctions = new delegate*<ref Utf8GridParser, Grid>[]
		{
			&OnParsingSimpleTable,
			&OnParsingSimpleMultilineGrid,
			&OnParsingPencilMarked,
			&onParsingSusser_1,
			&onParsingSusser_2,
			&OnParsingExcel,
			&OnParsingOpenSudoku,
			&onParsingSukaku_1,
			&onParsingSukaku_2
		};

		// Bug fix for GitHub issue #216: Cannot apply Range syntax '1..3' onto pointer-typed array.
		// In other words, the following code will always cause an error on AnyCPU.
		//MultilineParseFunctions = ParseFunctions[1..3];
		MultilineParseFunctions = new delegate*<ref Utf8GridParser, Grid>[]
		{
			&OnParsingSimpleMultilineGrid,
			&OnParsingPencilMarked
		};

		static Grid onParsingSukaku_1(ref Utf8GridParser @this) => OnParsingSukaku(ref @this, @this.CompatibleFirst);
		static Grid onParsingSukaku_2(ref Utf8GridParser @this) => OnParsingSukaku(ref @this, !@this.CompatibleFirst);
		static Grid onParsingSusser_1(ref Utf8GridParser @this) => OnParsingSusser(ref @this, !@this.ShortenSusserFormat);
		static Grid onParsingSusser_2(ref Utf8GridParser @this) => OnParsingSusser(ref @this, @this.ShortenSusserFormat);
	}


	/// <summary>
	/// The string value to parse.
	/// </summary>
	public Utf8String ParsingValue { get; private set; }

	/// <summary>
	/// Indicates whether the parser will change the execution order of PM grid.
	/// If the value is <see langword="true"/>, the parser will check compatible one
	/// first, and then check recommended parsing plan ('<c><![CDATA[<d>]]></c>' and '<c>*d*</c>').
	/// </summary>
	public bool CompatibleFirst { get; }

	/// <summary>
	/// Indicates whether the parser will use shorten mode to parse a susser format grid.
	/// If the value is <see langword="true"/>, the parser will omit the continuous empty notation
	/// <c>.</c>s or <c>0</c>s to a <c>*</c>.
	/// </summary>
	public bool ShortenSusserFormat { get; private set; }


	/// <summary>
	/// To parse the value.
	/// </summary>
	/// <returns>The grid.</returns>
	public Grid Parse()
	{
		if (ParsingValue.Length == 729)
		{
			if (OnParsingExcel(ref this) is { IsUndefined: false } grid)
			{
				return grid;
			}
		}
		else if (ParsingValue.Contains("-+-"U8))
		{
			foreach (var parseMethod in MultilineParseFunctions)
			{
				if (parseMethod(ref this) is { IsUndefined: false } grid)
				{
					return grid;
				}
			}
		}
		else if (ParsingValue.Contains((Utf8Char)'\t'))
		{
			if (OnParsingExcel(ref this) is { IsUndefined: false } grid)
			{
				return grid;
			}
		}
		else
		{
			for (int trial = 0, length = ParseFunctions.Length; trial < length; trial++)
			{
				if (ParseFunctions[trial](ref this) is { IsUndefined: false } grid)
				{
					return grid;
				}
			}
		}

		return Grid.Undefined;
	}

	/// <summary>
	/// To parse the value with a specified grid parsing type.
	/// </summary>
	/// <param name="gridParsingOption">A specified parsing type.</param>
	/// <returns>The grid.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="gridParsingOption"/> is not defined.
	/// </exception>
	public Grid Parse(GridParsingOption gridParsingOption)
		=> gridParsingOption switch
		{
			GridParsingOption.Susser => OnParsingSusser(ref this, false),
			GridParsingOption.ShortenSusser => OnParsingSusser(ref this, true),
			GridParsingOption.Table => OnParsingSimpleMultilineGrid(ref this),
			GridParsingOption.PencilMarked => OnParsingPencilMarked(ref this),
			GridParsingOption.SimpleTable => OnParsingSimpleTable(ref this),
			GridParsingOption.Sukaku => OnParsingSukaku(ref this, compatibleFirst: false),
			GridParsingOption.SukakuSingleLine => OnParsingSukaku(ref this, compatibleFirst: true),
			GridParsingOption.Excel => OnParsingExcel(ref this),
			GridParsingOption.OpenSudoku => OnParsingOpenSudoku(ref this),
			_ => throw new ArgumentOutOfRangeException(nameof(gridParsingOption))
		};


	private static partial Grid OnParsingSimpleMultilineGrid(ref Utf8GridParser parser);
	private static partial Grid OnParsingExcel(ref Utf8GridParser parser);
	private static partial Grid OnParsingOpenSudoku(ref Utf8GridParser parser);
	private static partial Grid OnParsingPencilMarked(ref Utf8GridParser parser);
	private static partial Grid OnParsingSimpleTable(ref Utf8GridParser parser);
	private static partial Grid OnParsingSusser(ref Utf8GridParser parser, bool shortenSusser);
	private static partial Grid OnParsingSukaku(ref Utf8GridParser parser, bool compatibleFirst);
}
