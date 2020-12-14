using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;

namespace Sudoku.Constants
{
	partial class Processings
	{
		/// <inheritdoc cref="StaticConstructor"/>
		/// <remarks>
		/// The initialization order between static constructor and static fields
		/// may be annoying, so I use static constructor.
		/// </remarks>
		static Processings()
		{
			PeerMaps = new Cells[81];
			for (int i = 0; i < 81; i++)
			{
				PeerMaps[i] = Peers[i];
			}

			RegionMaps = new Cells[27];
			for (int i = 0; i < 27; i++)
			{
				RegionMaps[i] = RegionCells[i];
			}

			ReadOnlySpan<byte> r = (stackalloc byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
			ReadOnlySpan<byte> c = (stackalloc byte[] { 0, 3, 6, 1, 4, 7, 2, 5, 8 });
			var dic = new Dictionary<(byte, byte), (Cells, Cells, Cells)>(new ValueTupleComparer());
			for (byte bs = 9; bs < 27; bs++)
			{
				for (byte j = 0; j < 3; j++)
				{
					byte cs = bs < 18 ? r[(bs - 9) / 3 * 3 + j] : c[(bs - 18) / 3 * 3 + j];
					var bm = RegionMaps[bs];
					var cm = RegionMaps[cs];
					var i = bm & cm;
					dic.Add((bs, cs), (bm - i, cm - i, i));
				}
			}

			IntersectionMaps = dic;
		}
	}
}
