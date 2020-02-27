using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	/// <summary>
	/// Encapsulates a step finder that used in solving in
	/// <see cref="Manual.ManualSolver"/>.
	/// </summary>
	public abstract class TechniqueSearcher : IComparable<TechniqueSearcher>, IEquatable<TechniqueSearcher>
	{
		/// <summary>
		/// Indicates the priority of this technique searcher.
		/// </summary>
		public abstract int Priority { get; }


		/// <summary>
		/// Take a technique step after searched all solving steps.
		/// </summary>
		/// <param name="grid">The grid to search steps.</param>
		/// <returns>A technique information.</returns>
		public TechniqueInfo? TakeOne(IReadOnlyGrid grid)
		{
			var bag = new Bag<TechniqueInfo>();
			AccumulateAll(bag, grid);
			return bag.FirstOrDefault();
		}

		/// <summary>
		/// Accumulate all technique information instances into the specified accumulator.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">The grid.</param>
		public abstract void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid);

		/// <inheritdoc/>
		public virtual int CompareTo(TechniqueSearcher other) =>
			Priority.CompareTo(other.Priority);

		/// <inheritdoc/>
		public sealed override int GetHashCode() => Priority * 17 + 0xDEAD & 0xC0DE;

		/// <inheritdoc/>
		public virtual bool Equals(TechniqueSearcher other) => Priority == other.Priority;

		/// <inheritdoc/>
		public sealed override bool Equals(object? obj) =>
			obj is TechniqueSearcher comparer && Equals(comparer);

		/// <inheritdoc/>
		public override string ToString() => GetType().Name;


		/// <summary>
		/// Indicates whether two instances have a same value.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator ==(TechniqueSearcher left, TechniqueSearcher right) =>
			left.Equals(right);

		/// <summary>
		/// Indicates whether two instances have two different values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator !=(TechniqueSearcher left, TechniqueSearcher right) =>
			!(left == right);

		/// <summary>
		/// Indicates whether the priority value of the <paramref name="left"/>
		/// technique searcher is greater than the <paramref name="right"/> one.
		/// </summary>
		/// <param name="left">The left comparer.</param>
		/// <param name="right">The right comparer.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator >(TechniqueSearcher left, TechniqueSearcher right) =>
			left.CompareTo(right) > 0;

		/// <summary>
		/// Indicates whether the priority value of <paramref name="left"/>
		/// technique searcher is greater than or equals to the <paramref name="right"/> one.
		/// </summary>
		/// <param name="left">The left comparer.</param>
		/// <param name="right">The right comparer.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator >=(TechniqueSearcher left, TechniqueSearcher right) =>
			left.CompareTo(right) >= 0;

		/// <summary>
		/// Indicates whether the priority value of <paramref name="left"/>
		/// technique searcher is less than the <paramref name="right"/> one.
		/// </summary>
		/// <param name="left">The left comparer.</param>
		/// <param name="right">The right comparer.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator <(TechniqueSearcher left, TechniqueSearcher right) =>
			left.CompareTo(right) < 0;

		/// <summary>
		/// Indicates whether the priority value of <paramref name="left"/>
		/// technique searcher is less than or equals to the <paramref name="right"/> one.
		/// </summary>
		/// <param name="left">The left comparer.</param>
		/// <param name="right">The right comparer.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator <=(TechniqueSearcher left, TechniqueSearcher right) =>
			left.CompareTo(right) <= 0;
	}
}
