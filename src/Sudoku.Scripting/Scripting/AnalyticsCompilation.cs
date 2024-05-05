namespace Sudoku.Scripting;

/// <summary>
/// Represents a way to easily handle for analytics APIs in high-level reflection.
/// </summary>
internal static class AnalyticsCompilation
{
	/// <summary>
	/// Creates a <see cref="CSharpCompilation"/> as environment on operating with APIs.
	/// </summary>
	/// <returns>A <see cref="CSharpCompilation"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CSharpCompilation CreateCompilation()
		=> CSharpCompilation.Create(
			"InternalScriptCompilation",
			references: [
				..
				from assembly in AppDomain.CurrentDomain.GetAssemblies()
				where !assembly.IsDynamic
				let location = assembly.Location
				where !string.IsNullOrEmpty(location)
				select MetadataReference.CreateFromFile(location),
				MetadataReference.CreateFromFile(typeof(CellMap).Assembly.Location), // Sudoku.Core
				MetadataReference.CreateFromFile(typeof(Analyzer).Assembly.Location), // Sudoku.Analytics
			],
			options: new CSharpCompilationOptions(
				OutputKind.DynamicallyLinkedLibrary,
				allowUnsafe: true,
				checkOverflow: false,
				concurrentBuild: true,
				optimizationLevel: OptimizationLevel.Release,
				platform: Platform.X64
			)
		);
}
