using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a chain node.
	/// </summary>
	public struct Node : IValueEquatable<Node>
	{
		/// <summary>
		/// Initializes an instance with the specified digit, cell and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		public Node(int cell, int digit, bool isOn) : this()
		{
			Digit = digit;
			Cell = cell;
			IsOn = isOn;
		}

		/// <summary>
		/// Initializes an instance with the specified digit, the cell, a <see cref="bool"/> value
		/// and the parent node.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the specified node is on.</param>
		/// <param name="parent">(<see langword="in"/> parameter) The parent node.</param>
		public Node(int cell, int digit, bool isOn, in Node parent) : this(cell, digit, isOn) =>
			Parents = new List<Node> { parent };


		/// <summary>
		/// Indicates the cell used. In the default case, the AIC contains only one cell and the digit (which
		/// combine to a candidate).
		/// </summary>
		public readonly int Cell { get; }

		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public readonly int Digit { get; }

		/// <summary>
		/// Get the total number of the ancestors.
		/// </summary>
		public readonly int AncestorsCount
		{
			get
			{
				var ancestors = new List<Node>();
				for (List<Node> todo = new() { this }, next; todo.Count != 0; todo = next)
				{
					next = new();
					foreach (var p in todo)
					{
						if (!ancestors.Contains(p))
						{
							ancestors.Add(p);
							for (int i = 0, count = p.Parents?.Count ?? 0; i < count; i++)
							{
								next.Add(p.Parents![i]);
							}
						}
					}
				}

				return ancestors.Count;
			}
		}

		/// <summary>
		/// Indicates whether the specified node is on.
		/// </summary>
		public readonly bool IsOn { get; }

#if DOUBLE_LAYERED_ASSUMPTION
		/// <summary>
		/// Indicates the cause of the node.
		/// </summary>
		public readonly Cause Cause { get; init; }
#endif

		/// <summary>
		/// Indicates the root.
		/// </summary>
		/// <remarks>
		/// This property can only find the first root.
		/// </remarks>
		public readonly Node Root
		{
			get
			{
				if (Parents is null or { Count: 0 })
				{
					return this;
				}

				var p = this;
				while (p.Parents is { Count: not 0 } parents)
				{
					p = parents[0];
				}

				return p;
			}
		}

		/// <summary>
		/// Get all parents of the current node.
		/// </summary>
		public IList<Node>? Parents { readonly get; set; }

		/// <summary>
		/// The chain nodes.
		/// </summary>
		public readonly IReadOnlyList<Node> Chain
		{
			get
			{
				var result = new List<Node>();
				var done = new Set<Node>();
				var todo = new List<Node> { this };
				while (todo.Count != 0)
				{
					var next = new List<Node>();
					foreach (var p in todo)
					{
						if (!done.Contains(p))
						{
							done.Add(p);
							result.Add(p);
							for (int i = 0, count = p.Parents?.Count ?? 0; i < count; i++)
							{
								next.Add(p.Parents![i]);
							}
						}
					}

					todo = next;
				}

				return result;
			}
		}


		/// <summary>
		/// Clear all parent nodes.
		/// </summary>
		public void ClearParents() => Parents = null;

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="candidate">(<see langword="out"/> parameter) The candidate.</param>
		/// <param name="isOn">(<see langword="out"/> parameter) Indicates whether the candidate is on.</param>
		public readonly void Deconstruct(out int candidate, out bool isOn)
		{
			candidate = Cell * 9 + Digit;
			isOn = IsOn;
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="candidate">(<see langword="out"/> parameter) The candidate.</param>
		/// <param name="isOn">(<see langword="out"/> parameter) Indicates whether the candidate is on.</param>
		/// <param name="parents">(<see langword="out"/> parameter) All parents of this node.</param>
		public readonly void Deconstruct(out int candidate, out bool isOn, out IList<Node>? parents)
		{
			candidate = Cell * 9 + Digit;
			isOn = IsOn;
			parents = Parents;
		}

		/// <inheritdoc cref="object.Equals(object?)"/>
		public override readonly bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		public readonly bool Equals(in Node other) =>
			Cell == other.Cell && Digit == other.Digit && IsOn == other.IsOn;

		/// <summary>
		/// Determine whether the node is the parent of the specified node.
		/// </summary>
		/// <param name="node">(<see langword="in"/> parameter) The node.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public readonly bool IsParentOf(in Node node)
		{
			var pTest = node;
			while (pTest.Parents is { Count: not 0 } parents)
			{
				pTest = parents[0];
				if (pTest == this)
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc cref="object.GetHashCode"/>
		public override readonly int GetHashCode()
		{
			if (Parents is null)
			{
				return 0;
			}

			var hashCode = new HashCode();
			foreach (var parent in Parents)
			{
				hashCode.Add(parent);
			}

			return hashCode.ToHashCode();
		}

		/// <inheritdoc cref="object.ToString"/>
		public override readonly string ToString()
		{
			if (Parents is null or { Count: 0 })
			{
				return $"Candidate: {new GridMap { Cell }}({Digit + 1})";
			}
			else
			{
				var nodes = new SudokuMap();
				for (int i = 0; i < Parents.Count; i++)
				{
					var node = Parents[i];
					nodes.Add(node.Cell * 9 + node.Digit);
				}

				return $"Candidate: {new GridMap { Cell }}({Digit + 1}), Parent(s): {nodes}";
			}
		}


		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in Node left, in Node right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in Node left, in Node right) => !(left == right);
	}
}
