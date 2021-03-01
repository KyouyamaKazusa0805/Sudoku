using System;
using Sudoku.Drawing;

namespace Sudoku.UI.ComponentModels
{
	/// <summary>
	/// Encapsulates and provides a settings instance that is only used in UI (i.e. this project).
	/// </summary>
	public sealed partial class UiSettings : Settings
	{
		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException">
		/// Throws when the parameter <paramref name="newSetting"/> isn't of type <see cref="UiSettings"/>.
		/// </exception>
		public override void CoverBy(Settings newSetting)
		{
			// Check whether the arguments is of type 'UiSettings'
			// If not, throw an exception to report this error case.
			if (newSetting is not UiSettings inst)
			{
				throw new InvalidOperationException(
					$"Check failed: The argument '{nameof(newSetting)}' must be of type '{typeof(UiSettings)}'."
				);
			}

			InternalCoverBy(inst);
		}

		/// <inheritdoc/>
		public override UiSettings Clone()
		{
			var resultInstance = new UiSettings();
			foreach (var property in GetType().GetProperties())
			{
				property.SetValue(resultInstance, property.GetValue(this));
			}

			return resultInstance;
		}
	}
}
