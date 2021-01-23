using System.Reflection;
using System.Runtime.CompilerServices;
using Sudoku.Resources;

namespace Sudoku
{
	/// <summary>
	/// Indicates the assembly initializer.
	/// </summary>
	internal static class AssemblyInitializer
	{
		/// <summary>
		/// The initialize method.
		/// </summary>
		/// <exception cref="SudokuHandlingException">
		/// Throws when the deserialization operation is failed.
		/// </exception>
		[ModuleInitializer]
		public static void Initialize()
		{
			DeserializeResourceDictionary(nameof(TextResources.LangSourceEnUs), @"lang\Resources.en-us.dic");
			DeserializeResourceDictionary(nameof(TextResources.LangSourceZhCn), @"lang\Resources.zh-cn.dic");
		}

		/// <summary>
		/// Deserialize the resource dictionary. If failed, throw the exception.
		/// </summary>
		/// <param name="langSourceInstanceName">The name of the language resource instance.</param>
		/// <param name="path">The path to deserialize.</param>
		/// <exception cref="SudokuHandlingException">
		/// Throws when the specified files don't exist.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void DeserializeResourceDictionary(string langSourceInstanceName, string path)
		{
			if (!TextResources.Deserialize(langSourceInstanceName, path))
			{
				throw new SudokuHandlingException<string, string>(
					errorCode: 401,
					Assembly.GetExecutingAssembly().FullName!,
					path);
			}
		}
	}
}
