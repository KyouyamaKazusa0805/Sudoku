using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Solving.Annotations.TechniqueDisplayAttribute;

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
	/// <param name="IsDynamic">Indicates whether the chain is a dynamic forcng chains.</param>
	/// <param name="Level">The dynamic searching level.</param>
	public abstract record ChainingTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		bool XEnabled, bool YEnabled, bool IsNishio, bool IsMultiple, bool IsDynamic, int Level)
		: TechniqueInfo(Conclusions, Views)
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
		/// The base difficulty.
		/// </summary>
		public decimal BaseDifficulty =>
			(Level >= 2, Level > 0, IsNishio, IsMultiple, IsDynamic) switch
			{
				(true, _, _, _, _) => 9.5M + .5M * (Level - 2),
				(_, true, _, _, _) => 8.5M + .5M * Level,
				(_, _, true, _, _) => 7.5M,
				(_, _, _, true, _) => 8.0M,
				(_, _, _, _, true) => 8.5M,
				_ => 7.0M
			};

		/// <summary>
		/// The length difficulty.
		/// </summary>
		public decimal LengthDifficulty
		{
			get
			{
				decimal result = 0;
				int ceil = 4;
				int length = Complexity - 2;
				for (bool isOdd = false; length > ceil; isOdd.Flip())
				{
					result += .1M;
					ceil = isOdd ? ceil * 4 / 3 : ceil * 3 / 2;
				}

				return result;
			}
		}

		/// <summary>
		/// The total complexity.
		/// </summary>
		public int Complexity => FlatComplexity + NestedComplexity;

		/// <summary>
		/// The nested complexity.
		/// </summary>
		public int NestedComplexity =>
#if OBSOLETE
				int result = 0;
				var processed = new HashSet<FullChain>();
				foreach (var target in ChainsTargets)
				{
					foreach (var p in ChainingTechniqueSearcher.GetChain(target))
					{
						if (p.NestedChain is not null)
						{
							var f = new FullChain(p.NestedChain);
							if (!processed.Contains(f))
							{
								result += p.NestedChain.Complexity;
								processed.Add(f);
							}
						}
					}
				}

				return result;
#else
				0;
#endif


		/// <summary>
		/// The prefix.
		/// </summary>
		public string Prefix =>
			TechniqueCode switch
			{
				TechniqueCode.DynamicFc => "Dynamic ",
				TechniqueCode.NishioFc => "Nishio ",
				TechniqueCode.RegionFc => "Region ",
				TechniqueCode.CellFc => "Cell ",
				_ => base.Name
			};

		/// <summary>
		/// The suffix.
		/// </summary>
		public string Suffix => Level == 0 ? "Forcing Chains" : $"Forcing Chains{NestedSuffix}";

		/// <inheritdoc/>
		public override string Name => $"{Prefix}{Suffix}";

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			(Level > 0, IsNishio, IsMultiple, IsDynamic) switch
			{
				(true, _, _, _) => TechniqueCode.DynamicFc,
				(_, true, _, _) => TechniqueCode.NishioFc,
				(_, _, true, _) => SortKey == ChainingTypeCode.CellFc ? TechniqueCode.CellFc : TechniqueCode.RegionFc,
				(_, _, _, true) => TechniqueCode.DynamicFc,
				_ => TechniqueCode.Aic
			};

		/// <summary>
		/// Indicates whether the specified chain is an XY-Chain.
		/// </summary>
		protected bool IsXyChain
		{
			get
			{
				var links = Views[0].Links!;
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
		}

		/// <summary>
		/// The nested suffix.
		/// </summary>
		private string NestedSuffix => GetNestedSuffix(Level);


		/// <inheritdoc/>
		public virtual bool Equals(ChainingTechniqueInfo? other) => InternalEquals(this, other);

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

		/// <summary>
		/// Get the nested suffix with the specified level.
		/// </summary>
		/// <param name="level">The level.</param>
		/// <returns>The name suffix.</returns>
		private string GetNestedSuffix(int level) =>
			Level switch
			{
				0 => string.Empty,
				1 => " (+)",
				2 => $" (+ {GetDisplayName(TechniqueCode.Aic)})",
				3 => $" (+ Multiple Forcing Chains)",
				4 => $" (+ {GetDisplayName(TechniqueCode.DynamicFc)})",
				_ => $" (+ {GetDisplayName(TechniqueCode.DynamicFc)}{GetNestedSuffix(level - 3)})"
			};


#nullable disable warnings
		/// <summary>
		/// Determine whether two <see cref="ChainingTechniqueInfo"/> instances are same.
		/// </summary>
		/// <param name="left">The left one.</param>
		/// <param name="right">The right one.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private static bool InternalEquals(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) =>
			(left, right) switch
			{
				(null, null) => true,
				(not null, not null) => left.GetHashCode() == right.GetHashCode(),
				_ => false
			};
#nullable restore warnings
	}
}
