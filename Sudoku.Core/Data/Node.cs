using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sudoku.Data.Collections;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a chain node.
	/// </summary>
	public unsafe struct Node : IEquatable<Node>
	{
		/// <summary>
		/// Indicates the cell used. In the default case, the AIC contains only one cell and the digit (which
		/// combine to a candidate).
		/// </summary>
		private readonly int _cell;


		public Node(int digit, int cell, bool isOn) : this()
		{
			(Digit, Cells, IsOn) = (digit, new GridMap { cell }, isOn);
			_cell = cell;
		}

		public Node(int digit, GridMap cells, bool isOn) : this()
		{
			(Digit, Cells, IsOn) = (digit, cells, isOn);
			_cell = cells.Count == 1 ? cells.SetAt(0) : -1;
		}

		public Node(int digit, int cell, bool isOn, Node* parent) : this(digit, cell, isOn)
		{
			Alloc();
			AddParent(parent);
		}


		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; }

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

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		public override bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Node other) => Handle == other.Handle;

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override int GetHashCode() => Handle.GetHashCode();

		///// <include file='../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		//public override string ToString() =>
		//	$"Candidate: {new CandidateCollection(Candidate).ToString()}, Parents pointer: {(int)Parents}";

		/// <summary>
		/// Allocate the memory.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Alloc() => Parents = (Node**)(Handle = Marshal.AllocHGlobal(sizeof(Node*) * 6)).ToPointer();

		/// <summary>
		/// Free the memory.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Free() => Marshal.FreeHGlobal(Handle);


		/// <include file='../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Node left, Node right) => left.Equals(right);

		/// <include file='../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Node left, Node right) => !(left == right);
	}
}
