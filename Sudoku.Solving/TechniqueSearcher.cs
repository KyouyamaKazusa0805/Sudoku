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
		/// Take a step after searched all solving steps.
		/// </summary>
		/// <param name="grid">The grid to search steps.</param>
		/// <returns>A technique information.</returns>
		public TechniqueInfo? TakeOne(Grid grid) => TakeAll(grid).FirstOrDefault();

		/// <summary>
		/// Take the specified number of steps.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="count">The number of steps you want to take.</param>
		/// <returns>The specified number of technique information instances.</returns>
		public IReadOnlyList<TechniqueInfo> Take(Grid grid, int count)
		{
			// 'Take' method will never throw exceptions when
			// count is greater than the step count of the list.
			return TakeAll(grid).Take(count).ToList();
		}

		/// <summary>
		/// Take all technique steps after searched.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The list of all steps found.</returns>
		public abstract IReadOnlyList<TechniqueInfo> TakeAll(Grid grid);

		/// <inheritdoc/>
		public virtual int CompareTo(TechniqueSearcher other) =>
			Priority.CompareTo(other.Priority);

		/// <inheritdoc/>
		public sealed override int GetHashCode() => Priority * 0xDEAD + 17 & 0xC0DE;

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
