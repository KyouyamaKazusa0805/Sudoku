namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Encapsulates a source generator that generates the source code for the constants initialization
/// that is in the project <c>Sudoku.Core</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class CoreConstantsGenerator : ISourceGenerator
{
	/// <summary>
	/// The table that used for the table <c>IntersectionBlockTable</c>.
	/// </summary>
	private static readonly (int, int, int)[] IntersectionBlockCoreValues = new[]
	{
		(0, 1, 2), (3, 4, 5), (6, 7, 8),
		(0, 3, 6), (1, 4, 7), (2, 5, 8)
	};

	/// <summary>
	/// The block table.
	/// </summary>
	private static readonly int[] BlockTable =
	{
		0, 0, 0, 1, 1, 1, 2, 2, 2,
		0, 0, 0, 1, 1, 1, 2, 2, 2,
		0, 0, 0, 1, 1, 1, 2, 2, 2,
		3, 3, 3, 4, 4, 4, 5, 5, 5,
		3, 3, 3, 4, 4, 4, 5, 5, 5,
		3, 3, 3, 4, 4, 4, 5, 5, 5,
		6, 6, 6, 7, 7, 7, 8, 8, 8,
		6, 6, 6, 7, 7, 7, 8, 8, 8,
		6, 6, 6, 7, 7, 7, 8, 8, 8
	};

	/// <summary>
	/// The row table.
	/// </summary>
	private static readonly int[] RowTable =
	{
		 9,  9,  9,  9,  9,  9,  9,  9,  9,
		10, 10, 10, 10, 10, 10, 10, 10, 10,
		11, 11, 11, 11, 11, 11, 11, 11, 11,
		12, 12, 12, 12, 12, 12, 12, 12, 12,
		13, 13, 13, 13, 13, 13, 13, 13, 13,
		14, 14, 14, 14, 14, 14, 14, 14, 14,
		15, 15, 15, 15, 15, 15, 15, 15, 15,
		16, 16, 16, 16, 16, 16, 16, 16, 16,
		17, 17, 17, 17, 17, 17, 17, 17, 17
	};

	/// <summary>
	/// The column table.
	/// </summary>
	private static readonly int[] ColumnTable =
	{
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26,
		18, 19, 20, 21, 22, 23, 24, 25, 26
	};


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		int[][] tables = new[] { BlockTable, RowTable, ColumnTable }, regionCells = new int[27][];

		#region Initialize 'IntersectionBlockTable'
		var intersectionBlockSb = new StringBuilder()
			.AppendLine("new byte[54][]")
			.AppendLine("\t\t\t{");
		foreach (var (a, b, c) in IntersectionBlockCoreValues)
		{
			for (int i = 0; i < 3; i++)
			{
				intersectionBlockSb
					.Append(new string('\t', 4))
					.Append($"new byte[] {{ {b,2}, {c,2} }}, ")
					.Append($"new byte[] {{ {a,2}, {c,2} }}, ")
					.AppendLine($"new byte[] {{ {a,2}, {b,2} }},");
			}
		}

		intersectionBlockSb.Append("\t\t\t").Append('}');
		#endregion

		#region Initialize 'Peers'
		var peersSb = new StringBuilder()
			.AppendLine("new int[81][]")
			.AppendLine("\t\t\t{");

		for (int i = 0; i < 81; i++)
		{
			int[] peers = new int[20];
			int x = 0;
			foreach (int[] table in tables)
			{
				int label = table[i];

				for (int j = 0; j < 81; j++)
				{
					if (i != j && table[j] == label && Array.IndexOf(peers, j) == -1)
					{
						peers[x++] = j;
					}
				}
			}

			peersSb.Append("\t\t\t\t").Append("new[] { ");
			for (int j = 0; j < 20; j++)
			{
				peersSb.Append($"{peers[j],2}, ");
			}

			peersSb.AppendLine("},");
		}

		peersSb.Append("\t\t\t").Append('}');
		#endregion

		#region Initialize 'RegionCells'
		var regionCellsSb = new StringBuilder()
			.AppendLine("new int[27][]")
			.AppendLine("\t\t\t{");
		for (int regionType = 0; regionType < 3; regionType++)
		{
			for (int index = 0; index < 9; index++)
			{
				int regionLabel = regionType * 9 + index;
				int[] table = tables[regionType];
				int[] cells = new int[9];
				int x = 0;
				for (int j = 0; j < 81; j++)
				{
					if (table[j] == regionLabel)
					{
						cells[x++] = j;
						if (x >= 9)
							break;
					}
				}
				regionCells[regionLabel] = cells;

				regionCellsSb.Append("\t\t\t\t").Append("new[] { ");
				for (int j = 0; j < 9; j++)
				{
					regionCellsSb.Append($"{cells[j],2}, ");
				}

				regionCellsSb.AppendLine("},");
			}
		}
		regionCellsSb.Append("\t\t\t").Append('}');
		#endregion


		context.AddSource(
			"Sudoku.Constants.Tables.g.cc.cs",
			$@"#nullable enable

namespace Sudoku;

partial class Constants
{{
	partial class Tables
	{{
		/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		static Tables()
		{{
			IntersectionBlockTable = {intersectionBlockSb};

			Peers = {peersSb};

			RegionCells = {regionCellsSb};

			Regions = new[]
			{{
				global::Sudoku.Data.Region.Block,
				global::Sudoku.Data.Region.Row,
				global::Sudoku.Data.Region.Column
			}};

			RegionFirst = new[]
			{{
				0, 3,  6, 27, 30, 33, 54, 57, 60,
				0, 9, 18, 27, 36, 45, 54, 63, 72,
				0, 1,  2,  3,  4,  5,  6,  7,  8
			}};

			Combinatorials = new[,]
			{{
				{{  1,  -1,   -1,    -1,     -1,     -1,      -1,      -1,       -1,       -1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{  2,   1,   -1,    -1,     -1,     -1,      -1,      -1,       -1,       -1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{  3,   3,    1,    -1,     -1,     -1,      -1,      -1,       -1,       -1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{  4,   6,    4,     1,     -1,     -1,      -1,      -1,       -1,       -1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{  5,  10,   10,     5,      1,     -1,      -1,      -1,       -1,       -1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{  6,  15,   20,    15,      6,      1,      -1,      -1,       -1,       -1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{  7,  21,   35,    35,     21,      7,       1,      -1,       -1,       -1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{  8,  28,   56,    70,     56,     28,       8,       1,       -1,       -1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{  9,  36,   84,   126,    126,     84,      36,       9,        1,       -1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 10,  45,  120,   210,    252,    210,     120,      45,       10,        1,       -1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 11,  55,  165,   330,    462,    462,     330,     165,       55,       11,        1,       -1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 12,  66,  220,   495,    792,    924,     792,     495,      220,       66,       12,        1,        -1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 13,  78,  286,   715,   1287,   1716,    1716,    1287,      715,      286,       78,       13,         1,        -1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 14,  91,  364,  1001,   2002,   3003,    3432,    3003,     2002,     1001,      364,       91,        14,         1,        -1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 15, 105,  455,  1365,   3003,   5005,    6435,    6435,     5005,     3003,     1365,      455,       105,        15,         1,        -1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 16, 120,  560,  1820,   4368,   8008,   11440,   12870,    11440,     8008,     4368,     1820,       560,       120,        16,         1,        -1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 17, 136,  680,  2380,   6188,  12376,   19448,   24310,    24310,    19448,    12376,     6188,      2380,       680,       136,        17,         1,       -1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 18, 153,  816,  3060,   8568,  18564,   31824,   43758,    48620,    43758,    31824,    18564,      8568,      3060,       816,       153,        18,        1,       -1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 19, 171,  969,  3876,  11628,  27132,   50388,   75582,    92378,    92378,    75582,    50388,     27132,     11628,      3876,       969,       171,       19,        1,       -1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 20, 190, 1140,  4845,  15504,  38760,   77520,  125970,   167960,   184756,   167960,   125970,     77520,     38760,     15504,      4845,      1140,      190,       20,        1,       -1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 21, 210, 1330,  5985,  20349,  54264,  116280,  203490,   293930,   352716,   352716,   293930,    203490,    116280,     54264,     20349,      5985,     1330,      210,       21,        1,      -1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 22, 231, 1540,  7315,  26334,  74613,  170544,  319770,   497420,   646646,   705432,   646646,    497420,    319770,    170544,     74613,     26334,     7315,     1540,      231,       22,       1,      -1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 23, 253, 1771,  8855,  33649, 100947,  245157,  490314,   817190,  1144066,  1352078,  1352078,   1144066,    817190,    490314,    245157,    100947,    33649,     8855,     1771,      253,      23,       1,     -1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 24, 276, 2024, 10626,  42504, 134596,  346104,  735471,  1307504,  1961256,  2496144,  2704156,   2496144,   1961256,   1307504,    735471,    346104,   134596,    42504,    10626,     2024,     276,      24,      1,     -1,    -1,   -1,  -1, -1, -1 }},
				{{ 25, 300, 2300, 12650,  53130, 177100,  480700, 1081575,  2042975,  3268760,  4457400,  5200300,   5200300,   4457400,   3268760,   2042975,   1081575,   480700,   177100,    53130,    12650,    2300,     300,     25,      1,    -1,   -1,  -1, -1, -1 }},
				{{ 26, 325, 2600, 14950,  65780, 230230,  657800, 1562275,  3124550,  5311735,  7726160,  9657700,  10400600,   9657700,   7726160,   5311735,   3124550,  1562275,   657800,   230230,    65780,   14950,    2600,    325,     26,     1,   -1,  -1, -1, -1 }},
				{{ 27, 351, 2925, 17550,  80730, 296010,  888030, 2220075,  4686825,  8436285, 13037895, 17383860,  20058300,  20058300,  17383860,  13037895,   8436285,  4686825,  2220075,   888030,   296010,   80730,   17550,   2925,    351,    27,    1,  -1, -1, -1 }},
				{{ 28, 378, 3276, 20475,  98280, 376740, 1184040, 3108105,  6906900, 13123110, 21474180, 30421755,  37442160,  40116600,  37442160,  30421755,  21474180, 13123110,  6906900,  3108105,  1184040,  376740,   98280,  20475,   3276,   378,   28,   1, -1, -1 }},
				{{ 29, 406, 3654, 23751, 118755, 475020, 1560780, 4292145, 10015005, 20030010, 34597290, 51895935,  67863915,  77558760,  77558760,  67863915,  51895935, 34597290, 20030010, 10015005,  4292145, 1560780,  475020, 118755,  23751,  3654,  406,  29,  1, -1 }},
				{{ 30, 435, 4060, 27405, 142506, 593775, 2035800, 5852925, 14307150, 30045015, 54627300, 86493225, 119759850, 145422675, 155117520, 145422675, 119759850, 86493225, 54627300, 30045015, 14307150, 5852925, 2035800, 593775, 142506, 27405, 4060, 435, 30,  1 }},
			}};

			PeerMaps = new global::Sudoku.Collections.Cells[81];
			for (int i = 0; i < 81; i++) PeerMaps[i] = Peers[i];

			RegionMaps = new global::Sudoku.Collections.Cells[27];
			for (int i = 0; i < 27; i++) RegionMaps[i] = RegionCells[i];

			unsafe
			{{
				byte* r = stackalloc byte[] {{ 0, 1, 2, 3, 4, 5, 6, 7, 8 }};
				byte* c = stackalloc byte[] {{ 0, 3, 6, 1, 4, 7, 2, 5, 8 }};
				var dic = new global::System.Collections.Generic.Dictionary<(byte, byte), (global::Sudoku.Collections.Cells, global::Sudoku.Collections.Cells, global::Sudoku.Collections.Cells, byte[])>(new ValueTupleComparer());
				for (byte bs = 9; bs < 27; bs++)
				{{
					for (byte j = 0; j < 3; j++)
					{{
						byte cs = bs < 18 ? r[(bs - 9) / 3 * 3 + j] : c[(bs - 18) / 3 * 3 + j];
						global::Sudoku.Collections.Cells bm = RegionMaps[bs], cm = RegionMaps[cs], i = bm & cm;
						dic.Add((bs, cs), (bm - i, cm - i, i, IntersectionBlockTable[(bs - 9) * 3 + j]));
					}}
				}}

				IntersectionMaps = dic;
			}}
		}}


		/// <summary>
		/// The inner comparer of <see cref=""global::System.ValueTuple{{T1, T2}}""/> used for
		/// the field <see cref=""IntersectionMaps""/>.
		/// </summary>
		/// <seealso cref=""IntersectionMaps""/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""0.7"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		[global::Sudoku.Diagnostics.CodeAnalysis.AnonymousInnerType]
		private sealed class ValueTupleComparer : IEqualityComparer<(byte Value1, byte Value2)>
		{{
			/// <inheritdoc/>
			public bool Equals((byte Value1, byte Value2) x, (byte Value1, byte Value2) y) => x == y;

			/// <inheritdoc/>
			public int GetHashCode((byte Value1, byte Value2) obj) => obj.Value1 << 5 | obj.Value2;
		}}
	}}
}}
"
		);
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
