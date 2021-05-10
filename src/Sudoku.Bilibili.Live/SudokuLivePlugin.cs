using System;
using System.IO;
using BilibiliDM_PluginFramework;
using static System.Environment;

namespace Sudoku.Bilibili.Live
{
	/// <summary>
	/// Defines a sudoku live plugin.
	/// </summary>
	public sealed class SudokuLivePlugin : DMPlugin
	{
		/// <summary>
		/// Initializes an instance with the default instantiation behavior.
		/// </summary>
		public SudokuLivePlugin()
		{
			Connected += SudokuLivePlugin_Connected;
			Disconnected += SudokuLivePlugin_Disconnected;
			ReceivedDanmaku += SudokuLivePlugin_ReceivedDanmaku;
			ReceivedRoomCount += SudokuLivePlugin_ReceivedRoomCount;
			PluginAuth = "Sunnie";
			PluginName = "SudokuLivePlugin";
			PluginVer = "v0.1";
		}


		/// <inheritdoc/>
		public override void Admin()
		{
			base.Admin();

			Console.WriteLine("Admin ok.");
			Log("Admin ok.");
			//AddDM("Admin ok", true);
		}

		/// <inheritdoc/>
		public override void Stop()
		{
			base.Stop();

			Console.WriteLine("Plugin Stopped!");
			Log("Plugin Stopped!");
			//AddDM("Plugin Stopped!", true);
		}

		/// <inheritdoc/>
		public override void Start()
		{
			base.Start();

			Console.WriteLine("Plugin Started!");
			Log("Plugin Started!");
			//AddDM("Plugin Started!", true);
		}

		private void SudokuLivePlugin_ReceivedRoomCount(object sender, ReceivedRoomCountArgs e)
		{
		}

		private void SudokuLivePlugin_ReceivedDanmaku(object sender, ReceivedDanmakuArgs e)
		{
			var model = e.Danmaku;
			if (model is null)
			{
				return;
			}

			string cachePath = Path.Combine(GetFolderPath(SpecialFolder.MyDocuments), "Sudoku_Temp");
			if (!Directory.Exists(cachePath))
			{
				Directory.CreateDirectory(cachePath);
			}

			string filePath = Path.Combine(cachePath, "TempFilling.cache");
			try
			{
				if (!File.Exists(filePath))
				{
					File.Create(filePath);
				}

				string text = model.CommentText;
				if (!string.IsNullOrWhiteSpace(text))
				{
					File.WriteAllText(filePath, $"{text}{NewLine}");
				}
			}
			catch
			{
			}
		}

		private void SudokuLivePlugin_Disconnected(object sender, DisconnectEvtArgs e) => Log("Connected.");

		private void SudokuLivePlugin_Connected(object sender, ConnectedEvtArgs e) => Log("Disconnected.");
	}
}
