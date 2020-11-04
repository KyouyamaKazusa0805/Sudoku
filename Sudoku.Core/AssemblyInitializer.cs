using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
		[ModuleInitializer]
		public static void Initialize()
		{
			string path = "Resources.en-us.json";
			if (File.Exists(path) && g(path, out var obj1))
			{
				Resources.LangSourceEnUs = obj1;
			}

			path = "Resources.zh-cn.json";
			if (File.Exists(path) && g(path, out var obj2))
			{
				Resources.LangSourceZhCn = obj2;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool g(string path, [NotNullWhen(true)] out IDictionary<string, string>? result)
			{
				if (JsonSerializer.Deserialize<IDictionary<string, string>>(File.ReadAllText(path))
					is IDictionary<string, string> obj)
				{
					result = obj;
					return true;
				}
				else
				{
					result = null;
					return false;
				}
			}
		}
	}
}
