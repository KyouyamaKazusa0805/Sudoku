using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sudoku.UI
{
	/// <summary>
	/// Encapsulates the preferences used in this program.
	/// </summary>
	public sealed class Preferences
	{
		/// <summary>
		/// Copy all values in this current instance to the specified instance.
		/// </summary>
		/// <param name="instance">The instance to receive all values.</param>
		public void CopyTo(Preferences instance)
		{
			foreach (var (prop, value) in GetPropertyInfos())
			{
				prop.SetValue(instance, value);
			}
		}

		/// <summary>
		/// To get all possible property information instances.
		/// </summary>
		/// <returns>All preference items.</returns>
		internal IEnumerable<(PropertyInfo Info, object Value)> GetPropertyInfos() =>
			from prop in typeof(Preferences).GetProperties()
			where prop.CanWrite
			let value = typeof(Preferences).GetProperty(prop.Name)!.GetValue(this)!
			select (prop, value);


		/// <summary>
		/// Indicates whether the program will ask you "Do you want to quit?" after you clicked the close button.
		/// The default value is <see langword="false"/>.
		/// </summary>
		public bool AskBeforeQuitting { get; set; } = false;

		/// <summary>
		/// Indicates whether the program use zero character <c>'0'</c> as placeholders. The default value
		/// <see langword="true"/>.
		/// </summary>
		public bool UseZeroCharacterWhenCopyCode { get; set; } = true;

		/// <summary>
		/// Indicates whether the program will only display same-level techniques while searching for all steps.
		/// The default value is <see langword="true"/>.
		/// </summary>
		public bool OnlyDisplaySameLevelStepsWhenFindAllSteps { get; set; } = true;
	}
}
