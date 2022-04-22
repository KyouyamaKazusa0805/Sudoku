namespace System.CommandLine.Annotations;

/// <summary>
/// Defines an attribute that is applied to an enumeration field, indicating the website of the field.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class WebsiteAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="WebsiteAttribute"/> instance via the website of the field.
	/// </summary>
	/// <param name="uriString">The website of the field.</param>
	public WebsiteAttribute(string uriString) =>
		Site = Uri.TryCreate(uriString, UriKind.Absolute, out var result)
			? result
			: throw new ArgumentException("The specified string cannot be parsed as a URI link.");


	/// <summary>
	/// Indicates the website of the enumeration field.
	/// </summary>
	public Uri Site { get; }
}
