using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Sudoku.Data.Meta;

namespace Sudoku.Solving.Tools
{
	public static class Utility
	{
		public static CandidateField? FindCommonSubset(
			IEnumerable<CandidateField> candidateFields, int size)
		{
			var result = new CandidateField(false);
			foreach (var candidateField in candidateFields)
			{
				//result |= candidateField;
				result.BitOrWith(candidateField);
			}

			return result.Cardinality == size ? result : null;
		}

		public static IEnumerable<CellInfo> GetAllInfosWhenHavingDigit(this Grid grid, Region region, int digit) =>
			grid[region].Where(info => !info.IsValueCell && info[digit]);

		public static IEnumerable<CellInfo> SelectInfos(
			this Grid grid, Region region, CandidateField cellMask)
		{
			int i = 0;
			foreach (var info in grid[region])
			{
				if (cellMask[i] && !info.IsValueCell)
				{
					yield return info;
				}

				i++;
			}
		}

		public static IEnumerable<CandidateField> SelectCellPositions(
			this Grid grid, Region region, CandidateField digitMask)
		{
			return from i in digitMask.Trues
				   select grid.GetDigitPositionsOf(region, i);
		}

		public static IEnumerable<CandidateField> GetSubsetListOfSize(int size)
		{
			Contract.Requires(size >= 2 && size <= 4);

			return from value in new BinaryPermutation(size, 9)
				   select ToCandidateField(value);
		}

		public static IEnumerable<CandidateField> GetSubsetList()
		{
			return new List<CandidateField>(
				from permutation in new List<BinaryPermutation>
				{
					new BinaryPermutation(oneCount: 2, bitCount: 9),
					new BinaryPermutation(oneCount: 3, bitCount: 9),
					new BinaryPermutation(oneCount: 4, bitCount: 9)
				}
				from value in permutation
				select ToCandidateField(value));
		}

		private static CandidateField ToCandidateField(long value)
		{
			var result = new CandidateField();
			for (int i = 0; i < 9; i++, value >>= 1)
			{
				result[i] = (value & 1) == 1;
			}

			return result;
		}
	}
}
