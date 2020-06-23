using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sudoku.Data.Collections;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Indicates the normal node in the AICs.
	/// </summary>
	public struct ChainNode : IDisposable, IEnumerable<ChainNode>, IEquatable<ChainNode>
	{
		/// <summary>
		/// Initializes an instance with the specified cell, digit and a <see cref="bool"/> value.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the specified node is on.</param>
		public unsafe ChainNode(int cell, int digit, bool isOn)
		{
			(Cell, Digit, IsOn, PredecessorsCount) = (cell, digit, isOn, 0);
			Predecessors = (ChainNode**)(PredecessorsPtr = Marshal.AllocHGlobal(sizeof(ChainNode*) * 7)).ToPointer();
		}

		/// <summary>
		/// Initializes an instance with the specified cell, digit, a <see cref="bool"/> value
		/// and a pointer.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="isOn">A <see cref="bool"/> value indicating whether the specified node is on.</param>
		/// <param name="predecessor">The predecessor pointer.</param>
		public unsafe ChainNode(int cell, int digit, bool isOn, ChainNode* predecessor) : this(cell, digit, isOn) =>
			AddPredecessor(predecessor);


		/// <summary>
		/// The digit.
		/// </summary>
		public readonly int Digit { get; }

		/// <summary>
		/// The cell.
		/// </summary>
		public readonly int Cell { get; }

		/// <summary>
		/// The number of all predecessors recorded in the node.
		/// </summary>
		public int PredecessorsCount { readonly get; private set; }

		/// <summary>
		/// Indicates whether the specified node is on.
		/// </summary>
		public readonly bool IsOn { get; }

		/// <summary>
		/// The safe <see cref="IntPtr"/> to point to the property <see cref="Predecessors"/>.
		/// </summary>
		/// <seealso cref="Predecessors"/>
		public readonly IntPtr PredecessorsPtr { get; }

		/// <summary>
		/// The raw pointer of all predecessors (An array of 7 elements at most).
		/// </summary>
		public readonly unsafe ChainNode** Predecessors { get; }


		/// <inheritdoc/>
		public readonly void Dispose()
		{
			if (PredecessorsPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(PredecessorsPtr);
			}
		}

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="object"]'/>
		public override readonly bool Equals(object? obj) => obj is ChainNode comparer && Equals(comparer);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="Equals" and @paramType="__any"]'/>
		public readonly bool Equals(ChainNode other) => (Cell, Digit, IsOn) == (other.Cell, other.Digit, other.IsOn);

		/// <summary>
		/// To determine whether the current instance is similar with another node.
		/// </summary>
		/// <param name="other">Another node.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public readonly bool Similar(ChainNode other) => (Cell, Digit) == (other.Cell, other.Digit);

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="GetHashCode"]'/>
		public override readonly int GetHashCode() => ((Cell * 9 + Digit) << 17) ^ PredecessorsPtr.GetHashCode();

		/// <include file='../../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override readonly string ToString() =>
			$"{(IsOn ? string.Empty : "!")}{new CandidateCollection(Cell * 9 + Digit).ToString()}";

		/// <summary>
		/// Add the predecessor.
		/// </summary>
		/// <param name="nodePtr">The predecessor node pointer.</param>
		public unsafe void AddPredecessor(ChainNode* nodePtr) => Predecessors[PredecessorsCount++] = nodePtr;


		/// <summary>
		/// The default constructor of this struct.
		/// </summary>
		/// <returns>The default and valid instance.</returns>
		/// <remarks>
		/// This method is provided to replace the default construcutor <see cref="ChainNode()"/>.
		/// </remarks>
		public static ChainNode CreateInstance() => new ChainNode(default, default, default);

		/// <inheritdoc/>
		public readonly unsafe IEnumerator<ChainNode> GetEnumerator()
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
		readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(ChainNode left, ChainNode right) => left.Equals(right);

		/// <include file='../../../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(ChainNode left, ChainNode right) => !(left == right);
	}
}
