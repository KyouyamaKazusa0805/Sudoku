using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku.Analytics;

/// <summary>
/// Represents a role that describes for a conclusion.
/// </summary>
/// <typeparam name="TSelf">Indicates the type of the implementation.</typeparam>
/// <typeparam name="TMask">Indicates the type of the mask.</typeparam>
public interface IConclusion<TSelf, TMask> : IComparable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>, IEquatable<TSelf>
	where TSelf : IConclusion<TSelf, TMask>
	where TMask : unmanaged, IBinaryInteger<TMask>
{
	/// <summary>
	/// The field uses the mask table of length 81 to indicate the state and all possible candidates
	/// holding for each cell. Each mask uses a <see cref="Mask"/> value, but only uses 11 of 16 bits.
	/// <code>
	/// | 16  15  14  13  12  11  10| 9   8   7   6   5   4   3   2   1   0 |
	/// |-----------------------|---|---------------------------------------|
	/// |   |   |   |   |   |   | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 1 | 0 | 1 |
	/// '-----------------------|---|---------------------------------------'
	///                          \_/ \_____________________________________/
	///                          (2)                   (1)
	/// </code>
	/// Where (1) is for candidate offset value (from 0 to 728), and (2) is for the conclusion type (assignment or elimination).
	/// Please note that the part (2) only use one bit because the target value can only be assignment (0) or elimination (1), but the real type
	/// <see cref="ConclusionType"/> uses <see cref="byte"/> as its underlying numeric type because C# cannot set "A bit"
	/// to be the underlying type. The narrowest type is <see cref="byte"/>.
	/// </summary>
	/// <remarks>
	/// Two <typeparamref name="TSelf"/> values can be compared with each other. If one of those two is an elimination
	/// (i.e. holds the value <see cref="Elimination"/> as the type), the instance will be greater;
	/// if those two hold same conclusion type, but one of those two holds the global index of the candidate position is greater, it is greater.
	/// </remarks>
	public abstract TMask Mask { get; }

	/// <summary>
	/// Indicates the cell.
	/// </summary>
	public virtual Cell Cell
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate / 9;
	}

	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public virtual Digit Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Candidate % 9;
	}

	/// <summary>
	/// The conclusion type to control the action of applying.
	/// If the type is <see cref="Assignment"/>, this conclusion will be set value (Set a digit into a cell);
	/// otherwise, a candidate will be removed.
	/// </summary>
	public abstract ConclusionType ConclusionType { get; }

	/// <summary>
	/// Indicates the candidate.
	/// </summary>
	public abstract Candidate Candidate { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<TSelf>.Equals([NotNullWhen(true)] TSelf? other) => other is not null && Mask == other.Mask;

	/// <inheritdoc/>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="other"/> is <see langword="null"/>
	/// when target type <typeparamref name="TSelf"/> is a nullable one.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable<TSelf>.CompareTo(TSelf? other)
	{
		ArgumentNullException.ThrowIfNull(other);

		return Mask.CompareTo(other.Mask);
	}


	/// <summary>
	/// Negates the current conclusion instance, changing the conclusion type from <see cref="Assignment"/> to <see cref="Elimination"/>,
	/// or from <see cref="Elimination"/> to <see cref="Assignment"/>.
	/// </summary>
	/// <param name="self">The current conclusion instance to be negated.</param>
	/// <returns>The negation.</returns>
	public static abstract TSelf operator ~(TSelf self);
}
