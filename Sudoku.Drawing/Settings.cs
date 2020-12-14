using System;
using System.Linq;
using Sudoku.DocComments;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Provides with a base class for program settings.
	/// </summary>
	public partial class Settings : ICloneable<Settings>
	{
		/// <inheritdoc	cref="DefaultConstructor"/>
		public Settings()
		{
		}

		/// <summary>
		/// (Copy constructor) Copies another settings instance.
		/// </summary>
		/// <param name="another">Another instance.</param>
		public Settings(Settings another) => InternalCoverBy(another);


		/// <summary>
		/// To cover all settings.
		/// </summary>
		/// <param name="newSetting">The new settings instance.</param>
		public virtual void CoverBy(Settings newSetting) => InternalCoverBy(newSetting);

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

		/// <summary>
		/// The internal covering.
		/// </summary>
		/// <param name="newSetting">The new settings.</param>
		protected void InternalCoverBy(Settings newSetting)
		{
			foreach (var property in
				from prop in GetType().GetProperties()
				where prop.CanWrite
				select prop)
			{
				property.SetValue(this, property.GetValue(newSetting));
			}
		}
	}
}
