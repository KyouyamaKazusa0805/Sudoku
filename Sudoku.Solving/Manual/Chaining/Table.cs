using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a table.
	/// </summary>
	public unsafe struct Table : IDisposable, IEnumerable<ChainNode>
	{
		/// <summary>
		/// The empty table.
		/// </summary>
		public static readonly Table Empty = CreateInstance();


		/// <summary>
		/// The handle pointing to the property <see cref="Nodes"/>.
		/// </summary>
		private IntPtr _handle;

		/// <summary>
		/// The number of nodes in this list.
		/// </summary>
		public int Count { readonly get; private set; }

		/// <summary>
		/// The nodes used.
		/// </summary>
		public ChainNode* Nodes { readonly get; private set; }

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Dispose()
		{
			if (_handle != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(_handle);
			}
		}

		/// <summary>
		/// Add a node into the list.
		/// </summary>
		/// <param name="node">The node.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(ChainNode node) => Nodes[Count++] = node;

		/// <summary>
		/// Removes a node from the list, and returns the pointer.
		/// </summary>
		/// <returns>The pointer.</returns>
		[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: MaybeNull]
		public ChainNode* Remove()
		{
			if (Count > 0)
			{
				return &Nodes[Count--];
			}

			return null;
		}

		/// <summary>
		/// To check whether the list contains the specified node.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="result">(<see langword="out"/> parameter) The result.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		public readonly bool Contains(int cell, int digit, [NotNullWhen(true)] out ChainNode* result)
		{
			for (int i = 0; i < Count; i++)
			{
				var node = Nodes[i];
				var (c, d) = node;
				if (c == cell && d == digit)
				{
					result = &node;
					return true;
				}
			}

			result = null;
			return false;
		}

		/// <summary>
		/// To check whether the list contains the specified node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="result">(<see langword="out"/> parameter) The result.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Contains(ChainNode node, [NotNullWhen(true)] out ChainNode* result)
		{
			var (cell, digit) = node;
			return Contains(cell, digit, out result);
		}


		/// <summary>
		/// Creates an empty and valid instance.
		/// </summary>
		/// <returns>The instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Table CreateInstance()
		{
			var result = new Table();
			result.Nodes = (ChainNode*)(result._handle = Marshal.AllocHGlobal(sizeof(ChainNode) * 1458)).ToPointer();

			return result;
		}

		/// <inheritdoc/>
		public IEnumerator<ChainNode> GetEnumerator()
		{
			var list = new List<ChainNode>(Count);
			int i = 0;
			for (var ptr = Nodes; i < Count; i++, ptr++)
			{
				list.Add(*ptr);
			}

			return list.GetEnumerator();
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
