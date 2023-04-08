namespace Sudoku.Resources;

/// <summary>
/// Represents an exception that will be thrown when a certain resource key cannot be found in the resource dictionary.
/// </summary>
/// <param name="resourceKey">The resource key that causes this error.</param>
/// <param name="assembly">Indicates the assembly where this error is thrown.</param>
public sealed class ResourceNotFoundException(string resourceKey, Assembly assembly) : Exception
{
	/// <summary>
	/// Indicates the resource key that causes this error.
	/// </summary>
	public string ResourceKey { get; } = resourceKey;

	/// <summary>
	/// Indicates which assembly causes this error.
	/// </summary>
	public Assembly Assembly { get; } = assembly;

	/// <inheritdoc/>
	public override string Message => $"Specified resource not found. Resource key: '{ResourceKey}', assembly: '{Assembly}'.";
}
