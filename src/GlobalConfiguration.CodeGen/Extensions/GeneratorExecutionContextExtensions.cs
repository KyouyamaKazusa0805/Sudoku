namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="GeneratorExecutionContext"/>.
/// </summary>
/// <seealso cref="GeneratorExecutionContext"/>
internal static class GeneratorExecutionContextExtensions
{
	/// <summary>
	/// Simply calls <see cref="GeneratorExecutionContext.AddSource(string, string)"/>,
	/// </summary>
	/// <param name="this">The current context.</param>
	/// <param name="fileName">The file name. The file name may be same as the symbol name</param>
	/// <param name="sourceGeneratorName">The source generator name.</param>
	/// <param name="sourceCode">The source code.</param>
	/// <seealso cref="GeneratorExecutionContext.AddSource(string, string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AddSource(
		this ref GeneratorExecutionContext @this, string fileName,
		string? sourceGeneratorName, string sourceCode) =>
		@this.AddSource(
			$"{fileName}{(sourceGeneratorName is null ? string.Empty : $".g.{sourceGeneratorName}")}.cs",
			SourceText.From(sourceCode, Encoding.UTF8)
		);
}
