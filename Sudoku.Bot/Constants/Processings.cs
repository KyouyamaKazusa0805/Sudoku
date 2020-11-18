#if AUTHOR_RESERVED

using System.Text.Json;
using Sudoku.Drawing;

namespace Sudoku.Bot.Constants
{
	/// <summary>
	/// 给整个程序集提供处理的相关常量、只读量和操作。
	/// </summary>
	public static class Processings
	{
		/// <summary>
		/// 表示序列化的选项，用于 JSON 的序列化。
		/// </summary>
		public static readonly JsonSerializerOptions SerializerOption;


		/// <summary>
		/// 表示静态构造器。一般用于对一些静态对象的初始化。
		/// </summary>
		static Processings()
		{
			var jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
			jsonOptions.Converters.Add(new ColorJsonConverter());

			SerializerOption = jsonOptions;
		}
	}
}

#endif