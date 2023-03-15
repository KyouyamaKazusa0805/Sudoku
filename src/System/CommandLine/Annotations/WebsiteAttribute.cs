namespace System.CommandLine.Annotations;

/// <summary>
/// Defines an attribute that is applied to an enumeration field, indicating the website of the field.
/// </summary>
/// <param name="uriString">The website of the field.</param>
/// <exception cref="ArgumentException">Throws when specified string value cannot be parsed into a valid URI link.</exception>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class WebsiteAttribute(string uriString) : Attribute
{
	/// <summary>
	/// Indicates the website of the enumeration field.
	/// </summary>
	public Uri Site { get; } =
		Uri.TryCreate(uriString, UriKind.Absolute, out var result)
			? result
			: throw new ArgumentException("The specified string cannot be parsed into a valid URI link.");
}
