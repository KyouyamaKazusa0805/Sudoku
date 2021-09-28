namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Encapsulates a source generator that generates the source code for the constants initialization
/// that is in the project <c>Sudoku.Core</c>.
/// </summary>
[Generator]
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
		if (context.IsNotInProject(ProjectNames.Core))
		{
			return;
		}

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
						{
							break;
						}
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

		#region Initialize 'RegionFirst'
		string regionFirstString = @"new[]
			{
				0, 3, 6, 27, 30, 33, 54, 57, 60,
				0, 9, 18, 27, 36, 45, 54, 63, 72,
				0, 1, 2, 3, 4, 5, 6, 7, 8
			}";
		#endregion


		context.AddSource(
			"Sudoku.Constants.Tables",
			GeneratedFileShortcuts.CoreConstants,
			$@"using System;
using System.Collections.Generic;
using Sudoku.Data;

#nullable enable

namespace Sudoku;

partial class Constants
{{
	partial class Tables
	{{
		/// <summary>
		/// Initializes all constants for this type.
		/// </summary>
		/// <remarks><i>
		/// The interactive logic is implemented by source generator, so you can't modify the inner logic.
		/// </i></remarks>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		static Tables()
		{{
			IntersectionBlockTable = {intersectionBlockSb};

			Peers = {peersSb};

			RegionCells = {regionCellsSb};

			RegionFirst = {regionFirstString};

			PeerMaps = new Cells[81];
			for (int i = 0; i < 81; i++) PeerMaps[i] = Peers[i];

			RegionMaps = new Cells[27];
			for (int i = 0; i < 27; i++) RegionMaps[i] = RegionCells[i];

			unsafe
			{{
				byte* r = stackalloc byte[] {{ 0, 1, 2, 3, 4, 5, 6, 7, 8 }};
				byte* c = stackalloc byte[] {{ 0, 3, 6, 1, 4, 7, 2, 5, 8 }};
				var dic = new Dictionary<(byte, byte), (Cells, Cells, Cells, byte[])>(new ValueTupleComparer());
				for (byte bs = 9; bs < 27; bs++)
				{{
					for (byte j = 0; j < 3; j++)
					{{
						byte cs = bs < 18 ? r[(bs - 9) / 3 * 3 + j] : c[(bs - 18) / 3 * 3 + j];
						Cells bm = RegionMaps[bs], cm = RegionMaps[cs], i = bm & cm;
						dic.Add((bs, cs), (bm - i, cm - i, i, IntersectionBlockTable[(bs - 9) * 3 + j]));
					}}
				}}

				IntersectionMaps = dic;
			}}
		}}


		/// <summary>
		/// The inner comparer of <see cref=""ValueTuple{{T1, T2}}""/> used for
		/// the field <see cref=""IntersectionMaps""/>.
		/// </summary>
		/// <seealso cref=""IntersectionMaps""/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
		private readonly struct ValueTupleComparer : IEqualityComparer<(byte Value1, byte Value2)>
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
