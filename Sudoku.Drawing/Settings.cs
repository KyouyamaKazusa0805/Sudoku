using System;
using System.Linq;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Provides with a base class for program settings.
	/// </summary>
	public partial class Settings : ICloneable<Settings>
	{
		/// <summary>
		/// To cover all settings.
		/// </summary>
		/// <param name="newSetting">The new settings instance.</param>
		public virtual void CoverBy(Settings newSetting)
		{
			foreach (var property in
				from prop in GetType().GetProperties()
				where prop.CanWrite
				select prop)
			{
				property.SetValue(this, property.GetValue(newSetting));
			}
		}

		/// <inheritdoc/>
		public virtual Settings Clone()
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
