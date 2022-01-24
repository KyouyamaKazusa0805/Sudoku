namespace Sudoku.Data;

/// <summary>
/// Encapsulates a grid parser that can parse a string value and convert it
/// into a valid <see cref="Grid"/> instance as the result.
/// </summary>
public unsafe ref partial struct GridParser
{
	/// <summary>
	/// The list of all methods to parse.
	/// </summary>
	private static readonly delegate*<ref GridParser, Grid>[] ParseFunctions;

	/// <summary>
	/// The list of all methods to parse multiple-line grid.
	/// </summary>
	private static readonly delegate*<ref GridParser, Grid>[] MultilineParseFunctions;


	/// <summary>
	/// Initializes an instance with parsing data.
	/// </summary>
	/// <param name="parsingValue">The string to parse.</param>
	public GridParser(string parsingValue) : this(parsingValue, false, false)
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
	public GridParser(string parsingValue, bool compatibleFirst) : this(parsingValue, compatibleFirst, false)
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
	public GridParser(string parsingValue, bool compatibleFirst, bool shortenSusser)
	{
		ParsingValue = parsingValue;
		CompatibleFirst = compatibleFirst;
		ShortenSusserFormat = shortenSusser;
	}


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static GridParser()
	{
		ParseFunctions = new delegate*<ref GridParser, Grid>[]
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

		MultilineParseFunctions = ParseFunctions[1..3];


		static Grid onParsingSukaku_1(ref GridParser @this) => OnParsingSukaku(ref @this, @this.CompatibleFirst);
		static Grid onParsingSukaku_2(ref GridParser @this) => OnParsingSukaku(ref @this, !@this.CompatibleFirst);
		static Grid onParsingSusser_1(ref GridParser @this) => OnParsingSusser(ref @this, !@this.ShortenSusserFormat);
		static Grid onParsingSusser_2(ref GridParser @this) => OnParsingSusser(ref @this, @this.ShortenSusserFormat);
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
		else if (ParsingValue.Contains("-+-"))
		{
			foreach (var parseMethod in MultilineParseFunctions)
			{
				if (parseMethod(ref this) is { IsUndefined: false } grid)
				{
					return grid;
				}
			}
		}
		else if (ParsingValue.Contains('\t'))
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
	public Grid Parse(GridParsingOption gridParsingOption) =>
		gridParsingOption switch
		{
			GridParsingOption.Susser => OnParsingSusser(ref this, false),
			GridParsingOption.ShortenSusser => OnParsingSusser(ref this, true),
			GridParsingOption.Table => OnParsingSimpleMultilineGrid(ref this),
			GridParsingOption.PencilMarked => OnParsingPencilMarked(ref this),
			GridParsingOption.SimpleTable => OnParsingSimpleTable(ref this),
			GridParsingOption.Sukaku => OnParsingSukaku(ref this, compatibleFirst: false),
			GridParsingOption.SukakuSingleLine => OnParsingSukaku(ref this, compatibleFirst: true),
			GridParsingOption.Excel => OnParsingExcel(ref this),
			GridParsingOption.OpenSudoku => OnParsingOpenSudoku(ref this)
		};


	private static partial Grid OnParsingSimpleMultilineGrid(ref GridParser parser);
	private static partial Grid OnParsingExcel(ref GridParser parser);
	private static partial Grid OnParsingOpenSudoku(ref GridParser parser);
	private static partial Grid OnParsingPencilMarked(ref GridParser parser);
	private static partial Grid OnParsingSimpleTable(ref GridParser parser);
	private static partial Grid OnParsingSusser(ref GridParser parser, bool shortenSusser);
	private static partial Grid OnParsingSukaku(ref GridParser parser, bool compatibleFirst);
}
