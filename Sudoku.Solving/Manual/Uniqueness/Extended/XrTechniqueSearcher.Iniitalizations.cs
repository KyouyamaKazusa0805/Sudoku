using Sudoku.Data;
using System.Collections.Generic;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	partial class XrTechniqueSearcher
	{
		/// <summary>
		/// All combinations.
		/// </summary>
		private static readonly IReadOnlyDictionary<int, IEnumerable<short>> Combinations;


		/// <include file='../../../../GlobalDocComments.xml' path='comments/staticConstructor'/>
		static XrTechniqueSearcher()
		{
			var list = new Dictionary<int, IEnumerable<short>>();
			for (int size = 3; size <= 7; size++)
			{
				var innerList = new List<short>();
				foreach (short mask in new BitCombinationGenerator(9, size))
				{
					// Check whether all cells are in same region.
					// If so, continue the loop immediately.
					short a = (short)(mask >> 6), b = (short)(mask >> 3 & 7), c = (short)(mask & 7);
					if (size == 3 && (a == 7 || b == 7 || c == 7))
					{
						continue;
					}

					innerList.Add(mask);
				}

				list.Add(size, innerList);
			}

			Combinations = list;
		}
	}
}
