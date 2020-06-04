using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.LastResorts;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Encapsulates a <b>guardian</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Guardian")]
	public sealed class GuardianTechniqueSearcher : SdpTechniqueSearcher
	{
		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 55;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Check POM eliminations first.
			bool[] elimKeys = { false, false, false, false, false, false, false, false, false };
			var infos = new Bag<TechniqueInfo>();
			new PomTechniqueSearcher().GetAll(infos, grid);
			foreach (PomTechniqueInfo info in infos)
			{
				elimKeys[info.Digit] = true;
			}

			for (int digit = 0; digit < 9; digit++)
			{
				var candMap = CandMaps[digit];
				if (candMap.IsEmpty || !elimKeys[digit])
				{
					continue;
				}

				foreach (int cell in candMap)
				{
				}
			}
		}
	}
}
