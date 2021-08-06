using System;
using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Resources;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>senior exocet</b> (SE) technique.
	/// </summary>
	/// <param name="Views">All views.</param>
	/// <param name="Exocet">The exocet.</param>
	/// <param name="Digits">All digits.</param>
	/// <param name="EndoTargetCell">The endo target cell.</param>
	/// <param name="ExtraRegionsMask">The extra regions mask.</param>
	/// <param name="Eliminations">All eliminations.</param>
	public sealed record SeStepInfo(
		IReadOnlyList<View> Views, in Pattern Exocet, IEnumerable<int> Digits,
		int EndoTargetCell, int[]? ExtraRegionsMask, IReadOnlyList<Elimination> Eliminations
	) : ExocetStepInfo(Views, Exocet, Digits, null, null, Eliminations)
	{
		/// <summary>
		/// Indicates whether the specified instance contains any extra regions.
		/// </summary>
		public bool ContainsExtraRegions =>
			ExtraRegionsMask is not null && Array.Exists(ExtraRegionsMask, static m => m != 0);

		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M + (ContainsExtraRegions ? 0 : .2M);

		/// <inheritdoc/>
		public override string? Acronym => "SE";

		/// <inheritdoc/>
		public override Technique TechniqueCode => ContainsExtraRegions ? Technique.ComplexSe : Technique.Se;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.Exocet;

		/// <inheritdoc/>
		[FormatItem]
		protected override string? AdditionalFormat
		{
			get
			{
				const string separator = ", ";
				string endoTargetSnippet = TextResources.Current.EndoTargetSnippet;
				string endoTargetStr = $"{endoTargetSnippet}{EndoTargetCellStr}";
				if (ExtraRegionsMask is not null)
				{
					var sb = new ValueStringBuilder(stackalloc char[100]);
					int count = 0;
					for (int digit = 0; digit < 9; digit++)
					{
						if (ExtraRegionsMask[digit] is not (var mask and not 0))
						{
							continue;
						}

						sb.Append(digit + 1);
						sb.Append(new RegionCollection(mask.GetAllSets()).ToString());
						sb.Append(separator);

						count++;
					}

					if (count != 0)
					{
						sb.RemoveFromEnd(separator.Length);

						string extraRegionsIncluded = TextResources.Current.IncludedExtraRegionSnippet;
						return $"{endoTargetStr}{extraRegionsIncluded}{sb.ToString()}";
					}
				}

				return endoTargetStr;
			}
		}

		[FormatItem]
		private string EndoTargetCellStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells { EndoTargetCell }.ToString();
		}


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string? GetAdditional()
		{
			const string separator = ", ";
			string endoTargetStr = $"endo-target: {new Cells { EndoTargetCell }.ToString()}";
			if (ExtraRegionsMask is not null)
			{
				var sb = new ValueStringBuilder(stackalloc char[100]);
				int count = 0;
				for (int digit = 0; digit < 9; digit++)
				{
					if (ExtraRegionsMask[digit] is not (var mask and not 0))
					{
						continue;
					}

					sb.Append(digit + 1);
					sb.Append(new RegionCollection(mask.GetAllSets()).ToString());
					sb.Append(separator);

					count++;
				}

				if (count != 0)
				{
					sb.RemoveFromEnd(separator.Length);

					return $"{endoTargetStr}. Extra regions will be included: {sb.ToString()}";
				}
			}

			return endoTargetStr;
		}
	}
}
