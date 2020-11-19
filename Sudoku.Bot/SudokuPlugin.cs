#if AUTHOR_RESERVED

using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using HuajiTech.Mirai.Messaging;
using Sudoku.Drawing;

namespace Sudoku.Bot
{
	/// <summary>
	/// 封装一个关于数独相关操作的插件。
	/// </summary>
	public sealed partial class SudokuPlugin
	{
		/// <summary>
		/// 默认的图片绘制大小。
		/// </summary>
		[JsonIgnore]
		private const int Size = 800;

		/// <summary>
		/// 需要刷题的群的题库文件的文件夹路径。
		/// </summary>
		[JsonIgnore]
		private const string PuzzleLibDir = @"P:\Bot\题库\机器人题库";

		/// <summary>
		/// 刷题的群已经完成了的题目的存储文件夹路径。
		/// </summary>
		[JsonIgnore]
		private const string FinishedPuzzleDir = @"P:\Bot\题库\机器人题库_已完成";


		/// <summary>
		/// 随机数生成器。用来获取随机数。
		/// </summary>
		[JsonIgnore]
		private static readonly Random Rng = new();


		/// <summary>
		/// 表示项目内使用的设定项。
		/// </summary>
		internal static Settings Settings = new();

		/// <summary>
		/// 在程序内使用的绘图工具类。
		/// </summary>
		[JsonIgnore]
		private static GridPainter? _painter;


		/// <summary>
		/// 直接实例化一个插件对象，包含相关的执行事件。
		/// </summary>
		/// <param name="currentUserEventSource">执行事件操作的事件源。</param>
		public SudokuPlugin(CurrentUserEventSource currentUserEventSource) =>
			currentUserEventSource.GroupMessageReceivedEvent += OnReceivingMessageAsync;

		/// <summary>
		/// 当接收到消息的时候，触发该事件。
		/// </summary>
		/// <param name="session">当前会话。</param>
		/// <param name="e">群消息事件参数。</param>
		/// <returns>当前事件处理方法的封装 <see cref="Task"/> 对象。</returns>
		private static async Task OnReceivingMessageAsync(Session session, GroupMessageReceivedEventArgs e)
		{
			var complexStr = e.Message.Content;
			if (complexStr is not { Count: not 0 } || complexStr[0] is not PlainText pl)
			{
				return;
			}

			string info = pl.ToString();
			string[] splits = info.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			switch (info[0])
			{
				case '#':
					switch (splits)
					{
						case { Length: >= 1 } s:
							switch (splits[0][1..])
							{
								case "禁言": await JinxGroupAsync(s, e); break;
								case "解除禁言" or "取消禁言": await UnjinxGroupAsync(s, e); break;
							}
							break;
					}
					break;
				case '！' or '!':
					switch (splits)
					{
						case { Length: >= 1 } s:
							switch (s[0][1..])
							{
								case "帮助": await ShowHelperTextAsync(e); break;
								case "分析" when s.Length == 2:
									switch (s[1])
									{
										case "？": await AnalyzeHelpAsync(e); break;
										default: await AnalysisAsync(info, e); break;
									}
									break;
								case "生成图片" when s.Length >= 2:
									switch (s[1])
									{
										case "？": await GeneratePictureHelpAsync(e); break;
										default: await DrawImageAsync(info, e); break;
									}
									break;
								case "生成空盘":
									switch (s.Length)
									{
										case 1: await GenerateEmptyGridAsync(e); break;
										case >= 2 when s[1] == "？": await GenerateEmptyGridHelpAsync(e); break;
									}
									break;
								case "清盘" when s.Length >= 2:
									switch (s[1])
									{
										case "？": await CleanGridHelpAsync(e); break;
										default: await CleanGridAsync(info, e); break;
									}
									break;
								case "关于":
									switch (s.Length)
									{
										case 1: await IntroduceAsync(e); break;
										case 2 when s[1] == "？": await IntroduceHelpAsync(e); break;
									}
									break;
								case "开始绘图" when s.Length >= 2:
									switch (s[1])
									{
										case "？": await StartDrawingHelpAsync(e); break;
										default: await StartDrawingAsync(info, s, e); break;
									}
									break;
								case "结束绘图":
									switch (s.Length)
									{
										case 1: await DisposePainterAsync(e); break;
										case 2 when s[1] == "？": await DisposePainterHelpAsync(e); break;
									}
									break;
								case "填入" when s.Length >= 2:
									switch (s[1])
									{
										case "？": await FillHelpAsync(e); break;
										case "提示数": await FillAsync(s, e, true); break;
										case "填入数": await FillAsync(s, e, false); break;
									}
									break;
								case "画" when s.Length >= 2:
									switch (s[1])
									{
										case "？": await DrawHelpAsync(e); break;
										case "单元格": await DrawCellAsync(s, e, false); break;
										case "候选数": await DrawCandidateAsync(s, e, false); break;
										case "行": await DrawRowAsync(s, e, false); break;
										case "列": await DrawColumnAsync(s, e, false); break;
										case "宫": await DrawBlockAsync(s, e, false); break;
										case "链": await DrawChainAsync(s, e, false); break;
										case "叉叉": await DrawCrossAsync(s, e, false); break;
										case "圆圈": await DrawCircleAsync(s, e, false); break;
									}
									break;
								case "去掉" when s.Length >= 2:
									switch (s[1])
									{
										case "？": await DrawHelpAsync(e); break;
										case "单元格": await DrawCellAsync(s, e, true); break;
										case "候选数": await DrawCandidateAsync(s, e, true); break;
										case "行": await DrawRowAsync(s, e, true); break;
										case "列": await DrawColumnAsync(s, e, true); break;
										case "宫": await DrawBlockAsync(s, e, true); break;
										case "链": await DrawChainAsync(s, e, true); break;
										case "叉叉": await DrawCrossAsync(s, e, true); break;
										case "圆圈": await DrawCircleAsync(s, e, true); break;
									}
									break;
								case "抽题":
									switch (s.Length)
									{
										case 2 when s[1] == "？": await ExtractPuzzleHelpAsync(e); break;
										default: await ExtractPuzzleAsync(s, e); break;
									}
									break;
								case "命令符号说明":
									switch (s.Length)
									{
										case 1: await DescribeCommandNotationAsync(e); break;
										case 2 when s[1] == "？": await DescribeCommandNotationHelpAsync(e); break;
									}
									break;
								case "调整" when s.Length >= 2:
									switch (s[1])
									{
										case "？": await AdjustHelpAsync(e); break;
										case "确定值字体比例": await AdjustValueAsync(s, e); break;
										case "候选数字体比例": await AdjustCandidateAsync(s, e); break;
										case "背景色": await AdjustBackgroundAsync(s, e); break;
									}
									break;
							}
							break;
					}
					break;
			}
		}
	}
}

#endif