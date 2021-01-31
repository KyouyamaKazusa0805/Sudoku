using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sudoku.UI.Data
{
	/// <summary>
	/// Provides the custom settings used in this program.
	/// </summary>
	public sealed partial class Preferences
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
	}
}
