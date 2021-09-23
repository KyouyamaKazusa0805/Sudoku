#if !WINDOWS_APP

namespace Sudoku;

/// <summary>
/// Indicates the module initializer of this project.
/// </summary>
/// <remarks>
/// This type will be compiled and executed if the compilation symbol <c>WINDOWS_APP</c>
/// <b>isn't</b> configured in this project.
/// </remarks>
internal static class ModuleInitializer
{
	/// <summary>
	/// The initialize method.
	/// </summary>
	/// <exception cref="AssemblyFailedToLoadException">
	/// Throws when the deserialization operation is failed.
	/// </exception>
	[ModuleInitializer]
	public static void Initialize()
	{
		DeserializeResourceDictionary(nameof(TextResources.LangSourceEnUs), Paths.LangSourceEnUs);
		DeserializeResourceDictionary(nameof(TextResources.LangSourceZhCn), Paths.LangSourceZhCn);
	}

	/// <summary>
	/// Deserialize the resource dictionary. If failed, throw an exception.
	/// </summary>
	/// <param name="langSourceInstanceName">The name of the language resource instance.</param>
	/// <param name="path">The path to deserialize.</param>
	/// <exception cref="AssemblyFailedToLoadException">
	/// Throws when the specified files don't exist.
	/// </exception>
	/// <remarks>
	/// The exception throwing is relative with the conditional compilation symbol <c>WINDOWS_APP</c>.
	/// If the proejct configured the symbol <c>WINDOWS_APP</c>,
	/// <b>nothing</b> would happen when any error encountered.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DeserializeResourceDictionary(string langSourceInstanceName, string path)
	{
		if (!TextResources.Deserialize(langSourceInstanceName, path))
		{
			throw new AssemblyFailedToLoadException(Assembly.GetExecutingAssembly().FullName, path);
		}
	}
}

#endif