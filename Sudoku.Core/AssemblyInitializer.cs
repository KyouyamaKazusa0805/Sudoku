using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Runtime;
using Sudoku.Windows;

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
		/// <exception cref="SudokuRuntimeException">
		/// Throws when the deserialization operation is failed.
		/// </exception>
		[ModuleInitializer]
		public static void Initialize()
		{
			DeserializeResourceDictionary(nameof(Resources.LangSourceEnUs), @"lang\Resources.en-us.dic");
			DeserializeResourceDictionary(nameof(Resources.LangSourceZhCn), @"lang\Resources.zh-cn.dic");
		}

		/// <summary>
		/// Deserialize the resource dictionary. If failed, throw the exception.
		/// </summary>
		/// <param name="langSourceInstanceName">The name of the language resource instance.</param>
		/// <param name="path">The path to deserialize.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void DeserializeResourceDictionary(string langSourceInstanceName, string path)
		{
			if (!Resources.Deserialize(langSourceInstanceName, path))
			{
				throwException(path);
			}

			[DoesNotReturn]
			static void throwException(string path) =>
				throw new SudokuRuntimeException(
					$"Please check the existence of the resource dictionary file (path {path}).");
		}
	}
}
