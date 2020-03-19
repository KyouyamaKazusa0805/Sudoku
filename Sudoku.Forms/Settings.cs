using System;

namespace Sudoku.Forms
{
	/// <summary>
	/// To encapsulates a series of setting options for <see cref="MainForm"/> utility.
	/// </summary>
	public sealed partial class Settings : ICloneable<Settings>
	{
		/// <summary>
		/// To provides a default setting instance.
		/// </summary>
		public static readonly Settings DefaultSetting = new Settings();


		/// <inheritdoc/>
		public Settings Clone()
		{
			var resultInstance = new Settings();
			foreach (var property in GetType().GetProperties())
			{
				property.SetValue(resultInstance, property.GetValue(this));
			}

			return resultInstance;
		}
	}
}
