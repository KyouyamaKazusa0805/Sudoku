#if AUTHOR_RESERVED

using System;
using System.IO;
using System.Threading.Tasks;
using HuajiTech.Mirai;
using HuajiTech.Mirai.Events;
using DImage = System.Drawing.Image;
using HImage = HuajiTech.Mirai.Messaging.Image;

namespace Sudoku.Bot.Extensions
{
	/// <summary>
	/// 为 <see cref="MessageReceivedEventArgs"/> 实例提供扩展方法。
	/// </summary>
	public static class ReplyEx
	{
		/// <summary>
		/// 用来临时保存、缓存绘图后的图片。
		/// </summary>
		private const string TemporaryPath = @"C:\Users\Howdy\Desktop\Temp.png";


		/// <summary>
		/// 回复一个图片。
		/// </summary>
		/// <param name="e">(<see langword="this"/> 参数) 事件参数。</param>
		/// <param name="image">需要回复的图片。</param>
		/// <returns>提供异步操作的具体 <see cref="Task"/> 实例。</returns>
		public static async Task ReplyImageAsync(this MessageReceivedEventArgs e, DImage image)
		{
			image.Save(TemporaryPath);

			var hImage = new HImage(new Uri(TemporaryPath));
			await e.Reply(hImage);

			if (File.Exists(TemporaryPath))
			{
				File.Delete(TemporaryPath);
			}
		}
	}
}

#endif