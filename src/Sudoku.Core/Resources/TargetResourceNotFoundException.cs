namespace Sudoku.Resources;

/// <summary>
/// Indicates an exception that will be thrown if target resource is not found.
/// </summary>
/// <param name="assembly">The assembly.</param>
/// <param name="resourceKey">The resource key.</param>
/// <param name="culture">The culture information.</param>
public sealed partial class TargetResourceNotFoundException(
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] Assembly assembly,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] string resourceKey,
	[Data(DataMemberKinds.Field, Accessibility = "private readonly")] CultureInfo? culture
) : Exception
{
	/// <summary>
	/// The "unspecified" text.
	/// </summary>
	private const string CultureNotSpecifiedDefaultText = "<Unspecified>";


	/// <inheritdoc/>
	public override string Message
		=> $"""
		Specified resource not found.
		* Resource key: '{_resourceKey}',
		* assembly: '{_assembly}',
		* culture: '{_culture?.EnglishName ?? CultureNotSpecifiedDefaultText}'
		""";

	/// <inheritdoc/>
	public override IDictionary Data
		=> new Dictionary<string, object?>
		{
			{ nameof(assembly), _assembly },
			{ nameof(resourceKey), _resourceKey },
			{ nameof(culture), _culture }
		};
}
