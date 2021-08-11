using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sudoku.Resources;

namespace Sudoku
{
	/// <summary>
	/// Indicates the module initializer of this project.
	/// </summary>
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
			DeserializeResourceDictionary(nameof(TextResources.LangSourceEnUs), Paths.LangSourceEnUs, out _);
			DeserializeResourceDictionary(nameof(TextResources.LangSourceZhCn), Paths.LangSourceZhCn, out _);
		}

		/// <summary>
		/// Deserialize the resource dictionary. If failed, throw an exception.
		/// </summary>
		/// <param name="langSourceInstanceName">The name of the language resource instance.</param>
		/// <param name="path">The path to deserialize.</param>
		/// <param name="result">
		/// The result that indicates whether the deserialization operation
		/// is successful.
		/// </param>
		/// <exception cref="AssemblyFailedToLoadException">
		/// Throws when the specified files don't exist.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void DeserializeResourceDictionary(
			string langSourceInstanceName, string path, [DoesNotReturnIf(false)] out bool result)
		{
			if (!TextResources.Deserialize(langSourceInstanceName, path))
			{
				result = false;
				throw new AssemblyFailedToLoadException(Assembly.GetExecutingAssembly().FullName, path);
			}

			result = true;
		}
	}
}
