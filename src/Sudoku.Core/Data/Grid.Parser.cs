namespace Sudoku.Data;

partial struct Grid
{
	/// <summary>
	/// Encapsulates a grid parser.
	/// </summary>
	public unsafe ref partial struct Parser
	{
		/// <summary>
		/// The list of all methods to parse.
		/// </summary>
		private static readonly delegate*<ref Parser, Grid>[] ParseFunctions;


		/// <summary>
		/// Initializes an instance with parsing data.
		/// </summary>
		/// <param name="parsingValue">The string to parse.</param>
		public Parser(string parsingValue) : this(parsingValue, false)
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
		public Parser(string parsingValue, bool compatibleFirst)
		{
			ParsingValue = parsingValue;
			CompatibleFirst = compatibleFirst;
		}


		static Parser()
		{
			ParseFunctions = new delegate*<ref Parser, Grid>[]
			{
				&OnParsingSimpleTable,
				&OnParsingSimpleMultilineGrid,
				&OnParsingPencilMarked,
				&OnParsingSusser,
				&OnParsingExcel,
				&OnParsingOpenSudoku,
				&onParsingSukaku_1,
				&onParsingSukaku_2
			};

			static Grid onParsingSukaku_1(ref Parser @this) => OnParsingSukaku(ref @this, @this.CompatibleFirst);
			static Grid onParsingSukaku_2(ref Parser @this) => OnParsingSukaku(ref @this, !@this.CompatibleFirst);
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
		/// To parse the value.
		/// </summary>
		/// <returns>The grid.</returns>
		public Grid Parse()
		{
			// Optimization: We can check some concrete type to reduce the unnecessary parsing.
			switch (ParsingValue)
			{
				case { Length: 729 } when OnParsingExcel(ref this) is { IsUndefined: false } grid:
				{
					// The sukaku should be of length 729.
					return grid;
				}
				case var _ when ParsingValue.Contains("-+-"):
				{
					// The multi-line grid should be with the mark '-' and '+'.
					for (int trial = 1; trial <= 3; trial++)
					{
						if (ParseFunctions[trial](ref this) is { IsUndefined: false } grid)
						{
							return grid;
						}
					}

					break;
				}
				case var _ when ParsingValue.Contains('\t') && OnParsingExcel(ref this) is
				{
					IsUndefined: false
				} grid:
				{
					// The excel grid should be with '\t'.
					return grid;
				}
				default:
				{
					// Other cases.
					for (int trial = 0, length = ParseFunctions.Length; trial < length; trial++)
					{
						if (ParseFunctions[trial](ref this) is { IsUndefined: false } grid)
						{
							return grid;
						}
					}

					break;
				}
			}

			return Undefined;
		}

		/// <summary>
		/// To parse the value with a specified grid parsing type.
		/// </summary>
		/// <param name="gridParsingOption">A specified parsing type.</param>
		/// <returns>The grid.</returns>
		public Grid Parse(GridParsingOption gridParsingOption) => gridParsingOption switch
		{
			GridParsingOption.Susser => OnParsingSusser(ref this),
			GridParsingOption.Table => OnParsingSimpleMultilineGrid(ref this),
			GridParsingOption.PencilMarked => OnParsingPencilMarked(ref this),
			GridParsingOption.SimpleTable => OnParsingSimpleTable(ref this),
			GridParsingOption.Sukaku => OnParsingSukaku(ref this, compatibleFirst: false),
			GridParsingOption.SukakuSingleLine => OnParsingSukaku(ref this, compatibleFirst: true),
			GridParsingOption.Excel => OnParsingExcel(ref this),
			GridParsingOption.OpenSudoku => OnParsingOpenSudoku(ref this)
		};


		private static partial Grid OnParsingSimpleMultilineGrid(ref Parser parser);
		private static partial Grid OnParsingExcel(ref Parser parser);
		private static partial Grid OnParsingOpenSudoku(ref Parser parser);
		private static partial Grid OnParsingPencilMarked(ref Parser parser);
		private static partial Grid OnParsingSimpleTable(ref Parser parser);
		private static partial Grid OnParsingSusser(ref Parser parser);
		private static partial Grid OnParsingSukaku(ref Parser parser, bool compatibleFirst);
	}
}
