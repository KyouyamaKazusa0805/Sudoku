namespace System.Resources;

/// <summary>
/// Represents an exception that will be thrown if resource manager is missing.
/// </summary>
/// <param name="assembly"><inheritdoc/></param>
public sealed class MissingResourceManagerException(Assembly assembly) : ResourceException(assembly)
{
	/// <inheritdoc/>
	public override string Message
		=> string.Format(
			SR.Get("Message_MissingResourceManagerException"),
			[_assembly, nameof(SR), nameof(SR.RegisterResourceManager)]
		);

	/// <inheritdoc/>
	public override IDictionary Data => new Dictionary<string, object?> { { nameof(assembly), _assembly } };
}
