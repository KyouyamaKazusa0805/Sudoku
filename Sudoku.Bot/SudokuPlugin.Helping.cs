#if AUTHOR_RESERVED

using System;
using System.Text;
using System.Threading.Tasks;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;

namespace Sudoku.Bot
{
	partial class SudokuPlugin
	{
		/// <summary>
		/// 分析题目帮助。
		/// </summary>
		private static async Task AnalyzeHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！分析 <题目>。")
				.Append("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.ToString());

		/// <summary>
		/// 生成图片帮助。
		/// </summary>
		private static async Task GeneratePictureHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！生成图片 <题目>。")
				.Append("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.ToString());

		/// <summary>
		/// 生成空盘帮助。
		/// </summary>
		private static async Task GenerateEmptyGridHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！生成空盘。");

		/// <summary>
		/// 清盘帮助。
		/// </summary>
		private static async Task CleanGridHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！清盘 <题目>。")
				.Append("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.ToString());

		/// <summary>
		/// 介绍程序帮助。
		/// </summary>
		private static async Task IntroduceHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！关于。");

		/// <summary>
		/// 开始绘图帮助。
		/// </summary>
		private static async Task StartDrawingHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！开始绘图[ 大小 <大小>][ 盘面 <题目>]。")
				.AppendLine("图片大小需要是一个不超过 1000 的正整数，表示多大的图片，以像素为单位；")
				.AppendLine("题目代码目前可支持普通文本格式、Hodoku 的中间盘面格式等。")
				.Append("大小默认为 800，盘面默认为空盘。")
				.ToString());

		/// <summary>
		/// 完成绘图帮助。
		/// </summary>
		private static async Task DisposePainterHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！结束绘图。");

		/// <summary>
		/// 符号说明帮助。
		/// </summary>
		private static async Task DescribeCommandNotationHelpAsync(GroupMessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！命令符号说明。");

		/// <summary>
		/// 填充数字帮助。
		/// </summary>
		private static async Task FillHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync("格式：！填入 (提示数|填入数) <数字> 到 <单元格>。");

		/// <summary>
		/// 调整设置项帮助。
		/// </summary>
		private static async Task AdjustHelpAsync(GroupMessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：")
				.AppendLine("！调整 (确定值字体比例|候选数字体比例) <放大倍数>")
				.AppendLine("！调整 背景色 <颜色>")
				.AppendLine("其中“放大倍数”是一个非负小数，保留一位小数，表示字体相对于单元格的大小。")
				.AppendLine("在默认设置项里，该数值在确定值里默认为 0.9，在候选数里默认为 0.3（提供参考）；")
				.AppendLine("“颜色”支持 ARGB 颜色序列（透明分量、红色度、绿色度、蓝色度，分量范围均为 0 到 255，数字越大颜色越浅）；")
				.AppendLine("盘面背景色建议使用灰色，即 R、G、B 分量数值全部相同。")
				.ToString());

		/// <summary>
		/// 画画功能帮助。
		/// </summary>
		private static async Task DrawHelpAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：")
				.AppendLine($"！(去掉|画) 单元格 <单元格> <颜色>；")
				.AppendLine($"！(去掉|画) 候选数 <单元格> <数字> <颜色>；")
				.AppendLine($"！(去掉|画) 行 <行编号> <颜色>；")
				.AppendLine($"！(去掉|画) 列 <列编号> <颜色>；")
				.AppendLine($"！(去掉|画) 宫 <宫编号> <颜色>；")
				.AppendLine($"！(去掉|画) 链 从 <单元格> <数字> 到 <单元格> <数字>")
				.AppendLine($"！(去掉|画) 叉叉 <单元格>；")
				.AppendLine($"！(去掉|画) 圆圈 <单元格>。")
				.AppendLine("其中“颜色”项可设置为红、橙、黄、绿、青、蓝、紫七种颜色以及对应的浅色；但黄色除外。")
				.AppendLine("“颜色”同样支持 ARGB 颜色序列（透明分量、红色度、绿色度、蓝色度，分量范围均为 0 到 255，数字越大颜色越浅）。")
				.ToString());

		/// <summary>
		/// 抽取题目帮助。
		/// </summary>
		private static async Task ExtractPuzzleHelpAsync(GroupMessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！抽题 [不带候选数|带候选数]。")
				.Append("默认为带候选数。")
				.ToString());

		/// <summary>
		/// 符号说明。
		/// </summary>
		private static async Task DescribeCommandNotationAsync(GroupMessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("命令符号说明：")
				.AppendLine("(a|b|c|...)：表示内容为必选项，并在其中必须选择且只能选择一个作为结果。")
				.AppendLine("[内容]：表示内容为可选项，可以不写，并带有默认值。")
				.AppendLine("[a|b|c|...]：表示内容为可选项，可以不写，如果需要写则必须在内部选择一个。")
				.Append("<参数>：表示数据是一个根据上文传递过来的一个变量，可以根据命令的要求进行适当的调整，不一定限制它的数据范围。")
				.ToString());

		/// <summary>
		/// 帮助功能帮助。
		/// </summary>
		private static async Task ShowHelperTextAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("格式：！帮助。")
				.AppendLine("机器人可识别的功能有以下一些：")
				.AppendLine("！帮助：显示此帮助信息。")
				.AppendLine("！分析 <盘面>：显示题目的分析结果。")
				.AppendLine("！生成图片 <盘面>：将题目文本转为图片显示。")
				.AppendLine("！填入 (提示数|填入数) <数字> 到 <单元格>：填入提示数或填入数。")
				.AppendLine("！清盘 <盘面>：将盘面前期的排除、唯一余数、区块和数组技巧全部应用。")
				.AppendLine("！生成空盘：给一个空盘的图片。")
				.AppendLine("！开始绘图[ 大小 <图片大小>][ 盘面 <盘面>]：开始从空盘或指定盘面开始为盘面进行绘制，比如添加候选数涂色等。")
				.AppendLine("！结束绘图：指定画图过程结束，清除画板。")
				.AppendLine("！抽题：抽取一道题库里的题目。")
				.AppendLine("！调整：调整设置项的具体内容。")
				.AppendLine("！命令符号说明：解释命令里使用的括号的对应含义。")
				.AppendLine("！关于：我 介 绍 我 自 己")
				.AppendLine()
				.AppendLine("注：为和普通聊天信息作区分，命令前面的叹号“！”也是命令的一部分；")
				.AppendLine("每一个命令的详情信息请输入“命令 ？”来获取。")
				.AppendLine()
				.Append(DateTime.Today.ToString("yyyy-MM-dd ddd"))
				.ToString());

		/// <summary>
		/// 介绍自己。
		/// </summary>
		private static async Task IntroduceAsync(MessageReceivedEventArgs e) =>
			await e.Source.SendAsync(
				new StringBuilder()
				.AppendLine("Hello 大家好！我是一个机器人，叫小蛋蛋，是女孩子哦 (✿◡‿◡)")
				.Append("我爸还在给我加别的功能，所以我现在还需要大家的鼓励和支持哦，蟹蟹~")
				.ToString());
	}
}

#endif