using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Indicates the normal node in the AICs.
	/// </summary>
	/// <remarks>
	/// This data structure may be like a tree, but this structre is the reversal of the trees.
	/// In other words, the property stores its predecessors (please see the property
	/// <see cref="Predecessors"/>) rather than its children.
	/// </remarks>
	/// <seealso cref="Predecessors"/>
	public unsafe struct ChainNode : IDisposable, IEnumerable<ChainNode>, IEquatable<ChainNode>
	{
		/// <summary>
		/// The safe <see cref="IntPtr"/> handle pointing to the property <see cref="Predecessors"/>.
		/// </summary>
		/// <seealso cref="Predecessors"/>
		private readonly IntPtr _handle;
		

		/// <summary>
		/// Initializes an instance with the specified cell, digit and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the specified node is on.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChainNode(byte cell, byte digit, bool isOn)
		{
			(Cell, Digit, IsOn, PredecessorsCount) = (cell, digit, isOn, 0);
			Predecessors = (ChainNode**)(_handle = Marshal.AllocHGlobal(sizeof(ChainNode*) * 7)).ToPointer();
		}

		/// <summary>
		/// Initializes an instance with the specified cell, digit, a <see cref="bool"/> value
		/// and a pointer.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the specified node is on.</param>
		/// <param name="predecessor">The predecessor pointer.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ChainNode(byte cell, byte digit, bool isOn, ChainNode* predecessor) : this(cell, digit, isOn) =>
			AddPredecessor(predecessor);


		/// <summary>
		/// The digit.
		/// </summary>
		public readonly byte Digit { get; }

		/// <summary>
		/// The cell.
		/// </summary>
		public readonly byte Cell { get; }

		/// <summary>
		/// The number of all predecessors recorded in the node.
		/// </summary>
		public int PredecessorsCount { readonly get; private set; }

		/// <summary>
		/// Indicates whether the specified node is on.
		/// </summary>
		public readonly bool IsOn { get; }

		/// <summary>
		/// The raw pointer of all predecessors (An array of 7 elements at most).
		/// </summary>
		/// <remarks>
		/// This property is an array of <see cref="ChainNode"/>*, i.e. a <see cref="ChainNode"/>*[].
		/// </remarks>
		public readonly ChainNode** Predecessors { get; }


		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Dispose()
		{
			if (_handle != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(_handle);
			}
		}

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="cell">(<see langword="out"/> parameter) The cell.</param>
		/// <param name="digit">(<see langword="out"/> parameter) The digit.</param>
		public readonly void Deconstruct(out byte cell, out byte digit) => (cell, digit) = (Cell, Digit);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="cell">(<see langword="out"/> parameter) The cell.</param>
		/// <param name="digit">(<see langword="out"/> parameter) The digit.</param>
		/// <param name="isOn">(<see langword="out"/> parameter) A <see cref="bool"/> value.</param>
		public readonly void Deconstruct(out byte cell, out byte digit, out bool isOn) =>
			(cell, digit, isOn) = (Cell, Digit, IsOn);

		/// <summary>
		/// To determine whether the node is the parent of the specified node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="ptr">(<see langword="out"/> parent) The </param>
		/// <returns></returns>
		public readonly bool IsParentOf(ChainNode node, [NotNullWhen(true)] out ChainNode* ptr)
		{
			for (var p = &node; p->PredecessorsCount != 0; p = p->Predecessors[0])
			{
				if (this == *p)
				{
					ptr = p;
					return true;
				}
			}

			ptr = null;
			return false;
		}

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object? obj) => obj is ChainNode comparer && Equals(comparer);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(ChainNode other) => (Cell, Digit, IsOn) == (other.Cell, other.Digit, other.IsOn);

		/// <summary>
		/// To determine whether the current instance is similar with another node.
		/// </summary>
		/// <param name="other">Another node.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Similar(ChainNode other) => (Cell, Digit) == (other.Cell, other.Digit);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode() => ((Cell * 9 + Digit) << 17) ^ _handle.GetHashCode();

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString() =>
			$"{(IsOn ? string.Empty : "!")}{new CandidateCollection(Cell * 9 + Digit).ToString()}";

		/// <summary>
		/// Add the predecessor.
		/// </summary>
		/// <param name="nodePtr">The predecessor node pointer.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddPredecessor(ChainNode* nodePtr) => Predecessors[PredecessorsCount++] = nodePtr;


		/// <summary>
		/// The default constructor of this struct.
		/// </summary>
		/// <returns>The default and valid instance.</returns>
		/// <remarks>
		/// This method is provided to avoid calling the default construcutor <see cref="ChainNode()"/>.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ChainNode CreateInstance() => new ChainNode(default, default, default);

		/// <inheritdoc/>
		public readonly IEnumerator<ChainNode> GetEnumerator()
		{
			var list = new List<ChainNode>(PredecessorsCount);
			int index = 0;
			for (var ptr = *Predecessors; index < PredecessorsCount; ptr++, index++)
			{
				list.Add(*ptr);
			}

			return list.GetEnumerator();
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(ChainNode left, ChainNode right) => left.Equals(right);

		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(ChainNode left, ChainNode right) => !(left == right);
	}
}
