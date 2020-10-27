using System;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Provides with a base class for program settings.
	/// </summary>
	public abstract partial class Settings : ICloneable<Settings>
	{
		/// <summary>
		/// To cover all settings.
		/// </summary>
		/// <param name="newSetting">The new settings instance.</param>
		public abstract void CoverBy(Settings newSetting);

		/// <inheritdoc/>
		public abstract Settings Clone();
	}
}
