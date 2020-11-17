#if AUTHOR_RESERVED

using System.Collections.Generic;

namespace Sudoku.Bot
{
	/// <summary>
	/// 表示整个项目可能会用到的资源字典。
	/// </summary>
	public static class Resources
	{
		/// <summary>
		/// 内部的字典。
		/// </summary>
		private static readonly IDictionary<string, string> InnerDictionary = new Dictionary<string, string>
		{
			["MyName"] = "小蛋蛋",
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
		/// 获取字典里的指定字符串。
		/// </summary>
		/// <param name="key">需要获取的字符串对应的键。</param>
		/// <returns>对应存储的数据。</returns>
		/// <exception cref="KeyNotFoundException">当在资源字典里无法找到对应键的时候抛出该异常。</exception>
		public static string GetValue(string key) => InnerDictionary[key];
	}
}

#endif