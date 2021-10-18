namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="SourceProductionContext"/>.
/// </summary>
/// <seealso cref="SourceProductionContext"/>
internal static class SourceProductionContextExtensions
{
	/// <summary>
	/// Simply calls <see cref="SourceProductionContext.AddSource(string, string)"/>.
	/// </summary>
	/// <param name="this">The current context.</param>
	/// <param name="fileName">The file name. The file name may be same as the symbol name</param>
	/// <param name="sourceGeneratorName">The source generator name.</param>
	/// <param name="sourceCode">The source code.</param>
	/// <seealso cref="SourceProductionContext.AddSource(string, string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AddSource(
		this ref SourceProductionContext @this, string fileName,
		string? sourceGeneratorName, string sourceCode) =>
		@this.AddSource(
			$"{fileName}{(sourceGeneratorName is null ? string.Empty : $".g.{sourceGeneratorName}")}.cs",
			SourceText.From(sourceCode, Encoding.UTF8)
		);

	/// <summary>
	/// Simply calls <see cref="SourceProductionContext.AddSource(string, SourceText)"/>.
	/// </summary>
	/// <param name="this">The current context.</param>
	/// <param name="fileName">The file name. The file name may be same as the symbol name</param>
	/// <param name="sourceGeneratorName">The source generator name.</param>
	/// <param name="sourceCode">The source code.</param>
	/// <param name="encoding">The encoding.</param>
	/// <seealso cref="SourceProductionContext.AddSource(string, SourceText)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AddSource(
		this ref SourceProductionContext @this, string fileName,
		string? sourceGeneratorName, string sourceCode, Encoding encoding) =>
		@this.AddSource(
			$"{fileName}{(sourceGeneratorName is null ? string.Empty : $".g.{sourceGeneratorName}")}.cs",
			SourceText.From(sourceCode, encoding)
		);
}