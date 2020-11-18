#if AUTHOR_RESERVED

using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Sudoku.Bot.Constants;
using Sudoku.Drawing;

namespace Sudoku.Bot
{
	/// <summary>
	/// 该类表示当前整个程序集的初始化器，用于在载入的时候对项目进行一部分内容的初始化（比如序列化字段等）。
	/// </summary>
	internal static class AssemblyInitializer
	{
		/// <summary>
		/// 模块初始化器。
		/// </summary>
		[ModuleInitializer]
		public static void Initialize()
		{
			const string path = @"P:\Bot\插件设置.json";
			if (!File.Exists(path))
			{
				return;
			}

			try
			{
				var result = JsonSerializer.Deserialize<Settings>(File.ReadAllText(path), Processings.SerializerOption);
				if (result is null)
				{
					return;
				}

				SudokuPlugin.Settings = result;
			}
			catch
			{
			}
		}
	}
}

#endif