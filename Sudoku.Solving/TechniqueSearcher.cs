using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sudoku.Data;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving
{
	/// <summary>
	/// Encapsulates a step finder that used in solving in
	/// <see cref="ManualSolver"/>.
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public abstract class TechniqueSearcher : IComparable<TechniqueSearcher>, IEquatable<TechniqueSearcher>
	{
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
		/// <param name="accumulator">The accumulator to store technique information.</param>
		/// <param name="grid">The grid to search for techniques.</param>
		public abstract void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid);

		/// <inheritdoc/>
		public virtual int CompareTo(TechniqueSearcher other) =>
			GetPriority(this).CompareTo(GetPriority(other));

		/// <inheritdoc/>
		public sealed override int GetHashCode() => GetPriority(this) * 17 + 0xDEAD & 0xC0DE;

		/// <inheritdoc/>
		public virtual bool Equals(TechniqueSearcher other) => GetPriority(this) == GetPriority(other);

		/// <inheritdoc/>
		public sealed override bool Equals(object? obj) =>
			obj is TechniqueSearcher comparer && Equals(comparer);

		/// <inheritdoc/>
		public override string ToString() => GetType().Name;


		/// <summary>
		/// To get the priority of the technique searcher.
		/// </summary>
		/// <typeparam name="TTechniqueSearcher">The type of the specified technique searcher.</typeparam>
		/// <returns>The priority.</returns>
		public static int GetPriority<TTechniqueSearcher>()
			where TTechniqueSearcher : TechniqueSearcher =>
			(int)typeof(TTechniqueSearcher).GetProperty("Priority", BindingFlags.Static)!.GetValue(null)!;


		/// <summary>
		/// To get the priority of the technique searcher.
		/// </summary>
		/// <param name="instance">The technique searcher.</param>
		/// <returns>The priority.</returns>
		/// <remarks>
		/// This method uses reflection to get the specified value.
		/// </remarks>
		private static int GetPriority(TechniqueSearcher instance) =>
			(int)instance.GetType().GetProperty("Priority", BindingFlags.Static)!.GetValue(null)!;


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(TechniqueSearcher left, TechniqueSearcher right) =>
			left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
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
