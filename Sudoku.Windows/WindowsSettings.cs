using System;
using System.Linq;
using Sudoku.Drawing;

namespace Sudoku.Windows
{
	/// <summary>
	/// To encapsulates a series of setting options for <see cref="MainWindow"/> utility.
	/// </summary>
	[Serializable]
	public sealed partial class WindowsSettings : Settings
	{
		/// <summary>
		/// To provides a default setting instance.
		/// </summary>
		[NonSerialized]
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

			foreach (var property in from Property in GetType().GetProperties() where Property.CanWrite select Property)
			{
				property.SetValue(this, property.GetValue(@new));
			}
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
