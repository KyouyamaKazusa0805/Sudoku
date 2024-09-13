namespace Sudoku.Runtime.InteropServices;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <typeparam name="TRating">
/// Indicates the type of the rating value. The value must be a number (e.g. a <see cref="double"/> or an <see cref="int"/>).
/// </typeparam>
public abstract class ProgramMetadataAttribute<TRating> : ProgramMetadataAttribute
	where TRating : unmanaged, INumberBase<TRating>
{
	/// <summary>
	/// Indicates the type of the rating value.
	/// </summary>
	public TRating Rating { get; init; }
}
