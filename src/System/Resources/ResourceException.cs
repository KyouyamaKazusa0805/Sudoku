namespace System.Resources;

/// <summary>
/// Represents an exception type that relates to resource dictionary.
/// </summary>
/// <param name="assembly">Indicates the target assembly.</param>
public abstract partial class ResourceException([Field(Accessibility = "protected readonly")] Assembly? assembly) : Exception
{
	/// <inheritdoc/>
	public abstract override string Message { get; }

	/// <inheritdoc/>
	public abstract override IDictionary Data { get; }
}
