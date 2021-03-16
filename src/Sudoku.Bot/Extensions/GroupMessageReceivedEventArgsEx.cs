using System;
using System.IO;
using System.Threading.Tasks;
using HuajiTech.Mirai.Http;
using HuajiTech.Mirai.Http.Events;
using DImage = System.Drawing.Image;
using HImage = HuajiTech.Mirai.Http.Messaging.Image;

namespace Sudoku.Bot.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="GroupMessageReceivedEventArgs"/>.
	/// </summary>
	/// <seealso cref="GroupMessageReceivedEventArgs"/>
	public static class GroupMessageReceivedEventArgsEx
	{
		/// <summary>
		/// Send the message without mention someone.
		/// </summary>
		/// <param name="this">The event arguments.</param>
		/// <param name="message">The message to send.</param>
		/// <returns>The task.</returns>
		public static async Task NormalSendAsync(this GroupMessageReceivedEventArgs @this, MessageElement message) =>
			await @this.Source.SendAsync(message);

		/// <summary>
		/// To reply an image.
		/// </summary>
		/// <param name="this">The event arguments.</param>
		/// <param name="image">The image to reply.</param>
		/// <param name="path">
		/// The path to store the file. Due to the technical implementation, the method will save the file
		/// to the specified path, and then delete the file after used.
		/// </param>
		/// <returns>The task.</returns>
		public static async Task ReplyImageAsync(
			this GroupMessageReceivedEventArgs @this, DImage image, string path)
		{
			path = $@"{path}\Temp.png";

			try
			{
				image.Save(path);

				var hImage = new HImage(new Uri(path));
				await @this.ReplyAsync(hImage);
			}
			finally
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
		}
	}
}
