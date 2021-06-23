using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>chain</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="XEnabled">Indicates whether the chain is enabled X strong relations.</param>
	/// <param name="YEnabled">Indicates whether the chain is enabled Y strong relations.</param>
	/// <param name="IsNishio">Indicates whether the chain is a nishio forcing chains (X-Forcing chains).</param>
	/// <param name="IsMultiple">
	/// Indicates whether the chain is a multiple forcing chains (Cell forcing chains and Region forcing chains).
	/// </param>
	/// <param name="IsDynamic">Indicates whether the chain is a dynamic forcing chains.</param>
	/// <param name="Level">The dynamic searching level.</param>
	public abstract record ChainingStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		bool XEnabled, bool YEnabled, bool IsNishio, bool IsMultiple, bool IsDynamic, int Level
	) : StepInfo(Conclusions, Views)
	{
		/// <summary>
		/// The sort key.
		/// </summary>
		public abstract ChainingTypeCode SortKey { get; }

		/// <summary>
		/// The flat complexity.
		/// </summary>
		public abstract int FlatComplexity { get; }

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <summary>
		/// The total complexity.
		/// </summary>
		public int Complexity => FlatComplexity;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override Technique TechniqueCode => this switch
		{
			{ IsNishio: true } => Technique.NishioFc,
			{ IsDynamic: true } => SortKey switch
			{
				ChainingTypeCode.DynamicRegionFc => Technique.DynamicRegionFc,
				ChainingTypeCode.DynamicCellFc => Technique.DynamicCellFc,
				ChainingTypeCode.DynamicContradictionFc => Technique.DynamicContradictionFc,
				ChainingTypeCode.DynamicDoubleFc => Technique.DynamicDoubleFc
			},
			{ IsMultiple: true } => SortKey switch
			{
				ChainingTypeCode.RegionFc => Technique.RegionFc,
				ChainingTypeCode.CellFc => Technique.CellFc,
			},
			_ => Technique.Aic
		};

		/// <summary>
		/// The base difficulty.
		/// </summary>
		protected decimal BaseDifficulty => this switch
		{
			{ IsNishio: true } => 7.5M,
			{ IsDynamic: true } => Level switch
			{
				0 => 8.5M,
				1 => 8.5M + .5M * Level,
				>= 2 => 9.5M + .5M * (Level - 2)
			},
			{ IsMultiple: true } => 8.0M
		};

		/// <summary>
		/// The length difficulty.
		/// </summary>
		protected decimal LengthDifficulty
		{
			get
			{
				decimal result = 0;
				int ceil = 4;
				int length = Complexity - 2;
				for (bool isOdd = false; length > ceil; isOdd = !isOdd)
				{
					result += .1M;
					ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
				}

				return result;
			}
		}

		/// <summary>
		/// Indicates whether the specified chain is an XY-Chain.
		/// </summary>
		protected bool IsXyChain
		{
			get
			{
				if (Views[0].Links is { } links)
				{
					for (int i = 0; i < links.Count; i += 2)
					{
						var link = links[i];
						if (link.StartCandidate / 9 != link.EndCandidate / 9)
						{
							return false;
						}
					}

					return true;
				}

				return false;
			}
		}


		/// <inheritdoc/>
		public virtual bool Equals(ChainingStepInfo? other) => (this, other) switch
		{
			(null, null) => true,
			(not null, not null) => GetHashCode() == other.GetHashCode(),
			_ => false
		};

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			// Same conclusions hold same hash code.
			int result = 0xC0DE;
			foreach (var conclusion in Conclusions)
			{
				result ^= 0xDECADE | conclusion.GetHashCode();
			}

			return result;
		}
	}
}
