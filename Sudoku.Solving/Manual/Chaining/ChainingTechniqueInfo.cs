using System;
using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Solving.Annotations.TechniqueDisplayAttribute;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of <b>chain</b> technique.
	/// </summary>
	public abstract class ChainingTechniqueInfo : TechniqueInfo, IEquatable<ChainingTechniqueInfo?>
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="xEnabled">Indicates whether the chain is enabled X strong relations.</param>
		/// <param name="yEnabled">Indicates whether the chain is enabled Y strong relations.</param>
		/// <param name="isNishio">Indicates whether the chain is a nishio forcing chains (X-Forcing chains).</param>
		/// <param name="isMultiple">
		/// Indicates whether the chain is a multiple forcing chains (Cell forcing chains and Region forcing chains).
		/// </param>
		/// <param name="isDynamic">Indicates whether the chain is a dynamic forcng chains.</param>
		/// <param name="level">The dynamic searching level.</param>
		protected ChainingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, bool xEnabled, bool yEnabled,
			bool isNishio, bool isMultiple, bool isDynamic, int level) : base(conclusions, views) =>
			(XEnabled, YEnabled, IsNishio, IsMultiple, IsDynamic, Level) = (
				xEnabled, yEnabled, isNishio, isMultiple, isDynamic, level);


		/// <summary>
		/// Indicates whether the chain is enabled X strong relations.
		/// </summary>
		public bool XEnabled { get; }

		/// <summary>
		/// Indicates whether the chain is enabled Y strong relations.
		/// </summary>
		public bool YEnabled { get; }

		/// <summary>
		/// Indicates whether the chain is a nishio forcing chains (X-Forcing chains).
		/// </summary>
		public bool IsNishio { get; }

		/// <summary>
		/// Indicates whether the chain is a multiple forcing chains (Cell forcing chains and Region forcing chains).
		/// </summary>
		public bool IsMultiple { get; }

		/// <summary>
		/// Indicates whether the chain is a dynamic forcng chains.
		/// </summary>
		public bool IsDynamic { get; }

		/// <summary>
		/// Indicates the dynamic searching level.
		/// </summary>
		public int Level { get; }

		/// <summary>
		/// The sort key.
		/// </summary>
		public abstract int SortKey { get; }

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
				(_, _, _, true, _) => 8M,
				(_, _, _, _, true) => 8.5M,
				_ => throw Throwings.ImpossibleCase
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
		public int NestedComplexity
		{
			get
			{
				return 0;
				//int result = 0;
				//var processed = new HashSet<FullChain>();
				//foreach (var target in ChainsTargets)
				//{
				//	foreach (var p in ChainingTechniqueSearcher.GetChain(target))
				//	{
				//		if (!(p.NestedChain is null))
				//		{
				//			var f = new FullChain(p.NestedChain);
				//			if (!processed.Contains(f))
				//			{
				//				result += p.NestedChain.Complexity;
				//				processed.Add(f);
				//			}
				//		}
				//	}
				//}
				//
				//return result;
			}
		}

		/// <summary>
		/// The prefix.
		/// </summary>
		public string Prefix =>
			TechniqueCode switch
			{
				TechniqueCode.DynamicFc => "Dynamic ",
				TechniqueCode.NishioFc => "Nishio, ",
				TechniqueCode.MultipleFc => "Multiple ",
				_ => base.Name
			};

		/// <summary>
		/// The suffix.
		/// </summary>
		public string Suffix => Level == 0 ? " Chains" : $" Chains{NestedSuffix}";

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
				(_, _, true, _) => TechniqueCode.MultipleFc,
				(_, _, _, true) => TechniqueCode.DynamicFc,
				_ => TechniqueCode.Aic
			};

		/// <summary>
		/// The nested suffix.
		/// </summary>
		private string NestedSuffix => GetNestedSuffix(Level);


		/// <inheritdoc/>
		public override bool Equals(object? obj) => Equals(obj as ChainingTechniqueInfo);

		/// <inheritdoc/>
		public override bool Equals(TechniqueInfo? other) => Equals(other as ChainingTechniqueInfo);

		/// <inheritdoc/>
		public bool Equals(ChainingTechniqueInfo? other) => InternalEquals(this, other);

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			// Same conclusions hold same hash code.
			var result = new HashCode();
			foreach (var conclusion in Conclusions)
			{
				result.Add(conclusion);
			}

			return result.ToHashCode();
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
				3 => $" (+ {GetDisplayName(TechniqueCode.MultipleFc)})",
				4 => $" (+ {GetDisplayName(TechniqueCode.DynamicFc)})",
				_ => $" (+ {GetDisplayName(TechniqueCode.DynamicFc)}{GetNestedSuffix(level - 3)})"
			};


		/// <summary>
		/// Determine whether two <see cref="ChainingTechniqueInfo"/> instances are same.
		/// </summary>
		/// <param name="left">The left one.</param>
		/// <param name="right">The right one.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		private static bool InternalEquals(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) =>
			(left is null, right is null) switch
			{
				(true, true) => true,
				(false, false) => left!.GetHashCode() == right!.GetHashCode(),
				_ => false
			};


		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) =>
			InternalEquals(left, right);

		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(ChainingTechniqueInfo? left, ChainingTechniqueInfo? right) => !(left == right);
	}
}
