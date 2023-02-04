namespace SudokuStudio.Models;

/// <summary>
/// Defines a sudoku grid serialization data.
/// </summary>
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
public sealed partial class GridSerializationData :
	IEquatable<GridSerializationData>,
	IEqualityOperators<GridSerializationData, GridSerializationData, bool>
{
	/// <summary>
	/// Indicates the format description.
	/// </summary>
	public string FormatDescription { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the grid string.
	/// </summary>
	public required string GridString { get; set; }

	[DebuggerHidden]
	private int HashCode => Grid.Parse(GridString).GetHashCode();


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] GridSerializationData? other)
	{
		if (other is null)
		{
			return false;
		}

		if (!Grid.TryParse(GridString, out var a) || !Grid.TryParse(other.GridString, out var b))
		{
			return false;
		}

		return a == b;
	}

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(HashCode))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(GridString), nameof(FormatDescription))]
	public override partial string ToString();
}
