using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sudoku.Data.Collections;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a chain node.
	/// </summary>
	public unsafe struct Node : IDisposable, IEquatable<Node>
	{
		/// <summary>
		/// Indicates the cell used. In the default case, the AIC contains only one cell and the digit (which
		/// combine to a candidate).
		/// </summary>
		internal int _cell;


		/// <summary>
		/// Initializes an instance with the specified digit, cell and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		public Node(int cell, int digit, bool isOn) : this()
		{
			(Digit, Cells, IsOn) = (digit, new GridMap { cell }, isOn);
			_cell = cell;
		}

		/// <summary>
		/// Initializes an instance with the specified digit, cells and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="cells">The cells.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		public Node(GridMap cells, int digit, bool isOn) : this()
		{
			(Digit, Cells, IsOn) = (digit, cells, isOn);
			_cell = cells.Count == 1 ? cells.SetAt(0) : -1;
		}

		/// <summary>
		/// Initializes an instance with the specified digit, the cell, a <see cref="bool"/> value
		/// and the parent node.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the specified node is on.</param>
		/// <param name="parent">The parent node.</param>
		public Node(int cell, int digit, bool isOn, Node parent) : this(cell, digit, isOn)
		{
			Alloc();
			AddParent(parent);
		}


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
				for (List<Node> todo = new List<Node> { this }, next; todo.Count != 0; todo = next)
				{
					next = new List<Node>();
					foreach (var p in todo)
					{
						if (!ancestors.Contains(p))
						{
							ancestors.Add(p);
							for (int i = 0; i < p.ParentsCount; i++)
							{
								next.Add(p.Parents[i]);
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
		/// Indicates the cells.
		/// </summary>
		public readonly GridMap Cells { get; }

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
					p = p.Parents[0];
				}

				return p;
			}
		}

		/// <summary>
		/// Gets the start node of its reverse loop.
		/// </summary>
		public Node ReverseLoopNode
		{
			get
			{
				var result = new List<Node>();
				var org = new Node?(this);
				while (org.HasValue)
				{
					var o = org.Value;
					var rev = new Node(o._cell, o.Digit, !o.IsOn);
					result.Prepend(rev);
					org = o.ParentsCount != 0 ? o[0] : (Node?)null;
				}

				Node? prev = null;
				foreach (var rev in result)
				{
					if (prev.HasValue)
					{
						prev.Value.AddParent(rev);
					}

					prev = rev;
				}

				return result[0];
			}
		}

		/// <summary>
		/// The parents.
		/// </summary>
		public Node* Parents { readonly get; private set; }

		/// <summary>
		/// Indicates the handle of the property <see cref="Parents"/>.
		/// </summary>
		/// <seealso cref="Parents"/>
		public IntPtr Handle { readonly get; private set; }

		/// <summary>
		/// The chain nodes.
		/// </summary>
		public readonly IReadOnlyList<Node> Chain
		{
			get
			{
				var result = new List<Node>();
				var done = new HashSet<Node>();
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
								next.Add(p.Parents[i]);
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
		public readonly Node this[int index] => Parents[index];


		/// <summary>
		/// Add a node into the list.
		/// </summary>
		/// <param name="node">The node.</param>
		public void AddParent(Node node)
		{
			if (Handle == IntPtr.Zero)
			{
				Alloc();
			}

			Parents[ParentsCount] = node;
			if (++ParentsCount != 1)
			{
				_cell = -1;
			}
		}

		/// <summary>
		/// Clear all parent nodes.
		/// </summary>
		public void ClearParents()
		{
			if (Handle == IntPtr.Zero)
			{
				// No parents.
				return;
			}

			ParentsCount = 0;
			//for (int i = 0; i < 6; i++)
			//{
			//	Parents[i] = default;
			//}

			_cell = -1;
		}

		/// <inheritdoc/>
		public readonly void Dispose() => Free();

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		public override readonly bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		public readonly bool Equals(Node other) => Cells == other.Cells && Digit == other.Digit && IsOn == other.IsOn;

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
				pTest = pTest.Parents[0];
				if (pTest == this)
				{
					return true;
				}
			}

			return false;
		}

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override readonly int GetHashCode() => Handle.GetHashCode();

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override readonly string ToString()
		{
			if (ParentsCount == 0)
			{
				return $"Candidates: {new CellCollection(Cells).ToString()}({Digit + 1})";
			}
			else
			{
				var nodes = new SudokuMap();
				for (int i = 0; i < ParentsCount; i++)
				{
					var node = Parents[i];
					nodes.AddRange(from cell in node.Cells select cell * 9 + node.Digit);
				}

				string cells = new CellCollection(Cells).ToString();
				string parents = new CandidateCollection(nodes).ToString();
				return $"Candidates: {cells}({Digit + 1}), Parents: {parents}";
			}
		}

		/// <summary>
		/// Allocate the memory.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Alloc() => Parents = (Node*)(Handle = Marshal.AllocHGlobal(sizeof(Node) * 6)).ToPointer();

		/// <summary>
		/// Free the memory.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly void Free() => Marshal.FreeHGlobal(Handle);


		/// <include file='../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Node left, Node right) => left.Equals(right);

		/// <include file='../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Node left, Node right) => !(left == right);
	}
}
