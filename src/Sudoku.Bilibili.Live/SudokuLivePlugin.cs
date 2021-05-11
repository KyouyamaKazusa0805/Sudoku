using System;
using System.IO;
using Sudoku.Bilibili.Live.Plugin;
using static System.Environment;

namespace Sudoku.Bilibili.Live
{
	/// <summary>
	/// Defines a sudoku live plugin.
	/// </summary>
	public sealed class SudokuLivePlugin : LivePlugin
	{
		/// <summary>
		/// Initializes an instance with the default instantiation behavior.
		/// </summary>
		public SudokuLivePlugin()
		{
			initializeEvents();
			initializeProperties();

			void initializeProperties()
			{
				PluginAuthor = "Sunnie";
				PluginName = "SudokuLivePlugin";
				PluginVersion = "v0.1";
			}

			void initializeEvents()
			{
				Connected += static (o, _) => { if (o is SudokuLivePlugin s) { s.Log("Disconnected."); } };
				Disconnected += static (o, _) => { if (o is SudokuLivePlugin s) { s.Log("Connected."); } };
				ReceivedDanmaku += static (_, e) =>
				{
					var model = e.Info;
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

						string? text = model.CommentText;
						if (!string.IsNullOrWhiteSpace(text))
						{
							File.WriteAllText(filePath, $"{text}{NewLine}");
						}
					}
					catch
					{
					}
				};
			}
		}


		/// <inheritdoc/>
		public override void Stop()
		{
			base.Stop();

			Console.WriteLine("Plugin Stopped!");
			Log("Plugin Stopped!");
		}

		/// <inheritdoc/>
		public override void Start()
		{
			base.Start();

			Console.WriteLine("Plugin Started!");
			Log("Plugin Started!");
		}
	}
}
