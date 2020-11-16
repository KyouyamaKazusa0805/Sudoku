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

			["Help1"] = "帮助：",
			["Help2"] = "下面提示一些文字。",
			["Help3"] = "注：指令前面减号也是命令的一部分；",
			["Help4"] = "另外，中间的空格需要严格遵守，缺少一个都会导致命令不能识别；",
			["Help5"] = "中括号为可选项，可以不写。",
			["HelpHelp"] = "！帮助：显示此帮助信息。",
			["HelpAnalyze"] = "！分析 <盘面>：显示题目的分析结果。",
			["HelpGeneratePicture"] = "！生成图片 <盘面>：将题目文本转为图片显示。",
			["HelpClean"] = "！清盘 <盘面>：将盘面前期的排除、唯一余数、区块和数组技巧全部应用。",
			["HelpEmpty"] = "！生成空盘：给一个空盘的图片。",
			["HelpStartDrawing"] = "！开始绘图[ 大小 <图片大小>][ 盘面 <盘面>]：开始从空盘画盘面图，随后可以添加其它的操作，例如添加候选数涂色等。",
			["HelpEndDrawing"] = "！结束绘图：指定画图过程结束，清除画板。",
			["HelpIntroduceMyself"] = "！关于：我 介 绍 我 自 己",

			["Introduce1"] = "Hello 大家好！我是一个机器人，叫小蛋蛋，是女孩子哦 (✿◡‿◡)",
			["Introduce2"] = "我爸还在给我加别的功能，所以我现在还需要大家的鼓励和支持哦，蟹蟹~",

			["Analysis1"] = "答案盘：",
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
