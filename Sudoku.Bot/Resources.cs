using System.Collections.Generic;

namespace Sudoku.Bot
{
	/// <summary>
	/// The resource dictionary.
	/// </summary>
	public static class Resources
	{
		/// <summary>
		/// The inner dictionary.
		/// </summary>
		private static readonly IDictionary<string, string> InnerDictionary = new Dictionary<string, string>
		{
			["MyName"] = "小蛋蛋",
			["Morning"] = "早上好",
			["MorningToo"] = "你也早鸭",
			["Size"] = "大小",
			["Grid"] = "盘面",
			["From"] = "从",
			["To"] = "到",
			["Success"] = "成功。",
			["Given"] = "提示数",
			["Modifiable"] = "填入数",
			["Cell"] = "单元格",
			["Candidate"] = "候选数",

			["AnalysisCommand"] = "！分析",
			["GeneratePictureCommand"] = "！生成图片",
			["GenerateEmptyGridCommand"] = "！生成空盘",
			["CleaningGridCommand"] = "！清盘",
			["IntroducingCommand"] = "！关于",
			["HelpCommand"] = "！帮助",
			["StartDrawingCommand"] = "！开始绘图",
			["FillCommand"] = "！填入",
			["DrawCommand"] = "！画",
			["EndDrawingCommand"] = "！结束绘图",

			["OptionalArg"] = "中括号项为可选内容，可以不写。",
			["RequiredArg"] = "小括号里的内容是可选项，但必须从小括号里以竖线分隔的项里选择一个。",
		};


		/// <summary>
		/// Get the resource key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>The value.</returns>
		/// <exception cref="KeyNotFoundException">Throws when the specified key can't be found.</exception>
		public static string GetValue(string key) => InnerDictionary[key];
	}
}
