#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)

using System;
using System.Collections.Generic;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Indicates a full chain.
	/// </summary>
	public sealed class FullChain : IEquatable<FullChain?>
	{
		/// <summary>
		/// Initializes an instance with the specified chain.
		/// </summary>
		/// <param name="chain">The chain.</param>
		public FullChain(ChainingTechniqueInfo chain) => Chain = chain;


		/// <summary>
		/// Inner chain.
		/// </summary>
		public ChainingTechniqueInfo Chain { get; }


		/// <inheritdoc/>
		public bool Equals(FullChain? other) => Equals(this, other);

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			var result = new HashCode();
			foreach (var target in Chain.ChainsTargets)
			{
				foreach (var p in Chain.GetChain(target))
				{
					result.Add(p);
				}
			}

			return result.ToHashCode();
		}


		/// <summary>
		/// To determine whether two chains hold the same value.
		/// </summary>
		/// <param name="this">The left node.</param>
		/// <param name="other">The right node.</param>
		/// <returns>The return value.</returns>
		private static bool Equals(FullChain? @this, FullChain? other) =>
			(@this is null, other is null) switch
			{
				(true, true) => true,
				(false, false) => InternalEquals(@this!, other!),
				_ => false
			};

		/// <summary>
		/// The internal equality determination.
		/// </summary>
		/// <param name="this">The left node.</param>
		/// <param name="other">The right node.</param>
		/// <returns>The return value.</returns>
		private static bool InternalEquals(FullChain @this, FullChain other)
		{
			// Some returned collections may not be lists and may not implement equals correctly.
			// Wrap the content in an array list.
			var thisTargets = new List<Node>(@this.Chain.ChainsTargets);
			var otherTargets = new List<Node>(other.Chain.ChainsTargets);
			if (!thisTargets.CollectionEquals(otherTargets))
			{
				return false;
			}

			var i1 = thisTargets.GetEnumerator();
			var i2 = otherTargets.GetEnumerator();
			while (i1.MoveNext() && i2.MoveNext())
			{
				Node p1 = i1.Current, p2 = i2.Current;
				if (!@this.Chain.GetChain(p1).CollectionEquals(other.Chain.GetChain(p2)))
				{
					return false;
				}
			}

			return true;
		}


		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(FullChain? left, FullChain? right) => Equals(left, right);

		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(FullChain? left, FullChain? right) => !(left == right);
	}
}
