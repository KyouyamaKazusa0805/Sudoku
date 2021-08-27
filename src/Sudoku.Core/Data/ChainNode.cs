namespace Sudoku.Data;

/// <summary>
/// Defines a chain node, with basic information about the node. At the same time you can get the root node
/// of the chain, using the current node as the tail node.
/// </summary>
[AutoEquality(nameof(Cell), nameof(Digit), nameof(IsOn))]
[AutoDeconstruct(nameof(Cell), nameof(Digit), nameof(IsOn))]
public unsafe partial struct ChainNode : IValueEquatable<ChainNode>, IDisposable
{
	/// <summary>
	/// Indicates the number of parent nodes recorded in this collection.
	/// </summary>
	private int _currentParentIndex = 0;

	/// <summary>
	/// <para>Indicates all parents of the current node.</para>
	/// <para>
	/// The value is represented by the type <see cref="ChainNode"/>** (i.e. a double pointer),
	/// the first pointer mark stands for a basic reference to a real <see cref="ChainNode"/>-typed instance
	/// (i.e. a <see cref="ChainNode"/>*), and the second pointer mark stands for the property
	/// is an array of that type (i.e. a * to <see cref="ChainNode"/>*).
	/// </para>
	/// <para>
	/// In addition, the value stores the address value that points to the unmanaged memory, If you use out
	/// of this instance, you <b>must</b> calls the method <see cref="Dispose"/> to release the memory.
	/// </para>
	/// </summary>
	/// <seealso cref="Dispose"/>
	private ChainNode** _parents = null;


	/// <summary>
	/// Initializes a <see cref="ChainNode"/> instance using the specified cell, digit and a <see cref="bool"/>
	/// value indicating whether the node is on.
	/// </summary>
	/// <param name="cell">The cell used.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainNode(int cell, int digit, bool isOn)
	{
		Cell = cell;
		Digit = digit;
		IsOn = isOn;
	}

	/// <summary>
	/// Initializes a <see cref="ChainNode"/> instance using the specified cell, digit, a <see cref="bool"/>
	/// value indicating whether the node is on, and a single node that represents the parent of the current node.
	/// </summary>
	/// <param name="cell">The cell used.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="isOn">A <see cref="bool"/> value indicating whether the node is on.</param>
	/// <param name="parent">Indicates the parent of the current node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainNode(int cell, int digit, bool isOn, [DisallowNull] ChainNode* parent) : this(cell, digit, isOn)
	{
		CreateParentsBlock();
		_parents[_currentParentIndex++] = parent;
	}


	/// <summary>
	/// A <see cref="bool"/> value indicating whether the node is on.
	/// </summary>
	public bool IsOn { get; }

	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public int Cell { get; }

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit { get; }

	/// <summary>
	/// Get the total number of the ancestors.
	/// </summary>
	public readonly int AncestorsCount
	{
		get
		{
			var ancestors = new List<ChainNode>();
			for (List<ChainNode> todo = new() { this }, next; todo.Count != 0; todo = next)
			{
				next = new();
				foreach (var p in todo)
				{
					if (!ancestors.Contains(p))
					{
						ancestors.Add(p);
						for (int i = 0, count = p._currentParentIndex; i < count; i++)
						{
							next.Add(*p._parents[i]);
						}
					}
				}
			}

			return ancestors.Count;
		}
	}

	/// <summary>
	/// <para>Indicates the root of the current chain.</para>
	/// <para>
	/// This property will regard the current instance as a pointer that points to a normal chain.
	/// If the property <see cref="_parents"/> isn't <see langword="null"/>, then the property will search
	/// for the parent node and check its parent node (i.e. the grandparents). When the <see cref="_parents"/>
	/// is <see langword="null"/>, the node should be the root node, and then the method will return
	/// the address that points to it.
	/// </para>
	/// </summary>
	/// <seealso cref="_parents"/>
	[NotNull]
	public readonly ChainNode* Root
	{
		get
		{
			fixed (ChainNode* pThis = &this)
			{
				if (_currentParentIndex == 0)
				{
					return pThis;
				}

				ChainNode* p;
				for (
					p = pThis;
					p->_parents is var parents and not null && parents[0]->_currentParentIndex != 0;
					p = p->_parents[0]
				) ;

				return p;
			}
		}
	}

	/// <summary>
	/// Gets all parent nodes.
	/// </summary>
	public readonly ChainNode*[]? Parents
	{
		get
		{
			if (_parents == null)
			{
				return null;
			}

			var result = new ChainNode*[_currentParentIndex];
			fixed (ChainNode** pResult = result)
			{
				Unsafe.CopyBlock(pResult, _parents, (uint)(sizeof(ChainNode*) * _currentParentIndex));
			}

			return result;
		}
	}

	/// <summary>
	/// Indicates the nodes that the current node lies in.
	/// </summary>
	/// <remarks>
	/// For the performance optimization, the return type is a list of <see cref="IntPtr"/>s indicates
	/// the pointer that points to each elements. If you want to get the real value, please cast the type
	/// to <see cref="ChainNode"/>*:
	/// <code>
	/// var list = Chain; // Get the value from the property.
	/// if (list.Count != 0)
	/// {
	///     var p = (ChainNode*)list[0]; // For example, get the first element.
	/// 
	///     // Code uses the variable 'p'.
	/// }
	/// </code>
	/// </remarks>
	/// <seealso cref="IntPtr"/>
	public readonly IReadOnlyList<IntPtr> Chain
	{
		get
		{
			List<IntPtr> todo;
			fixed (ChainNode* pThis = &this)
			{
				todo = new() { (IntPtr)pThis };
			}

			List<IntPtr> tempList = new(), done = new();
			while (todo.Count != 0)
			{
				var next = new List<IntPtr>();
				foreach (ChainNode* p in todo)
				{
					bool contains = false;
					foreach (ChainNode* node in done)
					{
						if (*node == *p)
						{
							contains = true;
							break;
						}
					}

					if (!contains)
					{
						done.Add((IntPtr)p);
						tempList.Add((IntPtr)p);
						for (int i = 0, count = p->_currentParentIndex; i < count; i++)
						{
							next.Add((IntPtr)p->_parents[i]);
						}
					}
				}

				todo = next;
			}

			return tempList;
		}
	}


#pragma warning disable CS1591
	public readonly void Deconstruct(out int candidate, out bool isOn)
	{
		candidate = Cell * 9 + Digit;
		isOn = IsOn;
	}
#pragma warning restore CS1591

	/// <summary>
	/// Determine whether the node is the parent of the specified node.
	/// </summary>
	/// <param name="node">The pointer that points to a chain node.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public readonly bool IsParentOf([NotNull, DisallowNull] ChainNode* node)
	{
		var pTest = node;
		while (pTest->_currentParentIndex != 0)
		{
			pTest = pTest->_parents[0];
			if (*pTest == this)
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	public override readonly int GetHashCode()
	{
		if (_parents is null)
		{
			return 0;
		}

		var hashCode = new HashCode();
		for (int i = 0; i < _currentParentIndex; i++)
		{
			var (cell, digit, isOn) = *_parents[i];
			hashCode.Add((isOn ? 729 : 0) + cell * 9 + digit);
		}

		return hashCode.ToHashCode();
	}

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString()
	{
		if (_currentParentIndex == 0)
		{
			return $"Candidate: {new Cells { Cell }}({Digit + 1})";
		}
		else
		{
			var nodes = Candidates.Empty;
			for (int i = 0; i < _currentParentIndex; i++)
			{
				var parent = _parents[i];
				nodes.AddAnyway(parent->Cell * 9 + parent->Digit);
			}

			return $"Candidate: {new Cells { Cell }}({Digit + 1}), Parent(s): {nodes}";
		}
	}

	/// <summary>
	/// Returns the reference to the first parent node.
	/// </summary>
	/// <returns>
	/// The reference to the first element of parents. If <see cref="_parents"/> is <see langword="null"/>,
	/// the return value is <see langword="ref"/> *(<see cref="ChainNode"/>**)<see langword="null"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ref readonly ChainNode* GetPinnableReference() =>
		ref _currentParentIndex == 0 ? ref *(ChainNode**)null : ref _parents[0];

	/// <summary>
	/// Append a chain node into the collection, as one of the parent nodes.
	/// </summary>
	/// <param name="ptr">The pointer that points to the chain node to be added.</param>
	/// <exception cref="InvalidOperationException">Throws when the buffer is full.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddParent([NotNull, DisallowNull] ChainNode* ptr)
	{
		if (_parents == null)
		{
			CreateParentsBlock();
		}

		if (_currentParentIndex >= 7)
		{
			throw new InvalidOperationException(
				"You can't append any elements into the collection because the buffer can only stores 7 elements."
			);
		}

		_parents[_currentParentIndex++] = ptr;
	}

	/// <summary>
	/// Release the memory, and clear all memories of this instance
	/// (specially for the property <see cref="_parents"/>).
	/// </summary>
	/// <seealso cref="_parents"/>
	public void Dispose()
	{
		foreach (ChainNode* ptr in Chain)
		{
			if (ptr->_parents != null)
			{
				NativeMemory.Free(ptr->_parents);
			}
		}
	}

	/// <summary>
	/// Initializes an creates the buffer.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[MemberNotNull(nameof(_parents))]
	private void CreateParentsBlock()
	{
		_parents = (ChainNode**)NativeMemory.Alloc((nuint)sizeof(ChainNode*) * 7);
		_currentParentIndex = 0;
	}
}
