using System;
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
		private int _cell;


		/// <summary>
		/// Initializes an instance with the specified digit, cell and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		public Node(int digit, int cell, bool isOn) : this()
		{
			(Digit, Cells, IsOn) = (digit, new GridMap { cell }, isOn);
			_cell = cell;
		}

		/// <summary>
		/// Initializes an instance with the specified digit, cells and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="cells">The cells.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
		public Node(int digit, GridMap cells, bool isOn) : this()
		{
			(Digit, Cells, IsOn) = (digit, cells, isOn);
			_cell = cells.Count == 1 ? cells.SetAt(0) : -1;
		}

		/// <summary>
		/// Initializes an instance with the specified digit, the cell, a <see cref="bool"/> value
		/// and the parent node pointer (The pointer pointing to a parent node rather than a list of parent nodes).
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the specified node is on.</param>
		/// <param name="parent">The parent node pointer.</param>
		public Node(int digit, int cell, bool isOn, Node* parent) : this(digit, cell, isOn)
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
		/// Indicates whether the specified node is on.
		/// </summary>
		public readonly bool IsOn { get; }

		/// <summary>
		/// Indicates the cells.
		/// </summary>
		public readonly GridMap Cells { get; }

		/// <summary>
		/// Indicates the handle of the property <see cref="Parents"/>.
		/// </summary>
		/// <seealso cref="Parents"/>
		public IntPtr Handle { readonly get; private set; }

		/// <summary>
		/// The parents.
		/// </summary>
		public Node** Parents { readonly get; private set; }


		/// <summary>
		/// Get the parent node with the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The pointer pointing to the node.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws when the index is out of the range.
		/// </exception>
		public readonly Node* this[int index] =>
			index < 0 || index > ParentsCount ? throw new ArgumentOutOfRangeException(nameof(index)) : Parents[index];


		/// <summary>
		/// Add a node into the list.
		/// </summary>
		/// <param name="nodePtr">The node pointer.</param>
		public void AddParent(Node* nodePtr)
		{
			if (Handle == IntPtr.Zero)
			{
				Alloc();
			}

			Parents[ParentsCount++] = nodePtr;
			if (ParentsCount != 1)
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
			for (int i = 0; i < 6; i++)
			{
				Parents[i] = null;
			}
		}

		/// <inheritdoc/>
		public readonly void Dispose() => Free();

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		public override readonly bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		public readonly bool Equals(Node other) => Handle == other.Handle;

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override readonly int GetHashCode() => Handle.GetHashCode();

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override readonly string ToString() => $"{new CellCollection(Cells).ToString()}({Digit + 1})";

		/// <summary>
		/// Allocate the memory.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Alloc() => Parents = (Node**)(Handle = Marshal.AllocHGlobal(sizeof(Node*) * 6)).ToPointer();

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
