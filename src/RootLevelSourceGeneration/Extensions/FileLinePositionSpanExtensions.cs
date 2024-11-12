namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides with extension methods on <see cref="FileLinePositionSpan"/>.
/// </summary>
/// <seealso cref="FileLinePositionSpan"/>
internal static class FileLinePositionSpanExtensions
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	public static void Deconstruct(this FileLinePositionSpan @this, out LinePosition startLinePosition, out LinePosition endLinePosition)
		=> (startLinePosition, endLinePosition) = (@this.StartLinePosition, @this.EndLinePosition);
}
