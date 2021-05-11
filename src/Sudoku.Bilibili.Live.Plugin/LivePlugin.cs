using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Provides a live plugin.
	/// </summary>
	/// <remarks>
	/// Please note that this plugin instance will use WPF library (e.g. <see cref="DispatcherObject"/>).
	/// If you want to use the code here, please creates a new WPF library project to guarantee that
	/// <c>WindowsBase.dll</c> exists.
	/// </remarks>
	/// <seealso cref="DispatcherObject"/>
	public abstract class LivePlugin : DispatcherObject, INotifyPropertyChanged
	{
		/// <summary>
		/// Indicates the status.
		/// </summary>
		private bool _status = false;


		/// <summary>
		/// Indicates the status of this plugin.
		/// </summary>
		public bool Status
		{
			get => _status;

			private set
			{
				if (value != _status)
				{
					_status = value;

					OnPropertyChanged(nameof(Status));
				}
			}
		}

		/// <summary>
		/// Indicates the current room ID.
		/// </summary>
		public int? RoomId { get; private set; }

		/// <summary>
		/// Indicates the name of the plugin.
		/// </summary>
		public string PluginName { get; set; } = string.Empty;

		/// <summary>
		/// Indicates the author of this plugin.
		/// </summary>
		public string PluginAuthor { get; set; } = string.Empty;

		/// <summary>
		/// Indicates the contact of the author of this plugin.
		/// </summary>
		public string PluginContact { get; set; } = string.Empty;

		/// <summary>
		/// Indicates the version of this plugin.
		/// </summary>
		public string PluginVersion { get; set; } = string.Empty;

		/// <summary>
		/// Indicates the description of this plugin.
		/// </summary>
		public string PluginDescription { get; set; } = string.Empty;


		/// <summary>
		/// Indicates the event to be triggered when the property has been changed its status.
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// Indicates the event to be triggered when connected.
		/// </summary>
		public event ConnectedEventHandler? Connected;

		/// <summary>
		/// Indicates the event to be triggered when disconnected.
		/// </summary>
		public event DisconnectEventHandler? Disconnected;

		/// <summary>
		/// Indicates the event to be triggered when received a danmaku.
		/// </summary>
		public event ReceivedDanmakuEventHandler? ReceivedDanmaku;

		/// <summary>
		/// Indicates the event to be triggered when received the room count.
		/// </summary>
		public event ReceivedRoomCountEventHandler? ReceivedRoomCount;


		/// <summary>
		/// The inner method that is executed when connected to the server.
		/// </summary>
		/// <param name="roomId">The room ID.</param>
		public void ConnectedCore(int roomId)
		{
			RoomId = roomId;

			try
			{
				Connected?.Invoke(this, roomId);
			}
			catch (Exception ex)
			{
				Log($"The plugin has encountered an error while connecting. Detail: {ex.Message}");
			}
		}

		/// <summary>
		/// The inner method that is executed when received a danmaku.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		public void ReceivedDanmakuCore(ReceivedDanmakuEventArgs e)
		{
			try
			{
				ReceivedDanmaku?.Invoke(this, e);
			}
			catch (Exception ex)
			{
				Log($"The plugin has encountered an error while receiving a danmaku. Detail: {ex.Message}");
			}
		}

		/// <summary>
		/// Inner method that received the room count.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		public void ReceivedRoomCountCore(ReceivedRoomCountEventArgs e)
		{
			try
			{
				ReceivedRoomCount?.Invoke(this, e);
			}
			catch (Exception ex)
			{
				Log($"The plugin has encountered an error while room count. Detail: {ex.Message}");
			}
		}

		/// <summary>
		/// Inner method that disconnected from the server.
		/// </summary>
		public void DisconnectedCore()
		{
			RoomId = null;

			try
			{
				Disconnected?.Invoke(this, DisconnectEventArgs.Empty);
			}
			catch (Exception ex)
			{
				Log($"The plugin has encountered an error while disconnecting. Detail: {ex.Message}");
			}
		}

		/// <summary>
		/// Executed when the plugin is started.
		/// </summary>
		public virtual void Start() => Status = true;

		/// <summary>
		/// Executed when the plugin is stopped.
		/// </summary>
		public virtual void Stop() => Status = false;

		/// <summary>
		/// Executed when the plugin is reserved by the caller.
		/// </summary>
		public virtual void Reserve()
		{
		}

		/// <summary>
		/// Executed when the plugin is init'ed.
		/// </summary>
		public virtual void Inited()
		{
		}

		/// <summary>
		/// Executed when the plugin is exited.
		/// </summary>
		public virtual void Exited()
		{
		}

		/// <summary>
		/// Log the message.
		/// </summary>
		/// <param name="text">The text.</param>
		public void Log(string text)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(action));

			void action()
			{
				dynamic mw = Application.Current.MainWindow;
				mw.logging($"{PluginName} {text}");
			}
		}

		/// <summary>
		/// Raises a danmaku.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="fullscreen">Indicates whether the window is full-screen.</param>
		public void RaiseDanmaku(string text, bool fullscreen = false)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(action));

			void action()
			{
				dynamic mw = Application.Current.MainWindow;
				mw.AddDMText(PluginName, text, true, fullscreen);
			}
		}

		/// <summary>
		/// To execute when the property has been changed.
		/// </summary>
		/// <param name="propertyName">
		/// The property name corresponding to that property that changed its status.
		/// </param>
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
			PropertyChanged?.Invoke(this, new(propertyName));
	}
}
