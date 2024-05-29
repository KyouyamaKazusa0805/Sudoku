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
			ResourceDictionary.Get("Message_MissingResourceManagerException"),
#if NET9_0_OR_GREATER
			[
#endif
			_assembly,
			nameof(ResourceDictionary),
			nameof(ResourceDictionary.RegisterResourceManager)
#if NET9_0_OR_GREATER
			]
#endif
		);

	/// <inheritdoc/>
	public override IDictionary Data => new Dictionary<string, object?> { { nameof(assembly), _assembly } };
}
