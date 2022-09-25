namespace System.Xml;

/// <summary>
/// Provides extension methods on <see cref="XmlDocument"/>.
/// </summary>
/// <seealso cref="XmlDocument"/>
internal static class XmlDocumentExtensions
{
	/// <summary>
	/// Try to load the XML document located to the specified path.
	/// </summary>
	/// <param name="this">The current XML document instance.</param>
	/// <param name="path">The path to load.</param>
	/// <returns>The current reference to the XML document.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static XmlDocument OnLoading(this XmlDocument @this, string path)
	{
		@this.Load(path);
		return @this;
	}
}
