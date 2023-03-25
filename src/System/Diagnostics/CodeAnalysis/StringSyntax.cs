namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a list of <see cref="string"/> constants being used by <see cref="StringSyntaxAttribute"/>.
/// </summary>
/// <remarks>
/// In fact such fields are totally same as ones in attribute type <see cref="StringSyntaxAttribute"/>,
/// but I don't like to reference them using attribute full name.
/// </remarks>
/// <seealso cref="StringSyntaxAttribute"/>
public static class StringSyntax
{
	/// <summary>
	/// The syntax identifier for strings containing JavaScript Object Notation (JSON).
	/// </summary>
	public const string Json = nameof(Json);

	/// <summary>
	/// The syntax identifier for strings containing regular expressions.
	/// </summary>
	public const string Regex = nameof(Regex);

	/// <summary>
	/// The syntax identifier for strings containing URIs.
	/// </summary>
	public const string Uri = nameof(Uri);

	/// <summary>
	/// The syntax identifier for strings containing XML.
	/// </summary>
	public const string Xml = nameof(Xml);
}
