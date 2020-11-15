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
	/// Encapsulates methods for <see cref="MessageReceivedEventArgs"/>.
	/// </summary>
	public static class ReplyEx
	{
		/// <summary>
		/// The temporary path to save image.
		/// </summary>
		private const string TemporaryPath = @"C:\Users\Howdy\Desktop\Temp.png";


		/// <summary>
		/// Reply an image.
		/// </summary>
		/// <param name="e">(<see langword="this"/> parameter) The event arguments.</param>
		/// <param name="image">The image.</param>
		/// <returns>The task of this method.</returns>
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

		/// <summary>
		/// Reply an image with the message.
		/// </summary>
		/// <param name="e">(<see langword="this"/> parameter) The event arguments.</param>
		/// <param name="image">The image.</param>
		/// <param name="message">The additional message.</param>
		/// <returns>The task of this method.</returns>
		public static async Task ReplyImageWithTextAsync(
			this MessageReceivedEventArgs e, DImage image, string message)
		{
			image.Save(TemporaryPath);

			var hImage = new HImage(new Uri(TemporaryPath));
			await e.Reply(message + hImage);

			if (File.Exists(TemporaryPath))
			{
				File.Delete(TemporaryPath);
			}
		}
	}
}
