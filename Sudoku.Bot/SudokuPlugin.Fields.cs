using System;
using System.Text.Json.Serialization;
using Sudoku.Drawing;

namespace Sudoku.Bot
{
	partial class SudokuPlugin
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
	}
}
