using System.Reflection;
using System.Runtime.Intrinsics;
using Sudoku.Collections;

namespace Sudoku.Test;

/// <summary>
/// Defines a chain node.
/// </summary>
public abstract class Node :
	IEquatable<Node>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<Node, Node>
#endif
{
	/// <summary>
	/// Indicates the bits used. <see cref="_higher"/> and <see cref="_lower"/>
	/// store the basic data of the cells used.
	/// </summary>
	protected readonly long _higher, _lower, _other;


	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="nodeType">The node type.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Node(NodeType nodeType, byte digit, in Cells cells)
	{
		var vector = cells.ToVector();
		_higher = vector.GetElement(0);
		_lower = vector.GetElement(1);
		_other = (int)nodeType << 4 | digit;
	}

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="higher">The higher 64 bits.</param>
	/// <param name="lower">The lower 64 bits.</param>
	/// <param name="other">The other 64 bits.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected Node(long higher, long lower, long other)
	{
		_higher = higher;
		_lower = lower;
		_other = other;
	}


	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public byte Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (byte)(_other & 15);
	}

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public Cells Cells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(_higher, _lower);
	}

	/// <summary>
	/// Indicates the type of the node.
	/// </summary>
	public NodeType Type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (NodeType)(int)(_other >> 4 & 7);
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as Node);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Node? other) =>
		other is not null
			&& _higher == other._higher && _lower == other._lower && _other == other._other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override int GetHashCode() => HashCode.Combine(_higher, _lower, _other);

	/// <summary>
	/// Gets the simplified string value that only displays the important information.
	/// </summary>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString() => $"{Digit + 1}{Cells}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString()
	{
		const string defaultName = "<Unnamed>";

		string nodeTypeName = defaultName;
		if (typeof(NodeType).GetField(Type.ToString()) is { } fieldInfo)
		{
			var attr = fieldInfo.GetCustomAttribute<NodeTypeNameAttribute<NodeType>>();
			nodeTypeName = attr is { Name: var name } ? name : (GetType().FullName ?? defaultName);
		}

		return $"{nodeTypeName}: {ToSimpleString()}";
	}


	/// <summary>
	/// Determines whether two <see cref="Node"/>s are same.
	/// </summary>
	/// <param name="left">Indicates the left-side instance to compare.</param>
	/// <param name="right">Indicates the right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Node? left, Node? right) =>
		(left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <summary>
	/// Determines whether two <see cref="Node"/>s are not totally same.
	/// </summary>
	/// <param name="left">Indicates the left-side instance to compare.</param>
	/// <param name="right">Indicates the right-side instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Node? left, Node? right) => !(left == right);
}
