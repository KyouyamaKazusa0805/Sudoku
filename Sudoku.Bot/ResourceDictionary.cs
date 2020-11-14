using System.Collections.Generic;

namespace Sudoku.Bot
{
	/// <summary>
	/// The resource dictionary.
	/// </summary>
	public static class ResourceDictionary
	{
		/// <summary>
		/// The inner dictionary.
		/// </summary>
		private static readonly IDictionary<string, string> InnerDictionary = new Dictionary<string, string>
		{
			["AnalysisCommand"] = "-分析",
			["DrawingCommand"] = "-生成图片",
			["GenerateEmptyGridCommand"] = "-生成空盘",
			["CleaningGridCommand"] = "-清盘",
			["IntroducingCommand"] = "小蛋蛋，介绍一下你吧",
			["HelpCommand"] = "-帮助",

			["Help1"] = "帮助：",
			["Help2"] = "下面提示一些文字。",
			["Help3"] = "注：指令前面减号也是命令的一部分。",
			["HelpHelp"] = "-帮助：显示此帮助信息。",
			["HelpAnalyze"] = "-分析 <盘面>：显示题目的分析结果。",
			["HelpGeneratePicture"] = "-生成图片 <盘面>：将题目文本转为图片显示。",
			["HelpClean"] = "-清盘 <盘面>：将盘面前期的排除、唯一余数、区块和数组技巧全部应用。",
			["HelpEmpty"] = "-生成空盘：给一个空盘的图片。",
			["HelpIntroduceMyself"] = "小蛋蛋，介绍一下你吧：我 介 绍 我 自 己",

			["Introduce1"] = "Hello 大家好！我是一个机器人，叫小蛋蛋，是女孩子哦 (✿◡‿◡)",
			["Introduce2"] = "我爸还在给我加别的功能，所以我现在还需要大家的鼓励和支持哦，蟹蟹~",
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
