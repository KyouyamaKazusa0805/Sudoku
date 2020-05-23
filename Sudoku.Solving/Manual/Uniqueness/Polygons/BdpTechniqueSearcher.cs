using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Encapsulates a <b>Borescoper's deadly pattern</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Borescoper's Deadly Pattern")]
	public sealed partial class BdpTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// All different patterns.
		/// </summary>
		/// <remarks>
		/// All possible heptagons and octagons are in here.
		/// </remarks>
		private static readonly Pattern[] Patterns = new Pattern[14580];


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 53;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			if (EmptyMap.Count < 7)
			{
				return;
			}

			if (EmptyMap.Count == 7)
			{
				CheckFor3Digits(accumulator, grid);
			}

			CheckFor4Digits(accumulator, grid);
		}

		private void CheckFor3Digits(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int i = 0; i < 11664; i++)
			{
				var pattern = Patterns[i];
				// TODO: Check 4 types.
			}
		}

		private void CheckFor4Digits(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int i = 11664; i < 14580; i++)
			{
				var pattern = Patterns[i];
				// TODO: Check 4 types.
			}
		}
	}
}
