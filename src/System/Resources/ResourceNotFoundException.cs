namespace System.Resources;

/// <summary>
/// Indicates an exception that will be thrown if target resource is not found.
/// </summary>
/// <param name="assembly"><inheritdoc/></param>
/// <param name="resourceKey">The resource key.</param>
/// <param name="culture">The culture information.</param>
public sealed partial class ResourceNotFoundException(
	Assembly? assembly,
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "private readonly")] string resourceKey,
	[PrimaryConstructorParameter(MemberKinds.Field, Accessibility = "private readonly")] CultureInfo? culture
) : ResourceException(assembly)
{
	/// <summary>
	/// The "unspecified" text.
	/// </summary>
	private const string CultureNotSpecifiedDefaultText = "<Unspecified>";


	/// <inheritdoc/>
	public override string Message
		=> string.Format(
			SR.Get("Message_ResourceNotFoundException"),
#if NET9_0_OR_GREATER
			[
#endif
			_resourceKey,
			_assembly,
			_culture?.EnglishName ?? CultureNotSpecifiedDefaultText
#if NET9_0_OR_GREATER
			]
#endif
		);

	/// <inheritdoc/>
	public override IDictionary Data
		=> new Dictionary<string, object?>
		{
			{ nameof(assembly), _assembly },
			{ nameof(resourceKey), _resourceKey },
			{ nameof(culture), _culture }
		};
}
