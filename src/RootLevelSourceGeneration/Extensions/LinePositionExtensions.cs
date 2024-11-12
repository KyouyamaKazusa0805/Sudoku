namespace Microsoft.CodeAnalysis.Text;

/// <summary>
/// Provides with extension methods on <see cref="LinePosition"/>.
/// </summary>
/// <seealso cref="LinePosition"/>
internal static class LinePositionExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	public static void Deconstruct(this LinePosition @this, out int line, out int character)
		=> (line, character) = (@this.Line, @this.Character);
}
