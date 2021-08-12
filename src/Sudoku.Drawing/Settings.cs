using System.Drawing;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Provides with a base class for program settings.
	/// </summary>
	public partial class Settings : ICloneable<Settings>
	{
		/// <summary>
		/// Initializes a default <see cref="Settings"/> instance.
		/// </summary>
		public Settings()
		{
		}

		/// <summary>
		///  Copies another settings instance.
		/// </summary>
		/// <param name="another">Another instance.</param>
		public Settings(Settings another) => InternalCoverBy(another);


		/// <summary>
		/// To cover all settings.
		/// </summary>
		/// <param name="newSetting">The new settings instance.</param>
		public virtual void CoverBy(Settings newSetting) => InternalCoverBy(newSetting);

		/// <inheritdoc/>
		public virtual Settings Clone() => InternalClone<Settings>();

		/// <summary>
		/// Try to get the palette color using the specified index.
		/// </summary>
		/// <param name="id">The ID value.</param>
		/// <param name="result">The result color.</param>
		/// <returns>A <see cref="bool"/> indicating whether the operation is successful.</returns>
		public bool TryGetPaletteColor(int id, out Color result)
		{
			if (id is >= 0 and < 15)
			{
				result = id switch
				{
					0 => Color1,
					1 => Color2,
					2 => Color3,
					3 => Color4,
					4 => Color5,
					5 => Color6,
					6 => Color7,
					7 => Color8,
					8 => Color9,
					9 => Color10,
					10 => Color11,
					11 => Color12,
					12 => Color13,
					13 => Color14,
					14 => Color15
				};
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}

		/// <summary>
		/// The internal covering.
		/// </summary>
		/// <param name="newSetting">The new settings.</param>
		protected void InternalCoverBy(Settings newSetting)
		{
			foreach (var property in from prop in GetType().GetProperties() where prop.CanWrite select prop)
			{
				property.SetValue(this, property.GetValue(newSetting));
			}
		}

		/// <summary>
		/// The internal clone.
		/// </summary>
		/// <typeparam name="TSettings">The type of the cloneation.</typeparam>
		/// <returns>The result instance.</returns>
		protected TSettings InternalClone<TSettings>() where TSettings : Settings, new()
		{
			var resultInstance = new TSettings();
			foreach (var property in GetType().GetProperties())
			{
				property.SetValue(resultInstance, property.GetValue(this));
			}

			return resultInstance;
		}
	}
}
