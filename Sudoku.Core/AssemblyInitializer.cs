using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Sudoku.Windows;
using ResourceDictionary = System.Collections.Generic.IDictionary<string, string>;

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
			string path = @"lang\Resources.en-us.dic";
			if (File.Exists(path) && g(path, out var obj1))
			{
				Resources.LangSourceEnUs = obj1;
			}

			path = @"lang\Resources.zh-cn.dic";
			if (File.Exists(path) && g(path, out var obj2))
			{
				Resources.LangSourceZhCn = obj2;
			}

			static bool g(string path, [NotNullWhen(true)] out ResourceDictionary? result)
			{
				ResourceDictionary? r;
				try
				{
					r = JsonSerializer.Deserialize<ResourceDictionary>(File.ReadAllText(path));
				}
				catch
				{
					r = null;
				}

				if (r is not null)
				{
					result = r;
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
