using System;
using System.Text.Json.Serialization;
using Sudoku.Drawing;

namespace Sudoku.Windows
{
	/// <summary>
	/// To encapsulates a series of setting options for <see cref="MainWindow"/> utility.
	/// </summary>
	public sealed partial class WindowsSettings : Settings
	{
		/// <summary>
		/// To provides a default setting instance.
		/// </summary>
		[JsonIgnore]
		public static readonly WindowsSettings DefaultSetting = new();


		/// <summary>
		/// To cover all settings.
		/// </summary>
		/// <param name="newSetting">The new settings instance.</param>
		/// <exception cref="ArgumentException">
		/// Throws when the instance is not of type <see cref="WindowsSettings"/>.
		/// </exception>
		public override void CoverBy(Settings newSetting)
		{
			_ = newSetting is not WindowsSettings @new ? throw new ArgumentException("The specified argument is invalid due to its invalid type.", nameof(newSetting)) : 0;

			InternalCoverBy(@new);
		}

		/// <inheritdoc/>
		public override Settings Clone()
		{
			var resultInstance = new WindowsSettings();
			foreach (var property in GetType().GetProperties())
			{
				property.SetValue(resultInstance, property.GetValue(this));
			}

			return resultInstance;
		}
	}
}
