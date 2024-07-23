namespace Sudoku.ComponentModel;

/// <summary>
/// Represents a stack that stores a list of <see cref="Operation"/> instances that can be undone and redone.
/// </summary>
/// <param name="capacity">
/// Indicates the desired capacity of the stacks.
/// If the internal stacks are full, the stacks will be expanded with double size.
/// </param>
/// <seealso cref="Operation"/>
public sealed class OperationStack(int capacity) : IUndoable
{
	/// <summary>
	/// Indicates the stacks to be used.
	/// </summary>
	private readonly Stack<Operation> _undoStack = new(capacity), _redoStack = new(capacity);


	/// <summary>
	/// Initializes an <see cref="OperationStack"/> instance.
	/// </summary>
	public OperationStack() : this(4)
	{
	}


	/// <summary>
	/// Indicates whether the undo stack is empty.
	/// </summary>
	public bool IsUndoOperationsEmpty => _undoStack.Count == 0;

	/// <summary>
	/// Indicates whether the redo stack is empty.
	/// </summary>
	public bool IsRedoOperationsEmpty => _redoStack.Count == 0;

	/// <summary>
	/// Indicates the number of operations can be undone.
	/// </summary>
	public int UndoOperationsCount => _undoStack.Count;

	/// <summary>
	/// Indicates the number of operations can be redone.
	/// </summary>
	public int RedoOperationsCount => _redoStack.Count;


	/// <summary>
	/// Clears both stacks.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		_undoStack.Clear();
		_redoStack.Clear();
	}

	/// <summary>
	/// Add a new <see cref="Operation"/> into the collection.
	/// </summary>
	/// <param name="operation">The operation to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(Operation operation)
	{
		_undoStack.Push(operation);
		_redoStack.Clear();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Undo()
	{
		if (_undoStack.Count != 0)
		{
			var operation = _undoStack.Pop();
			operation.Undo();
			_redoStack.Push(operation);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Redo()
	{
		if (_redoStack.Count != 0)
		{
			var operation = _redoStack.Pop();
			operation.Redo();
			_undoStack.Push(operation);
		}
	}
}
