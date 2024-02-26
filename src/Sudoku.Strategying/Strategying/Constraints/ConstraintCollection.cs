namespace Sudoku.Strategying.Constraints;

/// <summary>
/// Represents a constraint collection.
/// </summary>
[Equals]
[EqualityOperators]
public sealed partial class ConstraintCollection :
	HashSet<Constraint>,
	IEquatable<ConstraintCollection>,
	IEqualityOperators<ConstraintCollection, ConstraintCollection, bool>
{
	/// <summary>
	/// Gets the constraint at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The <see cref="Constraint"/> instance at the specified index.</returns>
	public Constraint this[int index]
	{
		get
		{
			var i = -1;
			foreach (var element in this)
			{
				if (++i == index)
				{
					return element;
				}
			}

			throw new IndexOutOfRangeException();
		}
	}


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] ConstraintCollection? other)
		=> other is not null && Count == other.Count && SetEquals(other);

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hashCode = new HashCode();
		foreach (var element in this)
		{
			hashCode.Add(element);
		}

		return hashCode.ToHashCode();
	}
}
