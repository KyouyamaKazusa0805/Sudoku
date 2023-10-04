using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using static Sudoku.Analytics.ConclusionType;

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
	/// </summary>
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
