using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data.Collections;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a chain node.
	/// </summary>
	public struct Node : IEquatable<Node>
	{
		/// <summary>
		/// The parent nodes.
		/// </summary>
		private Node[]? _parents;


		/// <summary>
		/// Initializes an instance with the specified digit, cell and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		public Node(int cell, int digit, bool isOn) : this() => (Digit, Cell, IsOn) = (digit, cell, isOn);

		/// <summary>
		/// Initializes an instance with the specified digit, the cell, a <see cref="bool"/> value
		/// and the parent node.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the specified node is on.</param>
		/// <param name="parent">The parent node.</param>
		public Node(int cell, int digit, bool isOn, Node parent) : this(cell, digit, isOn) => AddParent(parent);


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
		/// Indicates the number of parents.
		/// </summary>
		public int ParentsCount { readonly get; private set; }

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
							for (int i = 0; i < p.ParentsCount; i++)
							{
								next.Add(p[i]);
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
				if (ParentsCount == 0)
				{
					return this;
				}

				var p = this;
				while (p.ParentsCount != 0)
				{
					p = p[0];
				}

				return p;
			}
		}

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
							for (int i = 0; i < p.ParentsCount; i++)
							{
								next.Add(p[i]);
							}
						}
					}

					todo = next;
				}

				return result;
			}
		}


		/// <summary>
		/// Get the parent node with the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The parent node.</returns>
		/// <exception cref="NullReferenceException">
		/// Throws when the <see cref="_parents"/> is currently <see langword="null"/>.
		/// </exception>
		/// <seealso cref="_parents"/>
		[IndexerName("Index")]
		public readonly Node this[int index] => _parents?[index] ?? throw new NullReferenceException();


		/// <summary>
		/// Add a node into the list.
		/// </summary>
		/// <param name="node">The node.</param>
		public void AddParent(Node node) => (_parents ??= new Node[7])[ParentsCount++] = node;

		/// <summary>
		/// Clear all parent nodes.
		/// </summary>
		public void ClearParents() => ParentsCount = 0;

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		public override readonly bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		public readonly bool Equals(Node other) => Cell == other.Cell && Digit == other.Digit && IsOn == other.IsOn;

		/// <summary>
		/// Determine whether the node is the parent of the specified node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public readonly bool IsParentOf(Node node)
		{
			var pTest = node;
			while (pTest.ParentsCount != 0)
			{
				pTest = pTest[0];
				if (pTest == this)
				{
					return true;
				}
			}

			return false;
		}

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override readonly int GetHashCode()
		{
			if (_parents is null)
			{
				return 0;
			}

			var hashCode = new HashCode();
			foreach (var parent in _parents)
			{
				hashCode.Add(parent);
			}

			return hashCode.ToHashCode();
		}

		/// <include file='..\GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override readonly string ToString()
		{
			if (ParentsCount == 0)
			{
				return $"Candidates: {new CellCollection(Cell).ToString()}({Digit + 1})";
			}
			else
			{
				var nodes = new SudokuMap();
				for (int i = 0; i < ParentsCount; i++)
				{
					var node = _parents![i];
					nodes.Add(node.Cell * 9 + node.Digit);
				}

				string cell = new CellCollection(Cell).ToString();
				string parents = new CandidateCollection(nodes).ToString();
				return $"Candidate: {cell}({Digit + 1}), Parent(s): {parents}";
			}
		}


		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Node left, Node right) => left.Equals(right);

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Node left, Node right) => !(left == right);
	}
}
