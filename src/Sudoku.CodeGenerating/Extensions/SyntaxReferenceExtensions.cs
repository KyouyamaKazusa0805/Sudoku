namespace Microsoft.CodeAnalysis;

#if !SUPPORT_SOURCE_GENERATOR
/// <summary>
/// Provides extension methods on <see cref="SyntaxReference"/>.
/// </summary>
/// <seealso cref="SyntaxReference"/>
internal static class SyntaxReferenceExtensions
{
#pragma warning disable IDE0079
#pragma warning disable CS1591
	public static void Deconstruct(this SyntaxReference @this, out TextSpan textSpan, out SyntaxNode syntaxNode)
	{
		textSpan = @this.Span;
		syntaxNode = @this.GetSyntax();
	}
#pragma warning restore CS1591
#pragma warning restore IDE0079
}
#else
internal sealed class SyntaxReferenceDeconstructArgumentProvider
{
	private SyntaxReferenceDeconstructArgumentProvider() { }


	[DeconstructArgumentProvider]
	internal static SyntaxNode SyntaxNode(SyntaxReference @this) => @this.GetSyntax();
}
#endif
