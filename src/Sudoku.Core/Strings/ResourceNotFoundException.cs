namespace Sudoku.Strings;

/// <summary>
/// Represents an exception that will be thrown when a certain resource key cannot be found in the resource dictionary.
/// </summary>
/// <param name="resourceKey">Indicates the resource key that causes this error.</param>
/// <param name="assembly">Indicates the assembly where this error is thrown.</param>
public sealed partial class ResourceNotFoundException([Data] string resourceKey, [Data] Assembly assembly) : Exception
{
	/// <inheritdoc/>
	public override string Message => $"Specified resource not found. Resource key: '{ResourceKey}', assembly: '{Assembly}'.";
}
